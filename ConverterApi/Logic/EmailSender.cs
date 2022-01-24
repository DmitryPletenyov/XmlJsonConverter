using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ConverterApi.Logic
{
    public static class EmailSender
    {
        // draft
        public static async Task Send(string emailTo, byte[] fileByteArray, string fileName, string contetntType)
        {
            MailAddress from = new MailAddress("converter@contoso.com");
            MailAddress to = new MailAddress(emailTo);

            MailMessage message = new MailMessage(from, to);
            message.Body = "Find converted file in attachments.";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "Find converted file in attachments.";
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            using (var SmtpServer = new SmtpClient("mail.reckonbits.com.pk"))
            {
                using (var stream = new MemoryStream(fileByteArray))
                {
                    message.Attachments.Add(new Attachment(stream, fileName, contetntType));

                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("admin@reckonbits.com.pk", "your password");
                    SmtpServer.EnableSsl = false;
                    await SmtpServer.SendMailAsync(message);
                }
            }
        }
    }
}
