﻿using PISS.Models;
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
                model = repo.GetQuery("АssignmentFile").Where(d => d.StudentId == currentUserId).FirstOrDefault();
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
                Diploma diploma = repo.GetQuery("Thesis", "Thesis.SourceCodeFile").Where(d => d.StudentId == currentUserId).FirstOrDefault();
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
                        using (ThesisRepository thesisRepo = new ThesisRepository())
                        {
                            model = thesisRepo.Add(new Thesis());
                            thesisRepo.SaveChanges();
                            diploma.Thesis = model;
                            diploma.ThesisId = model.Id;
                            repo.Update(diploma);
                            repo.SaveChanges();
                        }
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
            if (files != null && files.Count() > 0)
            {
                using (ThesisRepository repo = new ThesisRepository())
                {
                    repo.UploadFile(files.First(), model);
                    repo.SaveChanges();
                }
            }

            return RedirectToAction("Thesis");
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
