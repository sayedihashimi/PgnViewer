using ilf.pgn;
using ilf.pgn.Data;
using Newtonsoft.Json;
using PgnViewerApi.Models;
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

                    summarylist.Add(
                        new GameSummary
                        {
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
}
