using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models.Repositories
{
    public class DiplomasRepository : Repository<Diploma>
    {
        public void UploadFile(HttpPostedFileBase file, Diploma diploma, string propertyName)
        {
            if (propertyName != "AssignmentFile" && propertyName != "ReviewFile")
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

                if (propertyName == "AssignmentFile")
                {
                    diploma.АssignmentFile = newFile;
                    diploma.АssignmentFileId = newFile.Id;
                }
                else if (propertyName == "ReviewFile")
                {
                    diploma.ReviewFile = newFile;
                    diploma.ReviewFileId = newFile.Id;
                }

                this.Update(diploma);
            }

        }
    }
}