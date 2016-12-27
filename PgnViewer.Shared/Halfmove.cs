using System;

namespace PgnViewer.Shared
{

    public class Halfmove
    {
        public string MoveString { get; set; }
        public Halfmove(string moveString)
        {
            MoveString = moveString;
        }

        public static Halfmove NullMove {
            get {
                return new Halfmove(null);
            }
        }

        public override string ToString()
        {
            return MoveString;
        }

        public override bool Equals(object obj)
        {
            Halfmove other = obj as Halfmove;
            if (other == null ||
                MoveString != null && other.MoveString == null) {
                return false;
            }
            else if(MoveString == null && other.MoveString == null) {
                return true;
            }

            return string.Equals(MoveString, other.MoveString);
        }

        public override int GetHashCode()
        {
            int result = 1;
            if(MoveString != null) {
                result += MoveString.GetHashCode();
            }

            return result;
        }
    }
}
