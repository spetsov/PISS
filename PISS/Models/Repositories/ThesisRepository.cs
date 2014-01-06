using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISS.Models.Repositories
{
    public class ThesisRepository : Repository<Thesis>
    {
        public void UploadFile(HttpPostedFileBase file, Thesis thesis)
        {
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

                thesis.SourceCodeFile = newFile;
                thesis.SourceCodeFileId = newFile.Id;

                this.Update(thesis);
            }

        }
    }
}