using PgnViewer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgnViewerWeb
{
    public class ViewFileViewModel {
        public ViewFileViewModel() : this(null, null) {

        }
        public ViewFileViewModel(string filename, List<string> games) {
            Filename = filename;
            Games = games;
        }
        public string Filename { get; set; }
        public List<string> Games { get; set; }
    }
    public class ViewFileViewModelOld
    {
        public ViewFileViewModelOld() : this(null, null) {

        }
        public ViewFileViewModelOld(string filename, List<GameSummary> games) {
            Filename = filename;
            Games = games;
        }
        public string Filename { get; set; }
        public List<GameSummary> Games { get; set; }
    }
}
