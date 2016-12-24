using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PgnViewerApi.Models
{
    public class GameSummary
    {
        public string White { get; set; }
        public string Black { get; set; }
        public string Pgn { get; set; }
    }
}