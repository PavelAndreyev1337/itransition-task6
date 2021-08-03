using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Collections.Concurrent;

namespace Task6.Models
{
    public class GameModel
    {
        public string Id { get; }

        public string Name;

        public string[] Board = new string[9]{
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty,
            System.String.Empty
        };

        public KeyValuePair<bool, PlayerModel> GameWon()
        {
            if(Board[0] == Board[1] && Board[1] == Board[2] && Board[2] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[2] == "X") ? player1 : player2);
            if(Board[3] == Board[4] && Board[4] == Board[5] && Board[5] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[5] == "X") ? player1 : player2);
            if(Board[6] == Board[7] && Board[7] == Board[8] && Board[8] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[8] == "X") ? player1 : player2);
            if(Board[0] == Board[3] && Board[3] == Board[6] && Board[6] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[6] == "X") ? player1 : player2);
            if(Board[1] == Board[4] && Board[4] == Board[7] && Board[7] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[7] == "X") ? player1 : player2);
            if(Board[2] == Board[5] && Board[5] == Board[8] && Board[8] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[8] == "X") ? player1 : player2);
            if(Board[0] == Board[4] && Board[4] == Board[8] && Board[8] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[8] == "X") ? player1 : player2);
            if(Board[2] == Board[4] && Board[4] == Board[6] && Board[6] != System.String.Empty)
                return new KeyValuePair<bool, PlayerModel>(true, (Board[6] == "X") ? player1 : player2);
            
            return new KeyValuePair<bool, PlayerModel>(false, null);
        }

        public bool player1turn = true;
        public PlayerModel player1 { get; set; }
        public PlayerModel player2 { get; set; }

        public List<PlayerModel> watchers = new List<PlayerModel>();

        public GameModel(PlayerModel creator)
        {
            Id = creator.Id;
            Name = "name";
            player1 = creator;
            player2 = null;
        }
    }
}