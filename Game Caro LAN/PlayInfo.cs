using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Caro_LAN
{
    class PlayInfo
    {
        #region Properties
        private Point point;
        private int currentPlayer;

        public Point Point { get => point; set => point = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        #endregion

        #region Initalize
        public PlayInfo(Point point, int currentPlayer)
        {
            this.Point = point;
            this.CurrentPlayer = currentPlayer;
        }
        #endregion

        #region Methods
        #endregion
    }
}
