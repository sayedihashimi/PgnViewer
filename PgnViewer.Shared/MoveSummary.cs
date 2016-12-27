using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnViewer.Shared
{
    public class MoveSummary
    {
        public string MoveString { get; set; }
        public string OriginSquare { get; set; }
        public string TargetSquare { get; set; }        
    }

    public class Halfmove
    {
        public string MoveString { get; set; }
    }

    public class ChessMove
    {
        public int MoveNumber { get; set; }
        public Halfmove WhiteMove { get; set; }
        public Halfmove BlackMove { get; set; }
    }
    public class GameMoves
    {
        public List<ChessMove> Moves { get; set; }

        public GameMoves()
        {
            Moves = new List<ChessMove>();
        }
    }
}
