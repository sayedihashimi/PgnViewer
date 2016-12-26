using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnViewer.Shared
{
    public class GameSummary
    {
        public string White { get; set; }
        public string Black { get; set; }
        public string Pgn { get; set; }
        public string Fenstring { get; set; }
        public List<MoveSummary> Moves{ get; set; }

        public override string ToString()
        {
            return $"[White={White}, Black={Black}], Pgn = {Pgn}";
        }
        
        public string GetMovesAsJson()
        {
            string result = null;

            if(Moves != null)
            {
                result = Newtonsoft.Json.JsonConvert.SerializeObject(Moves);
            }

            return result;
        }
    }
}
