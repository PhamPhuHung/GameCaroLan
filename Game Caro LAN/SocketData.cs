using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Caro_LAN
{
    [Serializable]
    public class SocketData
    {
        private int command;
        private Point point;
        private string message;

        public int Command { get => command; set => command = value; }
        public Point Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }

        public SocketData(int command, Point point, string message)
        {
            this.Command = command;
            this.Point = point;
            this.Message = message;
        }

        public enum SocketCommand
        {
            SEND_POINT = 0,
            NOTIFY = 1,
            NEW_GAME = 2,
            UNDO = 3,
            END_GAME = 4,
            QUIT = 5
        }
    }
}
