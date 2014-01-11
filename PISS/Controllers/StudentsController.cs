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
    [Authorize(Roles = "Student")]
    public class StudentsController : Controller
    {
        public ActionResult Index()
        {
            Diploma model;
            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                model = repo.Include("АssignmentFile").Where(d => d.StudentId == currentUserId).FirstOrDefault();
                if (model == null)
                {
                    model = new Diploma()
                    {
                        StudentId = currentUserId
                    };
                    repo.Add(model);
                    repo.SaveChanges();
                }
            }
            return View(model);
        }

        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files, Diploma model)
        {
            if (files != null && files.Count() > 0)
            {
                TempData["UploadedFiles"] = GetFileInfo(files).ToList();
                using (DiplomasRepository repo = new DiplomasRepository())
                {
                    repo.UploadFile(files.First(), model, "AssignmentFile");
                    repo.Update(model);
                    repo.SaveChanges();
                }                
            }

            return RedirectToAction("Result");
        }

        public ActionResult Result()
        {
            return View();
        }

        public ActionResult Thesis()
        {
            Thesis model;
            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                Diploma diploma = repo.Include("ReviewFile").Include("Thesis")
                    .Include("Thesis.SourceCodeFile").Include("DefenceCommisionMembers").Include("DefenceCommisionMembers.Member")
                    .Include("Consultants").Include("Consultants.Teacher").Include("LeadTeacher").Include("Reviewer").Include("Approver")
                    .Where(d => d.StudentId == currentUserId).FirstOrDefault();
                if (diploma == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (diploma.Thesis != null)
                    {
                        model = diploma.Thesis;
                    }
                    else if (diploma.Approved != ApprovedStatus.Unapproved)
                    {
                        model = new Thesis();
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            return View(model);
        }

        public ActionResult UploadThesis(IEnumerable<HttpPostedFileBase> files, Thesis model)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.Count() > 0)
                {
                    using (ThesisRepository repo = new ThesisRepository())
                    {
                        if (model.Id == 0)
                        {
                            var currentUserId = WebSecurity.GetUserId(User.Identity.Name);
                            repo.Add(model);
                            Diploma diploma = repo.Context.Diplomas.Where(d => d.StudentId == currentUserId).FirstOrDefault();
                            if (diploma != null)
                            {
                                diploma.Thesis = model;
                                diploma.ThesisId = model.Id;
                            }
                            repo.SaveChanges();
                        }
                        repo.UploadFile(files.First(), model);
                        repo.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Thesis");
        }

        public ActionResult DiplomaStatus()
        {
            var studentId = WebSecurity.GetUserId(User.Identity.Name);
            Diploma model;
            using (DiplomasRepository repo = new DiplomasRepository())
            {
                model = repo.Include("Student").Include("АssignmentFile").Include("ReviewFile").Include("Thesis")
                    .Include("Thesis.SourceCodeFile").Include("DefenceCommisionMembers").Include("DefenceCommisionMembers.Member")
                    .Include("Consultants").Include("Consultants.Teacher").Include("LeadTeacher").Include("Reviewer").Include("Approver")
                    .Where(d => d.StudentId == studentId).FirstOrDefault();

            }

            return View(model);
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
