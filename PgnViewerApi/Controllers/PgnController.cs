using ilf.pgn;
using ilf.pgn.Data;
using Newtonsoft.Json;
using PgnViewer.Shared;
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

        private GameMoves GetMoves(string gamepgn)
        {
            PgnReader reader = new PgnReader();
            Database pgnResult = reader.ReadFromString(gamepgn);
            Game game = null;
            List<MoveSummary> moves = new List<MoveSummary>();
            GameMoves allMoves = new GameMoves();

            if (pgnResult != null || pgnResult.Games != null && pgnResult.Games.Count >= 1)
            {
                game = pgnResult.Games[0];
            }
            if (game != null)
            {
                int moveCount = 1;
                foreach (var move in game.MoveText.GetMoves())
                {
                    allMoves.Moves.Add(new ChessMove
                    {
                        MoveNumber = moveCount++
                        //WhiteMove
                    });

                }
            }
                    throw new NotImplementedException();
        }

        [HttpPost]
        public List<GameSummary> GetGames([FromBody]string pgn)
        {
            if( !string.IsNullOrWhiteSpace(pgn))
            {
                PgnReader reader = new PgnReader();
                Database pgnResult = reader.ReadFromString(pgn);

                List<GameSummary> summarylist = new List<GameSummary>();
                foreach(var g in pgnResult.Games)
                {
                    var fen = (from ai in g.AdditionalInfo
                               where string.Equals("fen", ai.Name, StringComparison.OrdinalIgnoreCase)
                               select ai).FirstOrDefault();

                    string fenstring = string.Empty;

                    if (fen != null && !string.IsNullOrWhiteSpace(fen.Value))
                    {
                        fenstring = fen.Value;
                    }

                    summarylist.Add(
                        new GameSummary
                        {
                            White = g.WhitePlayer,
                            Black = g.BlackPlayer,
                            Pgn = g.ToString(),
                            Fenstring = fenstring,
                            Moves = GetMoves(g.ToString())
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
}
