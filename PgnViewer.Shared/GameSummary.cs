using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnViewer.Shared
{
    public class GameSummary
    {
        public string Id { get; set; }
        public int Index { get; set; } = 0;
        public string White { get; set; }
        public string Black { get; set; }

        // TODO: Remove pgn from game summary
        public string Pgn { get; set; }

        public override string ToString()
        {
            return $"[White={White}, Black={Black}], Pgn = {Pgn}";
        }    
    }

    //public class GameDetails
    //{
    //    public int Index { get; set; } = 0;
    //    public string White { get; set; }
    //    public string Black { get; set; }
    //    public string Fen { get; set; }
    //    public string Pgn { get; set; }
    //    public List<MoveSummary> Moves { get; set; }
    //}
}
