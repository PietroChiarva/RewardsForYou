using System.Web.Configuration;

namespace RewardsForYou.Domain
{
    public static class Settings
    {
        public static string SmtpHost => WebConfigurationManager.AppSettings["MAIL_SMTPHOST"];
        public static int? SmtpPort
        {
            get
            {
                var value = WebConfigurationManager.AppSettings["MAIL_SMTPPORT"];
                if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int result))
                    return result;

                return null;
            }
        }
        public static string SmtpUsername => WebConfigurationManager.AppSettings["MAIL_USER"];
        public static string SmtpPassword => WebConfigurationManager.AppSettings["MAIL_PWD"];
        public static string MailFrom => WebConfigurationManager.AppSettings["MAIL_FROM"];
        public static string MailToTest => WebConfigurationManager.AppSettings["MAIL_TO_TEST"];
        public static string MailCcTest => WebConfigurationManager.AppSettings["MAIL_CC_TEST"];
        public static string MailBccTest => WebConfigurationManager.AppSettings["MAIL_BCC_TEST"];
    }
}
