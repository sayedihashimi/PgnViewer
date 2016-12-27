using PgnViewer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PgnViewerWeb
{
    public class ViewFileViewModel
    {
        public ViewFileViewModel() : this(null, null) {

        }
        public ViewFileViewModel(string id, List<GameSummary> games) {
            Id = id;
            Games = games;
        }
        public string Id { get; set; }
        public List<GameSummary> Games { get; set; }
    }
}
