using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using PgnViewer.Shared;
using System.Net.Http;
using Flurl.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PgnViewerWeb.Controllers
{
    public class PgnViewerController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("BrowseForPgn");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile pgnfile)
        {
            string pgnString = GetStringFrom(pgnfile);
            List<GameSummary> games = await GetGamesFromString(pgnString);
            return View("ViewPgn", games);
        }

        private string GetStringFrom(IFormFile file)
        {
            using (MemoryStream s = new MemoryStream())
            {
                file.CopyTo(s);
                s.Flush();

                s.Position = 0;
                var sr = new StreamReader(s);
                return sr.ReadToEnd();
            }
        }

        private async Task<List<GameSummary>>GetGamesFromString(string pgnString)
        {
            string baseurl = @"http://localhost:20826";
            string url = string.Format("{0}/api/pgn/GetGames", baseurl);
            var resp = await (url.PostJsonAsync(pgnString)).ReceiveJson<List<GameSummary>>();

            return resp;
        }
        
    }
}
