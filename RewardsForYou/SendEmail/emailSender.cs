using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace globalEmailSender.email
{
    public class emailSender
    {
        public static void sendMail(emailInfo email, emailSmtpConfig smtpServer)
        {
            

            try
            {
                if (smtpServer.mailSmtp != null)
                {
                    if (email != null)
                    {
                        using (MailMessage messaggio = new MailMessage())
                        {
                            messaggio.From = new MailAddress(email.from);
                            messaggio.IsBodyHtml = true;

                            if (email.to != null)
                            {
                                foreach (string addressTo in email.to)
                                {
                                    messaggio.To.Add(addressTo);
                                }
                            }
                            if (email.cc != null)
                            {
                                foreach (string addressCc in email.cc)
                                {
                                    messaggio.CC.Add(addressCc);
                                }
                            }
                            if (email.ccn != null)
                            {
                                foreach (string addressCcn in email.ccn)
                                {
                                    messaggio.Bcc.Add(addressCcn);
                                }
                            }

                            messaggio.Subject = email.subject;
                            messaggio.BodyEncoding = System.Text.Encoding.UTF8;
                            messaggio.IsBodyHtml = true;
                            messaggio.Body = email.body;

                            if (email.attachments != null)
                            {
                                foreach (string attachment in email.attachments)
                                {
                                    messaggio.Attachments.Add(new Attachment(attachment));
                                }
                            }

                            SmtpClient server = new SmtpClient(smtpServer.mailSmtp, smtpServer.mailSmtpPort);
                            NetworkCredential oCredential = new NetworkCredential(smtpServer.mailUser, smtpServer.mailPwd);
                            server.DeliveryMethod = SmtpDeliveryMethod.Network;
                            server.UseDefaultCredentials = false;
                            server.Credentials = oCredential;

                            server.Send(messaggio);
                        }


                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un errore durante la spedizione della mail (" + ex.Message + ")");
            }

        }

        public static bool validateMailAddressString(string address)
        {
            string matchEmailPattern = @"^(([\w'-]+\.)+[\w'-]+|([a-zA-Z']{1}|[\w'-]{2,}))@"
                                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                         + @"([\w-]+\.)+[a-zA-Z]{2,4})$";

            if (address.Trim() == "") return false;
            string email = address.Replace(" ", "");
            string[] emailArray = email.Split(';');
            foreach (string singleAddress in emailArray)
            {
                if (!Regex.IsMatch(singleAddress, matchEmailPattern))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class emailInfo
    {
        public string from;
        public string[] to;
        public string[] cc;
        public string[] ccn;
        public string subject;
        public string body;
        public string[] attachments;
    }

    public class emailSmtpConfig
    {
        public string mailSmtp;
        public int mailSmtpPort;
        public string mailUser;
        public string mailPwd;
    }
}
