using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

    public class PgnGamesReaderWriter {
        public string GetStringFromFile(string filepath) {
            if (!File.Exists(filepath)) {
                throw new FileNotFoundException("File not found", filepath);
            }
            // TODO: check to see the file is less than 1 MB before reading
            return System.IO.File.ReadAllText(filepath);
        }

        public void SavePgnAsJsonFile(string filepath, List<string> games) {
            if (File.Exists(filepath)) {
                throw new ArgumentException($"Cannot write file because file already exists [{filepath}]");
            }

            string jsonstring = JsonConvert.SerializeObject(games);
            File.WriteAllText(filepath, jsonstring);
        }

        public List<string> GetPgnGamesFromString(string pgncontent) {
            if (string.IsNullOrEmpty(pgncontent)) { throw new ArgumentNullException(nameof(pgncontent)); }

            List<string> allgames = new List<string>();
            StringBuilder currentPgnString = new StringBuilder();

            string currentLine = null;
            string previousLine = null;
            bool currentLineIsTag = false;
            // start this with true so as to not reset to a new game
            bool previousLineIsTag = true;
            
            using (var stream = GenerateStreamFromString(pgncontent))
            using (var streamReader = new StreamReader(stream)) {
                // append to currentPgnString unless a new start of game is detected
                // start of game is detected when
                //      1. Last line was not a tag
                //      2. Current line is a tag
                Regex tagRegex = new Regex(@"\[.*");
                while ( (currentLine=streamReader.ReadLine()) != null) {
                    currentLineIsTag = tagRegex.IsMatch(currentLine);
                    if (!previousLineIsTag && currentLineIsTag) {                        
                        // Game has ended
                        string currentGame = currentPgnString.ToString();
                        if (!string.IsNullOrWhiteSpace(currentGame)) {
                            allgames.Add(currentGame);
                        }

                        // reset to a new game
                        currentPgnString = new StringBuilder();
                    }

                    // write out the current line to the stringbuilder
                    currentPgnString.AppendLine(currentLine);

                    // update currentLine and previousLine as the last thing in the loop
                    previousLine = currentLine;
                    previousLineIsTag = currentLineIsTag;
                    currentLine = null;
                    currentLineIsTag = false;
                }
            }

            string lastGame = currentPgnString.ToString();
            if (!string.IsNullOrWhiteSpace(lastGame)) {
                allgames.Add(lastGame);
            }

            return allgames;
        }

        private Stream GenerateStreamFromString(string s) {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
