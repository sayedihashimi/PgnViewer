using PgnViewer.Shared;
using PgnViewerApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PgnViewerApi.Controllers {
    public class GameController : ApiController {
        private string PgnFolder { get; } = System.Web.Hosting.HostingEnvironment.MapPath($"~/pgnfiles");

        public ChessGame GetGame(string filename, int index) {
            string filepath = $"{PgnFolder}/{filename}";
            var gamesRw = new PgnGamesReaderWriter();
            string gamePgn = gamesRw.GetGameFromFile(filepath, index);

            return GameHelper.BuildChessGameFrom(gamePgn);
        }
    }
}
