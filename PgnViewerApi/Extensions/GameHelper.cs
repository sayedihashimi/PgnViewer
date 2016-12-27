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
                Pgn = game.ToString()
            };

            bool firstMove = true;
            int totalNumHalfMoves = game.MoveText.GetMoves().Count();
            int currentMoveNumber = 1;
            var gameMoves = game.MoveText.GetMoves().ToList();

            for (int currentHalfMoveIndex = 0; currentHalfMoveIndex < gameMoves.Count(); currentHalfMoveIndex++) {
                Halfmove whiteMove = Halfmove.NullMove;
                Halfmove blackMove = Halfmove.NullMove;

                if (firstMove && !isWhiteMove) {
                    // create a move with a null move for white
                    blackMove = new Halfmove(gameMoves[currentHalfMoveIndex].ToString());

                }
                else {
                    whiteMove = new Halfmove(gameMoves[currentHalfMoveIndex].ToString());
                    if(gameMoves.Count() > (currentHalfMoveIndex + 1)) {
                        blackMove = new Halfmove(gameMoves[currentHalfMoveIndex + 1].ToString());
                    }

                    // increment halfMoveCounter an extra time for the blackMove
                    currentHalfMoveIndex++;
                }

                result.Moves.Add(new ChessMove {
                    MoveNumber = currentMoveNumber++,
                    WhiteMove = whiteMove,
                    BlackMove = blackMove
                });

            }
            
            return result;
        }

        public static Game GetSingleGameFrom(string pgn, int index = 0) {
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