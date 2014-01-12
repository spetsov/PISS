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
                model = repo.Include("GeneralWorkPlanFile").Include("PersonalWorkPlanFile").Include("YearByYearPlanFile").
                    Where(d => d.DoctorantId == currentUserId).FirstOrDefault();
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

        public ActionResult DoctorateStatus()
        {
            var doctorantId = WebSecurity.GetUserId(User.Identity.Name);
            Doctorate model;
            using (var repo = new DoctoratesRepository())
            {
                model = repo.Include("AttestationFile")
                            .Include("Consultants").Include("Consultants.Teacher")
                            .Include("LeadTeachers").Include("LeadTeachers.Teacher")
                            .Include("Reviewer")
                            .Where(d => d.DoctorantId == doctorantId).FirstOrDefault();

                if (model == null)
                {
                    var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
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

        public ActionResult UploadYearByYearWorkPlan(IEnumerable<HttpPostedFileBase> yearByYearPlanFiles, Doctorate model)
        {
            return this.UploadFiles(yearByYearPlanFiles, model, "YearByYearPlan");
        }


        public ActionResult UploadGeneralPlan(IEnumerable<HttpPostedFileBase> generalPlanFiles, Doctorate model)
        {
            return this.UploadFiles(generalPlanFiles, model, "GeneralWorkPlan");
        }

        public ActionResult UploadPersonalPlan(IEnumerable<HttpPostedFileBase> personalPlanFiles, Doctorate model)
        {
            return this.UploadFiles(personalPlanFiles, model, "PersonalWorkPlan");
        }

        public ActionResult Result()
        {
            return View();
        }

        private ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files, Doctorate model, string fileName)
        {
            if (files != null && files.Count() > 0)
            {
                TempData["UploadedFiles"] = GetFileInfo(files).ToList();
                using (var repo = new DoctoratesRepository())
                {
                    repo.UploadFile(files.First(), model, fileName);
                    repo.SaveChanges();
                }
            }

            return RedirectToAction("Result");
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
