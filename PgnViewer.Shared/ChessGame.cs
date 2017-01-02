using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnViewer.Shared
{
    public class ChessGame
    {
        public int Index { get; set; } = 0;
        public string White { get; set; }
        public string Black { get; set; }
        public string Fen { get; set; }
        public string Event { get; set; }
        public string FirstPlayer { get; set; } = "white";
        public string Result { get; set; }
        public string Pgn { get; set; }
        // public List<ChessMove> Moves { get; set; } = new List<ChessMove>();
        public List<ChessHalfmove> Moves { get; set; } = new List<ChessHalfmove>();
    }
    public class ChessHalfmove {
        public ChessHalfmove() : this(-1, null) {
        }
        public ChessHalfmove(int halfmoveId, string move) {
            Id = halfmoveId;
            Move = move;
        }
        public int Id { get; set; }
        public string Move { get; set; }        
    }
}
