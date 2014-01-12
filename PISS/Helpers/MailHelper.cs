using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PISS.Helpers
{
    public class MailHelper
    {
        public static void SendMail(string receiver, string subject, string body)
        {
            try
            {
                SmtpClient client = new SmtpClient("localhost", 25);
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("studentSystem@fmi.bg");
                mailMessage.To.Add(receiver);
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                //Log
            }
        }
    }
}