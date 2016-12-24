using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

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
        public IActionResult UploadFile(IFormFile pgnfile)
        {
            return View("ViewPgn", pgnfile);
        }

        private string GetStringFrom(IFormFile file)
        {
            using (MemoryStream s = new MemoryStream())
            {
                file.CopyTo(s);
                s.Flush();
            }

           

            throw new NotImplementedException();
        }
    }
}
