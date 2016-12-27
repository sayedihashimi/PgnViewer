namespace PgnViewer.Shared
{

    public class ChessMove
    {
        public int MoveNumber { get; set; } = 0;
        public Halfmove WhiteMove { get; set; } = Halfmove.NullMove;
        public Halfmove BlackMove { get; set; } = Halfmove.NullMove;
    }
}
