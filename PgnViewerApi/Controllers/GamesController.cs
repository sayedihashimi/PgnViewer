using PgnViewer.Shared;
using PgnViewerApi.Extensions;
using System.Web.Http;

namespace PgnViewerApi.Controllers {

    public class GamesController : ApiController {
        [HttpPost]
        public ChessGame GetChessGame([FromUri]int index, [FromBody]string gamepgn) {
            var chessgame = GameHelper.BuildChessGameFrom(gamepgn, index);

            return chessgame;
        }
    }
}
