using PISS.Models;
using PISS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Controllers
{
    [Authorize(Roles = "Doctorant")]
    public class DoctorantController : Controller
    {
        public ActionResult Index()
        {
            Doctorate model;
            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
            using (var repo = new DoctoratesRepository())
            {
                model = repo.Include("GeneralWorkPlanFile").Where(d => d.DoctorantId == currentUserId).FirstOrDefault();
                if (model == null)
                {
                    model = new Doctorate()
                    {
                        DoctorantId = currentUserId
                    };
                    repo.Add(model);
                    repo.SaveChanges();
                }
            }
            return View(model);
        }

        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files, Doctorate model)
        {
            if (files != null && files.Count() > 0)
            {
                TempData["UploadedFiles"] = GetFileInfo(files).ToList();
                using (var repo = new DoctoratesRepository())
                {
                    repo.UploadFile(files.First(), model, "GeneralWorkPlan");
                    repo.SaveChanges();
                }
            }

            return RedirectToAction("Result");
        }

        public ActionResult Result()
        {
            return View();
        }
        
        private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
        {
            return
                from a in files
                where a != null
                select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
        }
    }
}
