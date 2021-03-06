﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models.Repositories
{
    public class WorkExperienceRepository : Repository<WorkExperience>
    {   

        public void UploadFile(HttpPostedFileBase file, WorkExperience workExperience, string propertyName)
        {
            if (propertyName != "SuggestionFile" && propertyName != "GradeFile")
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

                if (propertyName == "SuggestionFile")
                {
                    workExperience.SuggestionFile = newFile;
                    workExperience.SuggestionFileId = newFile.Id;
                }
                else if (propertyName == "GradeFile")
                {
                    workExperience.GradeFile = newFile;
                    workExperience.GradeFileId = newFile.Id;
                }
            }

        }
    }
}