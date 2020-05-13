using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace ecare.Models
{
    public class eCareSendEmail
    {
        public bool SendEmail(string ToAddress, string name, string generatedPassword, string fileName)
        {

            bool isSuccess = false;

            string mailBody = File.ReadAllText(fileName);

            SmtpSection smtpObj = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            using (MailMessage mm = new MailMessage())
            {

                mm.From = new MailAddress(smtpObj.From); // Email address of the sender
                mm.To.Add(ToAddress);      // Email address of the recipient.
                mm.Subject = "Credentails for logging into the ECare System";
                mailBody = mailBody.Replace("{Name}", name);
                mailBody = mailBody.Replace("{email}", ToAddress);
                mailBody = mailBody.Replace("{password}", generatedPassword);
                mm.Body = mailBody;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpObj.Network.Host;
                smtp.EnableSsl = smtpObj.Network.EnableSsl;
                smtp.UseDefaultCredentials = true;
                NetworkCredential NetworkCred = new NetworkCredential(smtpObj.Network.UserName, smtpObj.Network.Password);
                smtp.UseDefaultCredentials = smtpObj.Network.DefaultCredentials;
                smtp.Credentials = NetworkCred;
                smtp.Port = smtpObj.Network.Port;
                smtp.Send(mm);
            }
            return isSuccess;
        }
    }
}