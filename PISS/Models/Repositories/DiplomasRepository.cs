﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace PISS.Models.Repositories
{
    public class DiplomasRepository : Repository<Diploma>
    {
        public void AddConsultants(string[] userIds, Diploma diploma)
        {
                if (diploma.Consultants == null)
                {
                    diploma.Consultants = new List<Consultant>();
                }
                foreach (var userId in userIds)
                {
                    int userIdParsed = int.Parse(userId);
                    var consultant = diploma.Consultants.Where(c => c.TeacherId == userIdParsed).FirstOrDefault();
                    if (consultant == null)
                    {
                        var user = this.Context.UserProfiles.Where(u => u.UserId == userIdParsed).FirstOrDefault();
                        consultant = new Consultant()
                        {
                            Teacher = user,
                            TeacherId = user.UserId
                        };
                        this.Context.Consultants.Add(consultant);
                        diploma.Consultants.Add(consultant);
                    }
                }

        }

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
            }

        }
    }
}