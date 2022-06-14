using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameLogic
{
    public class CurrentGameData
    {
        public List<Player> Players { get; set; }
        
        // Turn objects
        public Player CurrentPlayer { get { return Players[PlayerPointer]; } }

        private int PlayerPointer;

        public void NextTurn()
        {
            // change current player
            PlayerPointer++;
            if (PlayerPointer >= Players.Count)
            {
                PlayerPointer = 0;
            }
        }
    }
}
