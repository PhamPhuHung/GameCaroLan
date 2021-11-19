using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static Game_Caro_LAN.ChessBoardManager;

namespace Game_Caro_LAN
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager Socket;
        #endregion
        public Form1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;
            ChessBoard = new ChessBoardManager(pnChessBoard, tbPlayerName, pbPlayer1, pbPlayer2, pnInfo, lbAnnounce);
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            ChessBoard.EndedGame += ChessBoard_EndedGame;

            pbCountDown.Step = Constant.COUNT_DOWN_STEP;
            pbCountDown.Maximum = Constant.COUNT_DOWN_TIME;

            tmCountDown.Interval = Constant.COUNT_DOWN_INTERVAL;

            Socket = new SocketManager();

            NewGame();
            
        }
        #region Methods

        private void EndGame()
        {
            undoToolStripMenuItem.Enabled = false;
            tmCountDown.Stop();
            pnChessBoard.Enabled = false;
            pbCountDown.Value = 0;
            MessageBox.Show("End Game");
        }

        private void NewGame()
        {
            undoToolStripMenuItem.Enabled = true;
            tmCountDown.Stop();
            pbCountDown.Value = 0;

            ChessBoard.DrawChessBoard();

        }

        private void Undo()
        {
            ChessBoard.Undo();
        }

        private void Quit()
        {
                Application.Exit();
        }

        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmCountDown.Start();
            pnChessBoard.Enabled = false;
            pbCountDown.Value = 0;

            Socket.Send(new SocketData((int)SocketData.SocketCommand.SEND_POINT, e.ClickedPoint, ""));
            Listen();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCountDown_Tick(object sender, EventArgs e)
        {
            pbCountDown.PerformStep();

            if(pbCountDown.Value >= pbCountDown.Maximum)
            {
                EndGame();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to quit game?", "Quit Game", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            tbIP.Text = Socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(tbIP.Text)) tbIP.Text = Socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            if (string.IsNullOrEmpty(tbIP.Text)) tbIP.Text = "127.0.0.1";
        }

        private void btLAN_Click(object sender, EventArgs e)
        {
            Socket.IP = tbIP.Text;

            if (!Socket.ConnectServer())
            {
                Socket.isServer = true;
                pnChessBoard.Enabled = true;
                Socket.CreateServer();
            }
            else
            {
                Socket.isServer = false;
                pnChessBoard.Enabled = false;
                Listen();
            }

        }

        private void Listen()
        {

            Thread listenThread = new Thread(() =>
            {
                try
                {
                    SocketData data = (SocketData)Socket.Receive();

                    ProcessData(data);
                }
                catch { }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }
        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketData.SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                        {
                            pbCountDown.Value = 0;
                            pnChessBoard.Enabled =true;
                            tmCountDown.Start();
                            ChessBoard.OtherPlayerMark(data.Point);
                        }));
                    break;
                case (int)SocketData.SocketCommand.NOTIFY:
                    break;
                case (int)SocketData.SocketCommand.NEW_GAME:
                    break;
                case (int)SocketData.SocketCommand.UNDO:
                    break;
                case (int)SocketData.SocketCommand.END_GAME:
                    break;
                case (int)SocketData.SocketCommand.QUIT:
                    break;
                default:
                    break;
            }
            Listen();
        }
        #endregion


    }
}
