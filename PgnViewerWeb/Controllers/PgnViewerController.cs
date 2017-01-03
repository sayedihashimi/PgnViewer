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

        private string GetApiBaseAddress() {
            // if(_env.IsDevelopment()
            string apiAddress = System.Environment.GetEnvironmentVariable("ApiBaseAddress");
            if (string.IsNullOrWhiteSpace(apiAddress)) {
                apiAddress = @"http://localhost:20826";
            }

            return apiAddress;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            // get the list of file names
            string url = $"{GetApiBaseAddress()}/api/PgnFile";
            var response = await url.GetJsonAsync<List<string>>();


            return View("BrowseForPgn", response);
        }

        public async Task<IActionResult>UploadFile(IFormFile pgnFile) {
            // save the uploaded file in a temp file for now
            string tempfile = System.IO.Path.GetTempFileName();
            if (System.IO.File.Exists(tempfile)) {
                System.IO.File.Delete(tempfile);
            }

            await SaveFile(tempfile, pgnFile);
            string pgncontent = System.IO.File.ReadAllText(tempfile);

            
            string url = $"{GetApiBaseAddress()}/api/PgnFile?filename={Path.GetFileName(pgnFile.FileName)}";
            var response = await url.PostJsonAsync(pgncontent);
            string filename = null;
            
            if (response.IsSuccessStatusCode) {
                // get the location header out of the
                Uri newfileurl = response.Headers.Location;
                
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(newfileurl.Query);
                if (queryDictionary.ContainsKey("filename")) {
                    filename = queryDictionary["filename"];
                }
            }

            if (System.IO.File.Exists(tempfile)) {
                System.IO.File.Delete(tempfile);
            }

            return RedirectToAction("ViewFile", new { filename = filename });
        }
        public async Task<IActionResult>ViewFile(string filename) {
            // get the game from the file
            string url = $"{GetApiBaseAddress()}/api/PgnFile?filename={filename}";
            var response = await url.GetJsonAsync<List<GameSummaryInfo>>();

            return View("ViewFile", new ViewFileViewModel(filename, response));
        }

        public async Task<IActionResult>ViewGame(string filename, int index) {
            string url = $"{GetApiBaseAddress()}/api/Game?filename={filename}&index={index}";
            var resp = await (url.GetAsync()).ReceiveJson<ChessGame>();

            // http://localhost:20826/api/PgnFile?getNumGames=true&filename=201701010946-4037908.pgn.json
            string numGamesUrl = $"{GetApiBaseAddress()}/api/PgnFile?getNumGames=true&filename={filename}";
            var numGamesStr = await numGamesUrl.GetAsync().ReceiveString();
            int numGames = int.Parse(numGamesStr);

            return View("ViewGame", new ViewGameViewModel(filename, index, numGames, resp));
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
            string url = string.Format("{0}/api/pgn/GetGames", GetApiBaseAddress());
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
            string url = $"{GetApiBaseAddress()}/api/games?index={index}";

            return await GetFromWebPostWithBody<ChessGame>(url, pgnString);
        }
    }
}
