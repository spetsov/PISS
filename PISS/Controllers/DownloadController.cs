using PISS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PISS.Controllers
{
    public class DownloadController : Controller
    {

        public ActionResult Index(int id)
        {
            using (FilesRepository repo = new FilesRepository())
            {
                var file = repo.GetQuery().Where(f => f.Id == id).First();
                return File(file.Content, file.MimeType, file.FileName);
            }          
        }

    }
}
