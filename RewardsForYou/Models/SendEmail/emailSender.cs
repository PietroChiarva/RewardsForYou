using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime;
using System.Text;

namespace RewardsForYou.Domain
{
    public static class EmailSender
    {
        public class Email
        {
            public string SenderName { get; set; }
            public string SenderAddress { get; set; }

            /// <summary>
            /// A comma separated list of email recipients. Accepted separators are: , and ;
            /// </summary>
            public string RecipientEmails { get; set; }

            /// <summary>
            /// A comma separated list of email CC recipients. Accepted separators are: , and ;
            /// </summary>
            public string CcRecipientEmails { get; set; }

            /// <summary>
            /// A comma separated list of email BCC recipients. Accepted separators are: , and ;
            /// </summary>
            public string BccRecipientEmails { get; set; }

            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; } = true;

            public List<Attachment> Attachments { get; set; }
        }

        public static void SendEmail(Email email)
        {
            try
            {
                #region Message setup

                var message = new MailMessage
                {
                    From = string.IsNullOrEmpty(email.SenderName) ? new MailAddress(string.IsNullOrWhiteSpace(email.SenderAddress) ? Settings.MailFrom : email.SenderAddress) : new MailAddress(email.SenderAddress, string.IsNullOrWhiteSpace(email.SenderAddress) ? Settings.MailFrom : email.SenderAddress),
                    IsBodyHtml = email.IsBodyHtml,
                    BodyEncoding = Encoding.UTF8,
                    Body = email.Body,
                    Subject = email.Subject
                };

                //TO
                foreach (var recipient in GetEmailList(string.IsNullOrWhiteSpace(Settings.MailToTest) ? email.RecipientEmails : Settings.MailToTest))
                {
                    message.To.Add(new MailAddress(recipient));
                }

                //CC
                foreach (var recipient in GetEmailList(string.IsNullOrWhiteSpace(Settings.MailCcTest) ? email.CcRecipientEmails : Settings.MailCcTest))
                {
                    message.CC.Add(new MailAddress(recipient));
                }

                //BCC
                foreach (var recipient in GetEmailList(string.IsNullOrWhiteSpace(Settings.MailBccTest) ? email.BccRecipientEmails : Settings.MailBccTest))
                {
                    message.Bcc.Add(new MailAddress(recipient));
                }

                #endregion

                #region Add the attachments

                if (email.Attachments != null)
                {
                    foreach (var attachment in email.Attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                #endregion

                new SmtpClient(Settings.SmtpHost).Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Impossibile spedire la mail. Errore: {ex.Message}).", ex);
            }
        }

        private static List<string> GetEmailList(string emailsCommaSeparatedList)
        {
            if (string.IsNullOrWhiteSpace(emailsCommaSeparatedList))
                return new List<string>();

            return emailsCommaSeparatedList.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}












//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Mail;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace globalEmailSender.email
//{
//    public class emailSender
//    {
//        public static void sendMail(emailInfo email, emailSmtpConfig smtpServer)
//        {


//            try
//            {
//                if (smtpServer.mailSmtp != null)
//                {
//                    if (email != null)
//                    {
//                        using (MailMessage messaggio = new MailMessage())
//                        {
//                            messaggio.From = new MailAddress(email.from);
//                            messaggio.IsBodyHtml = true;

//                            if (email.to != null)
//                            {
//                                foreach (string addressTo in email.to)
//                                {
//                                    messaggio.To.Add(addressTo);
//                                }
//                            }
//                            if (email.cc != null)
//                            {
//                                foreach (string addressCc in email.cc)
//                                {
//                                    messaggio.CC.Add(addressCc);
//                                }
//                            }
//                            if (email.ccn != null)
//                            {
//                                foreach (string addressCcn in email.ccn)
//                                {
//                                    messaggio.Bcc.Add(addressCcn);
//                                }
//                            }

//                            messaggio.Subject = email.subject;
//                            messaggio.BodyEncoding = System.Text.Encoding.UTF8;
//                            messaggio.IsBodyHtml = true;
//                            messaggio.Body = email.body;

//                            if (email.attachments != null)
//                            {
//                                foreach (string attachment in email.attachments)
//                                {
//                                    messaggio.Attachments.Add(new Attachment(attachment));
//                                }
//                            }

//                            SmtpClient server = new SmtpClient(smtpServer.mailSmtp, smtpServer.mailSmtpPort);
//                            NetworkCredential oCredential = new NetworkCredential(smtpServer.mailUser, smtpServer.mailPwd);
//                            server.DeliveryMethod = SmtpDeliveryMethod.Network;
//                            server.UseDefaultCredentials = false;
//                            server.Credentials = oCredential;

//                            server.Send(messaggio);
//                        }


//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Si è verificato un errore durante la spedizione della mail (" + ex.Message + ")");
//            }

//        }

//        public static bool validateMailAddressString(string address)
//        {
//            string matchEmailPattern = @"^(([\w'-]+\.)+[\w'-]+|([a-zA-Z']{1}|[\w'-]{2,}))@"
//                                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
//				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
//                                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
//				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
//                                         + @"([\w-]+\.)+[a-zA-Z]{2,4})$";

//            if (address.Trim() == "") return false;
//            string email = address.Replace(" ", "");
//            string[] emailArray = email.Split(';');
//            foreach (string singleAddress in emailArray)
//            {
//                if (!Regex.IsMatch(singleAddress, matchEmailPattern))
//                {
//                    return false;
//                }
//            }

//            return true;
//        }
//    }

//    public class emailInfo
//    {
//        public string from { get; set; }
//        public string[] to { get; set; }
//        public string[] cc { get; set; }
//        public string[] ccn { get; set; }
//        public string subject { get; set; }
//        public string body { get; set; }
//        public string[] attachments { get; set; }
//    }

//    public class emailSmtpConfig
//    {
//        public string mailSmtp;
//        public int mailSmtpPort;
//        public string mailUser;
//        public string mailPwd;
//    }
//}
