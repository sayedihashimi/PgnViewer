using Newtonsoft.Json;
using PgnViewer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgnViewerWeb {
    public class ViewGameViewModel {
        public ViewGameViewModel() : this(null, -1, null) {

        }
        public ViewGameViewModel(string filename, int index, ChessGame game) {
            Filename = filename;
            Index = index;
            Game = game;
            InitMovesAsJson();
        }
        public string Filename { get; set; }
        public int Index { get; set; }
        public ChessGame Game { get; set; }

        public string MovesAsJson { get; set; }

        private void InitMovesAsJson() {
            if (Game != null && Game.Moves != null) {
                MovesAsJson = JsonConvert.SerializeObject(Game.Moves);
            }
        }

    }
}
