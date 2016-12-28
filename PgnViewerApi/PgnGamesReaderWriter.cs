﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PgnViewerApi {

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

        // TODO: This method can be implemented in GetPgnGamesFromString to optimize perf
        public string GetGameFromFile(string filepath, int indexToGet) {
            if (string.IsNullOrEmpty(filepath)) { throw new ArgumentNullException(nameof(filepath)); }
            if (!File.Exists(filepath)) {
                throw new FileNotFoundException("File not found", filepath);
            }

            string jsonString = GetStringFromFile(filepath);
            List<string> allgames = JsonConvert.DeserializeObject<List<string>>(jsonString);

            if(indexToGet <0||indexToGet > allgames.Count) {
                throw new ArgumentException($"Index [{indexToGet} is out of bounds]");
            }

            return allgames[indexToGet];
        }

        public Stream GenerateStreamFromString(string s) {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}