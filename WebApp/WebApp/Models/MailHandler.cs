using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebApp.Models
{
    public class MailHandler
    {
        public static bool SendMail(string emailTo, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress("stefanruvceski@gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new NetworkCredential("stefanruvceski@gmail.com", "Terminator_96");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

            return false;
        }
    }
}