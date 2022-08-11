namespace DohrniiBackoffice.Models
{
    public class EmailSettings
    {
        public string MailSender { get; set; }
        public string SmptHost { get; set; }
        public int SmptPort { get; set; }
        public string SmptUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSsl { get; set; }
    }
}
