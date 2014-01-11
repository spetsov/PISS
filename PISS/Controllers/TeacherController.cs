using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using PISS.Models;
using PISS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

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
                    .Include("Thesis.SourceCodeFile").Include("DefenceCommisionMembers").Include("DefenceCommisionMembers.Member")
                    .Include("Consultants").Include("Consultants.Teacher").Include("LeadTeacher").Include("Reviewer").Include("Approver")
                    .Where(d => d.StudentId == studentId).FirstOrDefault();

                model.SelectedConsultantsUserIds = new string[model.Diploma.Consultants.Count];
                var consultantsArray = model.Diploma.Consultants.ToArray();
                for (int i = 0; i < model.SelectedConsultantsUserIds.Length; i++)
                {
                    model.SelectedConsultantsUserIds[i] = consultantsArray[i].TeacherId.ToString();
                }

                model.SelectedDefenceCommisionMembersUserIds = new string[model.Diploma.DefenceCommisionMembers.Count];
                var defenceCommisionMembersArray = model.Diploma.DefenceCommisionMembers.ToArray();
                for (int i = 0; i < model.SelectedDefenceCommisionMembersUserIds.Length; i++)
                {
                    model.SelectedDefenceCommisionMembersUserIds[i] = defenceCommisionMembersArray[i].MemberId.ToString();
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
            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                var diploma = repo.GetSet().Find(incomingDiploma.Diploma.Id);
                if (!string.IsNullOrEmpty(incomingDiploma.Diploma.ReviewNotes))
                {
                    diploma.ReviewNotes = incomingDiploma.Diploma.ReviewNotes;
                    diploma.Approved = ApprovedStatus.ConditionallyApproved;
                    diploma.ApproverId = currentUserId;
                    diploma.Approver = repo.Context.UserProfiles.Find(currentUserId);
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
            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                var diploma = repo.Include("Consultants").Include("DefenceCommisionMembers").FirstOrDefault(i => i.Id == incomingDiploma.Diploma.Id);

                diploma.LeadTeacherId = incomingDiploma.Diploma.LeadTeacherId;
                diploma.LeadTeacher = repo.Context.UserProfiles.Find(incomingDiploma.Diploma.LeadTeacherId);
                diploma.DefenceDate = incomingDiploma.Diploma.DefenceDate;
                diploma.Grade = incomingDiploma.Diploma.Grade;
                if (files != null && files.Count() > 0)
                {
                    repo.UploadFile(files.First(), diploma, "ReviewFile");
                    diploma.ReviewerId = currentUserId;
                    diploma.Reviewer = repo.Context.UserProfiles.Find(currentUserId);
                }

                if (incomingDiploma.SelectedConsultantsUserIds != null 
                    && incomingDiploma.SelectedConsultantsUserIds.Length > 0)
                {
                    repo.AddConsultants(incomingDiploma.SelectedConsultantsUserIds, diploma);
                }

                if (incomingDiploma.SelectedDefenceCommisionMembersUserIds != null
                    && incomingDiploma.SelectedDefenceCommisionMembersUserIds.Length > 0)
                {
                    repo.AddDefenceMembers(incomingDiploma.SelectedDefenceCommisionMembersUserIds, diploma);
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
