using System.Runtime.Serialization;

namespace PgnViewer.Shared
{

    public class ChessMove
    {
        public int Id { get; set; } = 0;
        public string White { get; set; }

        public string Black { get; set; }
    }
}
