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
                model = repo.Include("Student").Include("АssignmentFile").Include("Thesis").Include("Thesis.SourceCodeFile").Where(d => d.StudentId == studentId).FirstOrDefault();
            }

            return View(new DiplomaTeacherViewModel()
            {
                Diploma = model
            });
        }

        public ActionResult ApproveAssignment(Diploma incomingDiploma, string notes)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                if (!string.IsNullOrEmpty(notes))
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

        private IEnumerable<Diploma> GetAllDiplomas(int pageIndex, int pageSize)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                return repo.Include("Student").OrderByDescending(d => d.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

    }
}
