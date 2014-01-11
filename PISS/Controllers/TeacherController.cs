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
            DiplomaViewModel model = new DiplomaViewModel();
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                model.Diploma = repo.Include("Student").Include("АssignmentFile").Include("ReviewFile").Include("Thesis")
                    .Include("Thesis.SourceCodeFile").Include("DefenceCommision").Include("DefenceCommision.Members")
                    .Include("DefenceCommision.Members.Member").Include("Consultants").Include("Consultants.Teacher")
                    .Include("LeadTeacher")
                    .Where(d => d.StudentId == studentId).FirstOrDefault();
                model.SelectedUserIds = new string[model.Diploma.Consultants.Count];
                var consultantsArray = model.Diploma.Consultants.ToArray();
                for (int i = 0; i < model.SelectedUserIds.Length; i++)
                {
                    model.SelectedUserIds[i] = consultantsArray[i].TeacherId.ToString();
                }
            }

            using (UserProfilesRepository repo = new UserProfilesRepository())
            {

                var teachers = repo.GetAllMembershipUsersForRole("Teacher").ToList();
                ViewBag.Users = teachers;
            }

            return View(model);
        }

        public ActionResult ApproveAssignment(DiplomaViewModel incomingDiploma)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                var diploma = repo.GetSet().Find(incomingDiploma.Diploma.Id);
                if (!string.IsNullOrEmpty(incomingDiploma.Diploma.ReviewNotes))
                {
                    diploma.Approved = ApprovedStatus.ConditionallyApproved;
                }
                else
                {
                    diploma.Approved = ApprovedStatus.Approved;
                }
                repo.Update(diploma);
                repo.SaveChanges();

                return RedirectToAction("Diploma", new { studentId = diploma.StudentId });
                // TODO: Send Email
            }           
            
        }

        public ActionResult GetAll([DataSourceRequest]DataSourceRequest request)
        {
            var diplomas = this.GetAllDiplomas(request.Page, request.PageSize);
            return Json(diplomas.ToDataSourceResult(request));
        }

        public ActionResult UpdateDiploma(IEnumerable<HttpPostedFileBase> files, DiplomaViewModel incomingDiploma)
        {
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                var diploma = repo.Include("Consultants").FirstOrDefault(i => i.Id == incomingDiploma.Diploma.Id);

                diploma.LeadTeacherId = incomingDiploma.Diploma.LeadTeacherId;
                diploma.LeadTeacher = repo.Context.UserProfiles.Find(incomingDiploma.Diploma.LeadTeacherId);
                diploma.DefenceDate = incomingDiploma.Diploma.DefenceDate;
                diploma.Grade = incomingDiploma.Diploma.Grade;
                if (files != null && files.Count() > 0)
                {
                    repo.UploadFile(files.First(), diploma, "ReviewFile");
                }

                if (incomingDiploma.SelectedUserIds != null && incomingDiploma.SelectedUserIds.Length > 0)
                {
                    repo.AddConsultants(incomingDiploma.SelectedUserIds, diploma);
                }

                repo.Update(diploma);
                repo.SaveChanges();
                return RedirectToAction("Diploma", new { studentId = diploma.StudentId });
            }      
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
