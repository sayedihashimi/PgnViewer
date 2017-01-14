using PgnViewer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgnViewerWeb
{
    public class ViewFileViewModel {
        public ViewFileViewModel() : this(null,null, null) {

        }
        public ViewFileViewModel(string filename, string downloadUrl, List<GameSummaryInfo> games) {
            Filename = filename;
            Games = games;
            DownloadUrl = downloadUrl;
        }

        public string Filename { get; set; }
        public string DownloadUrl { get; set; }
        public List<GameSummaryInfo> Games { get; set; }
    }    
}
