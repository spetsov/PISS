using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using PISS.Models.Repositories;
using PISS.Models;

namespace PISS.Areas.Administration.Controllers
{
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll([DataSourceRequest]DataSourceRequest request)
        {
            if (request.PageSize == 0)
            {
                request.PageSize = 10;
            }
            var result = Json(this.GetMembershipUsers(request.Page, request.PageSize).ToDataSourceResult(request));
            return result;
        }

        public ActionResult ApproveUser(string email)
        {
            using (UserProfilesRepository repo = new UserProfilesRepository())
            {
                repo.ApproveUser(email);
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteUser([DataSourceRequest] DataSourceRequest request, UserViewModel user)
        {
            if (user != null)
            {
                using (UserProfilesRepository repo = new UserProfilesRepository())
                {
                    repo.DeleteUser(user.Email);
                    var profile = repo.GetQuery().Where(u => u.UserId == user.UserId).FirstOrDefault();
                    if (profile != null)
                    {
                        repo.Delete(profile);
                    }
                    repo.SaveChanges();
                }
            }

            return Json(new[] { user }.ToDataSourceResult(request, ModelState));
        }

        private IList<UserViewModel> GetMembershipUsers(int pageIndex, int pageSize)
        {
            using (UserProfilesRepository repo = new UserProfilesRepository())
            {
                var users = repo.GetAllMembershipUsers((pageIndex - 1) * pageSize, pageSize);
                return users.ToList();
            }
        }
    }
}
