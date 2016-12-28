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
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PgnViewerWeb.Controllers
{
    public class PgnViewerController : Controller
    {
        private IHostingEnvironment _env;
        public PgnViewerController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("BrowseForPgn");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileOld(IFormFile pgnfile)
        {
            string pgnString = GetStringFrom(pgnfile);
            List<GameSummary> games = await GetGamesFromString(pgnString);
            return View("ViewFile", games);
        }

        public async Task<IActionResult> UploadFile(IFormFile pgnfile)
        {
            // get the file name and save it
            string filename = $"{DateTime.Now.ToString("yyyyMMddhhss-fffffff")}.pgn";
            string filepath = System.IO.Path.Combine(_env.WebRootPath, $"pgnfiles\\{filename}");

            await SaveFile(filepath, pgnfile);

            return RedirectToAction("ViewFile", new { id = filename });
        }

        // using id instead of filename to simplify route url
        public async Task<IActionResult> ViewFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            List<GameSummary> games = await GetGamesFromString(GetPgnStringFromFile(id));

            return View("ViewFile", new ViewFileViewModel(id, games));
        }

        public async Task<IActionResult>ViewGameOld(string id, int index)
        {
            string pgnString = GetPgnStringFromFile(id);

            ChessGame game = await GetGamefromString(pgnString, index);

            return View("ViewGame", new ViewGameViewModel(id, index, game));
        }

        public async Task<IActionResult>ViewGame(string filename, int index) {        
            string baseurl = @"http://localhost:20826";
            string url = $"{baseurl}/api/Game?filename={filename}&index={index}";
            var resp = await (url.GetAsync()).ReceiveJson<ChessGame>();



            return View("ViewGame", new ViewGameViewModel(filename, index, resp));
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

        private string GetPgnStringFromFile(string id)
        {
            string filepath = System.IO.Path.Combine(_env.WebRootPath, $"pgnfiles\\{id}");
            // TODO: check to see the file is less than 1 MB before reading
            return System.IO.File.ReadAllText(filepath);
        }

        private async Task<bool> SaveFile(string filepath, IFormFile file)
        {
            using(FileStream fs = new FileStream(filepath, FileMode.CreateNew))
            {
                await file.CopyToAsync(fs);
                fs.Flush();
                return true;
            }
        }

        private async Task<List<GameSummary>>GetGamesFromString(string pgnString)
        {
            string baseurl = @"http://localhost:20826";
            string url = string.Format("{0}/api/pgn/GetGames", baseurl);
            var resp = await (url.PostJsonAsync(pgnString)).ReceiveJson<List<GameSummary>>();

            return resp;
        }
        
        private async Task<T>GetFromWebPostWithBody<T>(string url,string body)
        {
            var resp = await (url.PostJsonAsync(body)).ReceiveJson<T>();

            return resp;
        }

        private async Task<ChessGame>GetGamefromString(string pgnString, int index = 0)
        {
            string baseurl = @"http://localhost:20826";
            string url = $"{baseurl}/api/games?index={index}";

            return await GetFromWebPostWithBody<ChessGame>(url, pgnString);
        }
    }
}
