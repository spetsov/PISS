using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models.Repositories
{
    public class DoctoratesRepository : Repository<Doctorate>
    {
        public void UploadFile(HttpPostedFileBase file, Doctorate doctorate, string propertyName)
        {
            if (propertyName != "GeneralWorkPlan" && propertyName != "PersonalWorkPlan")
            {
                return;
            }
            using (FilesRepository repo = new FilesRepository())
            {
                var newFile = repo.Add(new File()
                {
                    Content = new byte[file.ContentLength],
                    MimeType = file.ContentType,
                    FileName = file.FileName
                });

                int numBytesToRead = file.ContentLength;
                int numBytesRead = 0;
                do
                {
                    int n = file.InputStream.Read(newFile.Content, numBytesRead, file.ContentLength);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);
                file.InputStream.Close();

                repo.SaveChanges();

                if (propertyName == "GeneralWorkPlan")
                {
                    doctorate.GeneralWorkPlanFile = newFile;
                    doctorate.GeneralWorkPlanFileId = newFile.Id;
                }
                if (propertyName == "PersonalWorkPlan")
                {
                    doctorate.PersonalWorkPlanFile = newFile;
                    doctorate.PersonalWorkPlanFileId = newFile.Id;
                }

                this.Update(doctorate);
            }
        }

        public void AddLeadTeachers(string[] userIds, Doctorate doctorate)
        {
            if (doctorate.LeadTeachers == null)
            {
                doctorate.LeadTeachers = new List<LeadTeacher>();
            }
            doctorate.LeadTeachers.Clear();
            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    int userIdParsed = int.Parse(userId);
                    var leadTeacher = doctorate.LeadTeachers.Where(c => c.TeacherId == userIdParsed).FirstOrDefault();
                    if (leadTeacher == null)
                    {
                        var user = this.Context.UserProfiles.Where(u => u.UserId == userIdParsed).FirstOrDefault();
                        leadTeacher = new LeadTeacher()
                        {
                            Teacher = user,
                            TeacherId = user.UserId
                        };
                        this.Context.LeadTeachers.Add(leadTeacher);
                        doctorate.LeadTeachers.Add(leadTeacher);
                    }
                }
            }
        }

        public void AddConsultants(string[] userIds, Doctorate doctorate)
        {
            if (doctorate.Consultants == null)
            {
                doctorate.Consultants = new List<Consultant>();
            }
            doctorate.Consultants.Clear();

            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    int userIdParsed = int.Parse(userId);
                    var consultant = doctorate.Consultants.Where(c => c.TeacherId == userIdParsed).FirstOrDefault();
                    if (consultant == null)
                    {
                        var user = this.Context.UserProfiles.Where(u => u.UserId == userIdParsed).FirstOrDefault();
                        consultant = new Consultant()
                        {
                            Teacher = user,
                            TeacherId = user.UserId
                        };
                        this.Context.Consultants.Add(consultant);
                        doctorate.Consultants.Add(consultant);
                    }
                }
            }
        }
    }
}