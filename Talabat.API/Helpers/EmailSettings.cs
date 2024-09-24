
using System.Net;
using System.Net.Mail;
using Talabat.DAL.Entities.Identity;

namespace Talabat.API.Helpers
{
    public class EmailSettings
    {
        public static void SendEmail(Email email)
        {

            var client = new SmtpClient("smtp.sendgrid.net", 587);
            client.EnableSsl = true; //Encrypted
            client.Credentials = new NetworkCredential("apikey", "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");
            client.Send("abdalrhmanfathy170@gmail.com", email.To, email.Title, email.Body);

        }

    }
}
