using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using PISS.Models;

namespace PISS
{
    public class SystemInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new SystemDatabaseInitializer());

            try
            {
                using (var context = new SystemContext())
                {
                    context.Database.Initialize(false);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }
        }
    }
}
