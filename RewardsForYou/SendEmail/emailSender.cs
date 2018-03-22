using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace AuthenticationServer.Domain
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
			if(string.IsNullOrWhiteSpace(emailsCommaSeparatedList))
				return new List<string>();

			return emailsCommaSeparatedList.Split(new [] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}
