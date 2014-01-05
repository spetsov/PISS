using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models.Repositories
{
    public class UserProfilesRepository : Repository<UserProfile>
    {
        public IEnumerable<UserViewModel> GetAllMembershipUsers(int offset, int pageSize)
        {
            string query = "SELECT * FROM webpages_Membership as u JOIN UserProfile as up ON up.UserId = u.UserId order by CreateDate desc OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";

            var users = this.Context.Database.SqlQuery<UserViewModel>(query, offset, pageSize);
            return users;
        }

        public void ApproveUser(string email)
        {
                string query = "select ConfirmationToken from webpages_Membership where UserId = (select UserId from UserProfile where Email = '{0}')";

                string token = this.Context.Database.SqlQuery<string>(query, email).FirstOrDefault();
                WebSecurity.ConfirmAccount(token);
        }    
    }
}