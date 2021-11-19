using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Caro_LAN
{
    public class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;
        private List<Player> player;
        private int currentPlayer;
        private TextBox playerName;
        private PictureBox player1Mark;
        private PictureBox player2Mark;
        private Panel info;
        private Label announce;

        private List<List<Button>> matrix;

        public Panel ChessBoard { get => chessBoard; set => chessBoard = value; }
        public List<Player> Player { get => player; set => player = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox PlayerName { get => playerName; set => playerName = value; }
        public PictureBox Player1Mark { get => player1Mark; set => player1Mark = value; }
        public PictureBox Player2Mark { get => player2Mark; set => player2Mark = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public Panel Info { get => info; set => info = value; }
        public Label Announce { get => announce; set => announce = value; }

        private event EventHandler<ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private Stack<PlayInfo> playTimeLine;
        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark1, PictureBox mark2, Panel info, Label announce )
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.Player1Mark = mark1;
            this.Player2Mark = mark2;
            this.Info = info;
            this.Announce = announce;

            mark1.Location = new Point(4, 74);
            mark1.Enabled = true;
            mark2.Location = new Point(120, 74);

            announce.Location = new Point(32, 23);
            announce.Text = "VS";

            this.Player = new List<Player>()
            {
                new Player("HowKteam", Image.FromFile(Application.StartupPath + "\\Resources\\Caro1.JPG")),
                new Player("Free Education", Image.FromFile(Application.StartupPath + "\\Resources\\Caro2.JPG"))
            };
            
        }
        #endregion

        #region Method
        public void DrawChessBoard()
        {
            playTimeLine = new Stack<PlayInfo>();

            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();

            CurrentPlayer = 0;
            Player1Mark.BackgroundImage = Player[0].Mark;
            Player2Mark.BackgroundImage = Player[1].Mark;

            ChangePlayer();

            Matrix = new List<List<Button>>();

            for (int i = 0; i < Constant.CHESSBOARD_HEIGHT; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Constant.CHESSBOARD_WIDHT; j++)
                {
                    Button bt = new Button()
                    {
                        Size = new Size() { Width = Constant.CHESS_WIDTH, Height = Constant.CHESS_HEIGHT },
                        Location = new Point() { X = Constant.CHESS_WIDTH * j, Y = Constant.CHESS_HEIGHT * i},
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                };
                    bt.Click += Bt_Click;
                    ChessBoard.Controls.Add(bt);
                    Matrix[i].Add(bt);
                }
            }
        }
        private void Mark(Button bt)
        {
            bt.BackgroundImage = Player[CurrentPlayer].Mark;
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;
            Player1Mark.BorderStyle = Player1Mark.BorderStyle == BorderStyle.None ? BorderStyle.Fixed3D : BorderStyle.None;
            Player2Mark.BorderStyle = Player2Mark.BorderStyle == BorderStyle.None ? BorderStyle.Fixed3D : BorderStyle.None;
        }
        private bool IsEndGame(Button bt)
        {
            return IsEndHorizontal(bt) || IsEndVertical(bt) || IsEndPrimaryDiagonal(bt) || IsEndSubDiagonal(bt);
        }
        private Point GetChessPoint(Button bt)
        { 
            int vertical = Convert.ToInt32(bt.Tag);
            int horizontal = Matrix[vertical].IndexOf(bt);

            Point point = new Point(vertical, horizontal);
            return point;
        }
        private bool IsEndHorizontal(Button bt)
        {
            Point point = GetChessPoint(bt);

            int countLeft = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[point.X][i].BackgroundImage != bt.BackgroundImage) break;

                countLeft++;
            }

            int countRight = 0;
            for(int i = point.Y+1; i<Constant.CHESSBOARD_WIDHT;i++)
            {
                if (Matrix[point.X][i].BackgroundImage != bt.BackgroundImage) break;

                countRight++;
            }

            return countLeft + countRight == 5;
        }
        private bool IsEndVertical(Button bt)
        {
            Point point = GetChessPoint(bt);

            int countTop = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[i][point.Y].BackgroundImage != bt.BackgroundImage) break;

                countTop++;
            }

            int countBottom = 0;
            for (int i = point.X + 1; i < Constant.CHESSBOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.Y].BackgroundImage != bt.BackgroundImage) break;

                countBottom++;
            }

            return countTop + countBottom == 5;
        }
        private bool IsEndPrimaryDiagonal(Button bt)
        {
            Point point = GetChessPoint(bt);

            int countLeft = 0;
            int a = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[i][point.Y+a].BackgroundImage != bt.BackgroundImage) break;
                a++;
                countLeft++;
            }

            int countRight = 0;
            int b = 1;
            for (int i = point.X + 1; i < Constant.CHESSBOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.Y-b].BackgroundImage != bt.BackgroundImage) break;
                b++;
                countRight++;
            }

            return countLeft + countRight == 5;
        }
        private bool IsEndSubDiagonal(Button bt)
        {
            Point point = GetChessPoint(bt);

            int countLeft = 0;
            int a = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[i][point.Y-a].BackgroundImage != bt.BackgroundImage) break;
                a++;
                countLeft++;
            }

            int countRight = 0;
            int b = 1;
            for (int i = point.X + 1; i < Constant.CHESSBOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.Y+b].BackgroundImage != bt.BackgroundImage) break;
                b++;
                countRight++;
            }

            return countLeft + countRight == 5;
        }
        private void EndGame()
        {
            if (endedGame != null) endedGame(this, new EventArgs());
        }
        public bool Undo()
        {
            if (playTimeLine.Count <= 0) return false;

            PlayInfo playInfo = playTimeLine.Pop();

            Button bt = Matrix[playInfo.Point.X][playInfo.Point.Y];
            CurrentPlayer = playInfo.CurrentPlayer;
            bt.BackgroundImage = null;

            ChangePlayer();

            return false;
        }
        private void Bt_Click(object sender, EventArgs e)
        {
            Button bt = sender as Button;
            
            if (bt.BackgroundImage != null) return;

            Mark(bt);

            playTimeLine.Push(new PlayInfo(GetChessPoint(bt), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if (playerMarked != null) playerMarked(this, new ButtonClickEvent(GetChessPoint(bt)));

            if (IsEndGame(bt)) EndGame();

        }

        public void OtherPlayerMark(Point point)
        {
            Button bt = Matrix[point.X][point.Y];

            if (bt.BackgroundImage != null) return;

            Mark(bt);

            playTimeLine.Push(new PlayInfo(GetChessPoint(bt), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if (IsEndGame(bt)) EndGame();
        }
        #endregion

        public class ButtonClickEvent: EventArgs
        {
            private Point clickedPoint;

            public Point ClickedPoint { get => clickedPoint; set => clickedPoint = value; }

            public ButtonClickEvent(Point point)
            {
                this.ClickedPoint = point;
            }
        }

    }
}
