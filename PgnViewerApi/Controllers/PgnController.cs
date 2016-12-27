using ilf.pgn;
using ilf.pgn.Data;
using Newtonsoft.Json;
using PgnViewer.Shared;
using PgnViewerApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PgnViewerApi.Controllers
{
    public class PgnController : ApiController
    {     
        private List<MoveSummary> GetMoves(string gamepgn)
        {
            PgnReader reader = new PgnReader();
            Database pgnResult = reader.ReadFromString(gamepgn);
            Game game = null;
            List<MoveSummary> moves = new List<MoveSummary>();
            if(pgnResult != null || pgnResult.Games != null && pgnResult.Games.Count >= 1)
            {
                game = pgnResult.Games[0];
            }
            if(game != null)
            {
                foreach(var move in game.MoveText.GetMoves())
                {
                    moves.Add(new MoveSummary
                    {
                        MoveString = move.ToString(),
                        OriginSquare = move.OriginSquare != null ? move.OriginSquare.ToString() : string.Empty,
                        TargetSquare = move.TargetSquare != null ? move.TargetSquare.ToString() : string.Empty
                    });
                }
            }

            return moves;
        }

        [HttpPost]
        public List<GameSummary> GetGames([FromBody]string pgn)
        {
            if( !string.IsNullOrWhiteSpace(pgn))
            {
                PgnReader reader = new PgnReader();
                Database pgnResult = reader.ReadFromString(pgn);

                List<GameSummary> summarylist = new List<GameSummary>();
                int index = 0;
                foreach(var g in pgnResult.Games)
                {
                    summarylist.Add(
                        new GameSummary
                        {
                            Index = index++,
                            White = g.WhitePlayer,
                            Black = g.BlackPlayer,
                            Pgn = g.ToString()
                        });
                }

                return summarylist;
            }
            else
            {
                return null;
            }
        }   
    }

    public class GamesController : ApiController {
        [HttpPost]
        public ChessGame GetChessGame([FromUri]int index, [FromBody]string gamepgn) {
            var chessgame = GameHelper.BuildChessGameFrom(gamepgn, index);

            return chessgame;
        }
    }
}
