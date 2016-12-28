using PgnViewer.Shared;
using PgnViewerApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace PgnViewerApi.Controllers {
    public class GameController : ApiController {
        private string PgnFolder { get; } = System.Web.Hosting.HostingEnvironment.MapPath($"~/pgnfiles");

        public ChessGame GetGame(string filename, int index) {
            string filepath = $"{PgnFolder}/{filename}";
            var gamesRw = new PgnGamesReaderWriter();
            string gamePgn = gamesRw.GetGameFromFile(filepath, index);
            var game = GameHelper.BuildChessGameFrom(gamePgn);
            game.Pgn = gamePgn;

            string fenstring = GetFenStringFrom(gamePgn);
            if (!string.IsNullOrEmpty(fenstring)) {
                game.Fen = fenstring;
            }

            return game;
        }

        private string GetFenStringFrom(string gamePgn) {
            var gamesRw = new PgnGamesReaderWriter();
            using (var reader = new StringReader(gamePgn)) {
                string currentLine = null;
                Regex fenregex = new Regex(@"\[[fF][eE][nN]\s['""](.*)['""]]");
                while ((currentLine = reader.ReadLine()) != null) {
                    if (fenregex.IsMatch(currentLine)) {
                        Match match = fenregex.Match(currentLine);
                        if (match != null && match.Groups.Count >= 2) {
                            var canidateStr = match.Groups[1].Value;
                            if (!string.IsNullOrEmpty(canidateStr)) {
                                return canidateStr;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
