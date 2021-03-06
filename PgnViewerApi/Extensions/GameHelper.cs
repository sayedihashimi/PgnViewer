﻿using ilf.pgn;
using ilf.pgn.Data;
using PgnViewer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PgnViewerApi.Extensions
{
    public static class GameHelper
    {
        public static ChessGame BuildChessGameFrom(string pgn, int index = 0)
        {
            if (string.IsNullOrWhiteSpace(pgn)) {
                throw new ArgumentNullException("pgn");
            }

            Game game = GetSingleGameFrom(pgn, index);
            bool isWhiteMove = true;
            if(game.BoardSetup != null) {
                isWhiteMove = game.BoardSetup.IsWhiteMove;
            }

            var result = new ChessGame {
                White = game.WhitePlayer,
                Black = game.BlackPlayer,
                Fen = GetFromAdditionalInfo(game, "Fen"),
                Event = game.Event,
                Pgn = game.ToString(),
                FirstPlayer = isWhiteMove ? "white" : "black"
            };

            switch (game.Result) {
                case GameResult.White:
                    result.Result = "0-1";
                    break;
                case GameResult.Black:
                    result.Result = "1-0";
                    break;
                case GameResult.Draw:
                    result.Result = "1/2-1/2";
                    break;
                case GameResult.Open:
                default:
                    result.Result = string.Empty;
                    break;
            }

            result.Moves = new List<ChessHalfmove>();
            if (game.MoveText != null && game.MoveText.GetMoves().Count() > 0) {
                int moveIndex = 1;
                foreach (var move in game.MoveText.GetMoves()) {                    
                    result.Moves.Add(new ChessHalfmove(moveIndex++, move.ToString()));
                }
            }
            

            //bool firstMove = true;
            //int totalNumHalfMoves = game.MoveText.GetMoves().Count();
            //int currentMoveNumber = 1;
            //var gameMoves = game.MoveText.GetMoves().ToList();

            //for (int currentHalfMoveIndex = 0; currentHalfMoveIndex < gameMoves.Count(); currentHalfMoveIndex++) {
            //    string whiteMove = null;
            //    string blackMove = null;

            //    if (firstMove && !isWhiteMove) {
            //        // create a move with a null move for white
            //        blackMove = gameMoves[currentHalfMoveIndex].ToString();
            //    }
            //    else {
            //        whiteMove = gameMoves[currentHalfMoveIndex].ToString();
            //        if (gameMoves.Count() > (currentHalfMoveIndex + 1)) {
            //            blackMove = gameMoves[currentHalfMoveIndex + 1].ToString();
            //        }

            //        // increment halfMoveCounter an extra time for the blackMove
            //        currentHalfMoveIndex++;
            //    }

            //    result.Moves.Add(new ChessMove {
            //        Id = currentMoveNumber++,
            //        White = whiteMove,
            //        Black = blackMove
            //    });
            //}
            
            return result;
        }

        public static GameSummaryInfo BuildGameSummaryInfoFrom(Game game) {
            if (game == null) { throw new ArgumentNullException(nameof(game)); }

            GameSummaryInfo info = new GameSummaryInfo();
            info.White = game.WhitePlayer;
            info.Black = game.BlackPlayer;
            info.Event = game.Event;
            info.Year = game.Year.HasValue ? game.Year.ToString() : null;
            info.Month = game.Month.HasValue ? game.Month.ToString() : null;
            info.Day = game.Day.HasValue ? game.Day.ToString() : null;
            
            switch (game.Result) {
                case GameResult.White:
                    info.Result = "1-0";
                    break;
                case GameResult.Black:
                    info.Result = "0-1";
                    break;
                case GameResult.Draw:
                    info.Result = "1/2-1/2";
                    break;
                case GameResult.Open:
                default:
                    info.Result = null;
                    break;
            }

            return info;
        }

        public static Game GetSingleGameFrom(string pgn, int index = 0,bool verifOnlyOneGame=false) {
            if (pgn == null) { throw new ArgumentNullException(nameof(pgn)); }

            if (index < 0) {
                throw new ArgumentException($"Invalid value for index [{index}]");
            }

            PgnReader reader = new PgnReader();
            Database pgnResult = reader.ReadFromString(pgn);
            Game game = null;
            List<MoveSummary> moves = new List<MoveSummary>();
            if (pgnResult != null || pgnResult.Games != null && index > (pgnResult.Games.Count() - 1)) {
                game = pgnResult.Games[index];
            }
            if (game == null) {
                throw new ApplicationException("Game not found");
            }

            if(verifOnlyOneGame == true && pgnResult.Games.Count() != 1) {
                throw new ApplicationException("Found more than one game, when only one was expected");
            }

            return game;
        }

        public static string GetFromAdditionalInfo(Game game, string name) {
            string result = null;
            var qresult = (from ai in game.AdditionalInfo
                       where string.Equals(name, ai.Name, StringComparison.OrdinalIgnoreCase)
                       select ai).FirstOrDefault();


            if (qresult != null && !string.IsNullOrWhiteSpace(qresult.Value)) {
                result = qresult.Value;
            }

            return result;
        }

    }
}