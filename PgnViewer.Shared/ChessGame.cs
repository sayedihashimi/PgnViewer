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
        public string Pgn { get; set; }
        public List<ChessMove> Moves { get; set; } = new List<ChessMove>();
    }
}
