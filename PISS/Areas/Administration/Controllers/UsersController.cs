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
            return Json(this.GetMembershipUsers(request.Page, request.PageSize).ToDataSourceResult(request));
        }

        public ActionResult ApproveUser(string email)
        {
            using (UserProfilesRepository repo = new UserProfilesRepository())
            {
                repo.ApproveUser(email);
            }
            return View("Index");
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
