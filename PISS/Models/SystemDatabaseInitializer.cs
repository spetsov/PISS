using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models
{
    public class SystemDatabaseInitializer : IDatabaseInitializer<SystemContext>
    {
        public void InitializeDatabase(SystemContext context)
        {
            context.Database.CreateIfNotExists();

            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "Email", autoCreateTables: true);

            this.Seed(context);
        }

        protected void Seed(SystemContext context)
        {
            var roles = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;

            if (!roles.RoleExists("Admin"))
                roles.CreateRole("Admin");

            if (!roles.RoleExists("Teacher"))
                roles.CreateRole("Teacher");

            if (!roles.RoleExists("Student"))
                roles.CreateRole("Student");

            if (!roles.RoleExists("Doctorant"))
                roles.CreateRole("Doctorant");

            if (!WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "admin@2", false);
                roles.AddUsersToRoles(new string[] { "admin" }, new string[] { "Admin" });
            }
        }
    }
}