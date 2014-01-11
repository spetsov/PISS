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
    }
}