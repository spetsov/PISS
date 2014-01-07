using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using PISS.Models;
using PISS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PISS.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Diploma(int studentId)
        {
            Diploma model;
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                model = repo.Include("Student").Include("АssignmentFile").Include("ReviewFile").Include("Thesis")
                    .Include("Thesis.SourceCodeFile").Where(d => d.StudentId == studentId).FirstOrDefault();
            }

            return View(model);
        }

        public ActionResult ApproveAssignment(Diploma incomingDiploma)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                if (!string.IsNullOrEmpty(incomingDiploma.ReviewNotes))
                {
                    incomingDiploma.Approved = ApprovedStatus.ConditionallyApproved;
                }
                else
                {
                    incomingDiploma.Approved = ApprovedStatus.Approved;
                }
                repo.Update(incomingDiploma);
                repo.SaveChanges();
            }

            // TODO: Send Email

            return RedirectToAction("Diploma", new { studentId = incomingDiploma.StudentId });
        }

        public ActionResult GetAll([DataSourceRequest]DataSourceRequest request)
        {
            var diplomas = this.GetAllDiplomas(request.Page, request.PageSize);
            return Json(diplomas.ToDataSourceResult(request));
        }

        public ActionResult UpdateDiploma(IEnumerable<HttpPostedFileBase> files, Diploma incomingDiploma)
        {
            if (files != null && files.Count() > 0)
            {
                using (DiplomasRepository repo = new DiplomasRepository())
                {
                    repo.UploadFile(files.First(), incomingDiploma, "ReviewFile");
                    repo.SaveChanges();
                }
            }
            else
            {
                using (DiplomasRepository repo = new DiplomasRepository())
                {
                    repo.Update(incomingDiploma);
                    repo.SaveChanges();
                }
            }

            return RedirectToAction("Diploma", new { studentId = incomingDiploma.StudentId });
        }

        private IEnumerable<Diploma> GetAllDiplomas(int pageIndex, int pageSize)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                return repo.Include("Student").OrderByDescending(d => d.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

    }
}
