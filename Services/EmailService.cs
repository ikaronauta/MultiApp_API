// Services/EmailService.cs

using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace MultiApp_API.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public EmailService(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPort = int.Parse(_config["Email:SmtpPort"]);
        var smtpUser = _config["Email:User"];
        var smtpPass = _config["Email:Pass"];

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpUser, "MultiAPP"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        var alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

        var logoPath = Path.Combine(_env.WebRootPath, "images-multiapp", "logo.multiapp.png");

        if (!File.Exists(logoPath))
        {
            throw new Exception("No se encontró la imagen en: " + logoPath);
        }

        var logo = new LinkedResource(logoPath, "image/png")
        {
            ContentId = "logo",
            TransferEncoding = TransferEncoding.Base64
        };

        alternateView.LinkedResources.Add(logo);

        mail.AlternateViews.Add(alternateView);

        await client.SendMailAsync(mail);
    }
}

