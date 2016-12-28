using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PgnViewerApi.Controllers {
    public class PgnFileController : ApiController {
        private string PgnFolder { get; } = System.Web.Hosting.HostingEnvironment.MapPath($"~/pgnfiles");
        public List<string> GetFileNames() {
            string[]files = Directory.GetFiles(PgnFolder, "*.pgn.json", SearchOption.TopDirectoryOnly);
            List<string> filenames = new List<string>();
            foreach(var f in files) {
                filenames.Add(new FileInfo(f).Name);
            }

            return filenames;
        }
        public List<string>GetGamesInFile(string filename, int indexToGet = -1) {
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException("filename"); }
            filename = filename.Trim();            

            string filepath = $"{PgnFolder}/{filename}";
            if (!File.Exists(filepath)) {
                throw new FileNotFoundException("File not found", filepath);
            }

            var destFileInfo = new FileInfo(filepath);
            if (!destFileInfo.Name.ToLowerInvariant().EndsWith(".pgn.json")) {
                throw new ArgumentException($"Cannot process file with extension {destFileInfo.Extension}");
            }

            PgnGamesReaderWriter gamesRw = new PgnGamesReaderWriter();
            List<string> pgngames = JsonConvert.DeserializeObject<List<string>>(gamesRw.GetStringFromFile(filepath));

            if(indexToGet >= 0) {
                return new List<string> {
                    pgngames[indexToGet]
                };
            }

            return pgngames;
        }
        [HttpPost]
        public HttpResponseMessage UploadFile([FromBody]string pgnfilecontents) {
            if (string.IsNullOrEmpty(pgnfilecontents)) { throw new ArgumentNullException(nameof(pgnfilecontents)); }

            var gamesRw = new PgnGamesReaderWriter();
            List<string> pgngames = gamesRw.GetPgnGamesFromString(pgnfilecontents);

            // save the games to a .pgn.json file
            string filename = $"{DateTime.Now.ToString("yyyyMMddhhss-fffffff")}.pgn.json";
            string filepath = System.Web.Hosting.HostingEnvironment.MapPath($"~/pgnfiles/{filename}");

            gamesRw.SavePgnAsJsonFile(filepath, pgngames);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, "value");
            response.Headers.Location = new Uri($"{Request.RequestUri}?filename={filename}");
            return response;
        }
    }
}
