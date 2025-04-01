
using Microsoft.Extensions.Configuration;
using Moralar.UtilityFramework.Services.Core.Interface;
using Moralar.UtilityFramework.Services.Core.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Moralar.UtilityFramework.Application.Core;
using MimeKit;

using MailKit.Net.Smtp;
using MailKit.Security;
using Moralar.UtilityFramework.Configuration;
using System.IO;

namespace Moralar.UtilityFramework.Services.Core
{
    public class SendService : ISenderMailService, ISenderNotificationService
    {
        private readonly Config _config;

        public SendService()
        {
            IConfigurationRoot configurationRoot = Utilities.GetConfigurationRoot();
            _config = configurationRoot.GetSection("SERVICES").Get<Config>();
        }

        public void SendMessageEmail(string nome, string email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string path = null, List<string> ccEmails = null, List<string> ccoEmails = null, List<string> replyTo = null)
        {
            try
            {
                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL));
                mimeMessage.To.Add(new MailboxAddress(nome, email));
                if (ccEmails != null && ccEmails.Count > 0)
                {
                    for (int i = 0; i < ccEmails.Count; i++)
                    {
                        mimeMessage.Cc.Add(new MailboxAddress("", ccEmails[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        mimeMessage.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        mimeMessage.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                mimeMessage.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{mimeMessage.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{mimeMessage.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(path))
                {
                    path.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                mimeMessage.Body = builder.ToMessageBody();
                using SmtpClient smtpClient = new SmtpClient();

                var client = new SmtpClient();
                #pragma warning disable CS8622
                client.ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true;

                client.ConnectAsync(_config.SMTP.HOST, _config.SMTP.PORT, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (_config.SMTP.USEAUTH)
                {
                    client.AuthenticateAsync(_config.SMTP.EMAIL, _config.SMTP.PASSWORD);
                }

                client.Send(mimeMessage);
                client.DisconnectAsync(quit: true);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public void SendMessageEmail(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null)
        {
            try
            {
                if (!email.Any())
                {
                    throw new Exception("Informe pelo menos um destinatário");
                }

                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL));
                mimeMessage.To.Add(new MailboxAddress(nome, email[0]));
                email.RemoveAt(0);
                if (email.Count > 0)
                {
                    for (int i = 0; i < email.Count; i++)
                    {
                        mimeMessage.Cc.Add(new MailboxAddress("", email[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        mimeMessage.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        mimeMessage.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                mimeMessage.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{mimeMessage.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{mimeMessage.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(pathFile))
                {
                    pathFile.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                mimeMessage.Body = builder.ToMessageBody();
                using SmtpClient smtpClient = new SmtpClient();

                var client = new SmtpClient();
                #pragma warning disable CS8622
                client.ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true;

                client.ConnectAsync(_config.SMTP.HOST, _config.SMTP.PORT, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (_config.SMTP.USEAUTH)
                {
                    client.AuthenticateAsync(_config.SMTP.EMAIL, _config.SMTP.PASSWORD);
                }

                client.Send(mimeMessage);
                client.DisconnectAsync(quit: true);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public void SendMessageEmailAmazon(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null)
        {
            try
            {
                if (!email.Any())
                {
                    throw new Exception("Informe pelo menos um destinatário");
                }

                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL));
                mimeMessage.To.Add(new MailboxAddress(nome, email[0]));
                email.RemoveAt(0);
                if (email.Count > 0)
                {
                    for (int i = 0; i < email.Count; i++)
                    {
                        mimeMessage.Cc.Add(new MailboxAddress("", email[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        mimeMessage.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        mimeMessage.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                mimeMessage.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{mimeMessage.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{mimeMessage.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(pathFile))
                {
                    pathFile.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                mimeMessage.Body = builder.ToMessageBody();
                using SmtpClient smtpClient = new SmtpClient();
                // Its a configuration for amazon smtp and not needed in 2025

                //smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                //smtpClient.Connect(new Uri("smtp://" + _config.SMTP.HOST + ":" + _config.SMTP.PORT + "/?starttls=" + _config.SMTP.SSL));
                //if (_config.SMTP.USEAUTH)
                //{
                //    smtpClient.Authenticate(_config.SMTP.LOGIN, _config.SMTP.PASSWORD);
                //}

                //smtpClient.Send(mimeMessage);
                //smtpClient.Disconnect(quit: true);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public async Task SendMessageEmailAsync(string nome, string email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccEmails = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false)
        {
            _ = 3;
            try
            {
                if (!email.Any())
                {
                    throw new Exception("Informe pelo menos um destinatário");
                }

                MimeMessage message = new MimeMessage
                {
                    From = { (InternetAddress)new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL) },
                    To = { (InternetAddress)new MailboxAddress(nome, email) }
                };
                if (ccEmails != null && ccEmails.Count > 0)
                {
                    for (int i = 0; i < ccEmails.Count; i++)
                    {
                        message.Cc.Add(new MailboxAddress("", ccEmails[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        message.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        message.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                message.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };

                var image = await GetImageLogo("logo", "png", true);

                builder.LinkedResources.Add(image);


                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{message.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{message.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(pathFile))
                {
                    pathFile.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                message.Body = builder.ToMessageBody();

                var client = new SmtpClient();
                #pragma warning disable CS8622
                client.ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true;

                await client.ConnectAsync(_config.SMTP.HOST, _config.SMTP.PORT, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (_config.SMTP.USEAUTH)
                {
                    await client.AuthenticateAsync(_config.SMTP.EMAIL, _config.SMTP.PASSWORD).ConfigureAwait(configureAwait);
                }

                await client.SendAsync(message).ConfigureAwait(configureAwait);
                await client.DisconnectAsync(quit: true).ConfigureAwait(configureAwait);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public async Task<MimePart> GetImageLogo(string fileName, string extension, bool isDefault)
        {
            ConfigurationHelper conf = new ConfigurationHelper();

            string baseUrlImages = isDefault ? conf.getBaseUrl().Replace("upload", "images") : conf.getBaseUrl();


            string imageUrl = $"{baseUrlImages}/{fileName}.{extension}";
            Stream imageStream = await GetStreamFromUrlAsync(imageUrl);


            return new MimePart("image", "png")
            {
                Content = new MimeContent(imageStream),
                ContentId = fileName,
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                ContentTransferEncoding = ContentEncoding.Base64,
                IsAttachment = false
            };
        }

        private async Task<Stream> GetStreamFromUrlAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(url);
            MemoryStream stream = new MemoryStream(imageBytes);
            return stream;
        }

        public async Task SendMessageEmailAsync(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false)
        {
            _ = 3;
            try
            {
                if (!email.Any())
                {
                    throw new Exception("Informe pelo menos um destinatário");
                }

                MimeMessage message = new MimeMessage
                {
                    From = { (InternetAddress)new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL) },
                    To = { (InternetAddress)new MailboxAddress(nome, email[0]) }
                };
                email.RemoveAt(0);
                if (email.Count > 0)
                {
                    for (int i = 0; i < email.Count; i++)
                    {
                        message.Cc.Add(new MailboxAddress("", email[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        message.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        message.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                message.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };

                var image = await GetImageLogo("logo", "png", true);

                builder.LinkedResources.Add(image);

                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{message.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{message.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(pathFile))
                {
                    pathFile.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                message.Body = builder.ToMessageBody();

                var client = new SmtpClient();
                #pragma warning disable CS8622
                client.ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true;

                await client.ConnectAsync(_config.SMTP.HOST, _config.SMTP.PORT, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                if (_config.SMTP.USEAUTH)
                {
                    await client.AuthenticateAsync(_config.SMTP.EMAIL, _config.SMTP.PASSWORD).ConfigureAwait(configureAwait);
                }

                await client.SendAsync(message).ConfigureAwait(configureAwait);
                await client.DisconnectAsync(quit: true).ConfigureAwait(configureAwait);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public async Task SendMessageEmailAmazonAsync(string nome, List<string> email, string body, string subject, string titleEvent = null, bool invite = false, string local = null, DateTime? inicio = null, DateTime? utc = null, DateTime? fim = null, bool recorrente = false, string pathFile = null, List<string> ccoEmails = null, List<string> replyTo = null, bool configureAwait = false)
        {
            _ = 3;
            try
            {
                if (!email.Any())
                {
                    throw new Exception("Informe pelo menos um destinatário");
                }

                MimeMessage message = new MimeMessage
                {
                    From = { (InternetAddress)new MailboxAddress(_config.SMTP.NAME, _config.SMTP.EMAIL) },
                    To = { (InternetAddress)new MailboxAddress(nome, email[0]) }
                };
                email.RemoveAt(0);
                if (email.Count > 0)
                {
                    for (int i = 0; i < email.Count; i++)
                    {
                        message.Cc.Add(new MailboxAddress("", email[i]));
                    }
                }

                if (ccoEmails != null && ccoEmails.Count > 0)
                {
                    for (int j = 0; j < ccoEmails.Count; j++)
                    {
                        message.Bcc.Add(new MailboxAddress("", ccoEmails[j]));
                    }
                }

                if (replyTo != null && replyTo.Count > 0)
                {
                    for (int k = 0; k < replyTo.Count; k++)
                    {
                        message.ReplyTo.Add(new MailboxAddress("", replyTo[k]));
                    }
                }

                message.Subject = subject;
                BodyBuilder builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                if (invite)
                {
                    local = local ?? "Não informado";
                    inicio = inicio ?? DateTime.Now.AddHours(1.0);
                    fim = fim ?? inicio.GetValueOrDefault().AddHours(2.0);
                    utc = utc ?? DateTime.UtcNow.AddHours(1.0);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("BEGIN:VCALENDAR");
                    stringBuilder.AppendLine("PRODID:-//Schedule a Meeting");
                    stringBuilder.AppendLine("VERSION:2.0");
                    stringBuilder.AppendLine("METHOD:REQUEST");
                    stringBuilder.AppendLine("BEGIN:VEVENT");
                    stringBuilder.AppendLine($"DTSTART:{inicio?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTSTAMP:{utc?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    stringBuilder.AppendLine($"DTEND:{fim?.ToUniversalTime():yyyyMMddTHHmmssZ}");
                    if (recorrente)
                    {
                        stringBuilder.AppendLine($"RRULE:FREQ=WEEKLY;COUNT={157}");
                    }

                    stringBuilder.AppendLine("LOCATION:" + local);
                    stringBuilder.AppendLine($"UID:{Guid.NewGuid()}");
                    stringBuilder.AppendLine($"DESCRIPTION:{message.Body}");
                    stringBuilder.AppendLine($"X-ALT-DESC;FMTTYPE=text/html:{message.Body}");
                    stringBuilder.AppendLine("SUMMARY:" + titleEvent);
                    stringBuilder.AppendLine("ORGANIZER:MAILTO:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("ATTENDEE;CN=\"" + _config.SMTP.NAME + "\";RSVP=TRUE:mailto:" + _config.SMTP.EMAIL);
                    stringBuilder.AppendLine("BEGIN:VALARM");
                    stringBuilder.AppendLine("TRIGGER:-PT15M");
                    stringBuilder.AppendLine("ACTION:DISPLAY");
                    stringBuilder.AppendLine("DESCRIPTION:Reminder");
                    stringBuilder.AppendLine("END:VALARM");
                    stringBuilder.AppendLine("END:VEVENT");
                    stringBuilder.AppendLine("END:VCALENDAR");
                    ContentType contentType = new ContentType("text/calendar", "text/calendar");
                    contentType.Parameters?.Add("method", "REQUEST");
                    contentType.Parameters?.Add("name", titleEvent.TrimSpaces() + ".ics");
                    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
                    builder.Attachments.Add("Meeding.ics", stream, contentType);
                }

                if (!string.IsNullOrEmpty(pathFile))
                {
                    pathFile.Split(';').ToList().ForEach(delegate (string filePath)
                    {
                        builder.Attachments.Add(filePath);
                    });
                }

                message.Body = builder.ToMessageBody();
                using SmtpClient client = new SmtpClient();
                // Its a configuration for amazon smtp and not needed in 2025
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                //await client.ConnectAsync(new Uri("smtp://" + _config.SMTP.HOST + ":" + _config.SMTP.PORT + "/?starttls=" + _config.SMTP.SSL)).ConfigureAwait(configureAwait);
                //if (_config.SMTP.USEAUTH)
                //{
                //    await client.AuthenticateAsync(_config.SMTP.LOGIN, _config.SMTP.PASSWORD).ConfigureAwait(configureAwait);
                //}

                //await client.SendAsync(message).ConfigureAwait(configureAwait);
                //await client.DisconnectAsync(quit: true).ConfigureAwait(configureAwait);
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao enviar e-mail", innerException);
            }
        }

        public string GerateBody(string filename, Dictionary<string, string> substituicao)
        {
            string seed = LoadTemplate(filename + ".html");
            return substituicao.Aggregate(seed, (string current, KeyValuePair<string, string> item) => current.Replace(item.Key, item.Value));
        }

        private string LoadTemplate(string fileName)
        {
            try
            {
                return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory() + "/" + _config.SMTP.TEMPLATE, fileName));
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao buscar template de e-mail", innerException);
            }
        }

        public OneSignalResponse SendPush(string senderName, string message, IEnumerable<string> devicePushId, string groupName = null, string senderPhoto = null, JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, int messagelength = 150, int titleLength = 50)
        {
            try
            {
                List<string> list = devicePushId?.Where((string x) => !string.IsNullOrEmpty(x)).Distinct().ToList() ?? new List<string>();
                if (list.Count == 0)
                {
                    return null;
                }

                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications");
                RestRequest restRequest = new RestRequest(Method.POST);
                dynamic val = Combine(new JObject(), settings);
                dynamic val2 = new JObject();
                val2.en = message.Truncate(messagelength);
                dynamic val3 = new JObject();
                val3.en = senderName;
                val.ios_sound = sound ?? _config.ONESIGNAL[indexKeys].SOUND;
                val.android_sound = RemoveExt(sound ?? _config.ONESIGNAL[indexKeys].SOUND);
                val.app_id = _config.ONESIGNAL[indexKeys].APPID;
                val.contents = val2;
                val.small_icon = customIcon ?? _config.ONESIGNAL[indexKeys].ICON;
                val.headings = val3;
                val.priority = priority;
                if (!string.IsNullOrEmpty(groupName))
                {
                    val.android_group = "group_" + groupName;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    val.url = url;
                }

                if (!string.IsNullOrWhiteSpace(senderPhoto))
                {
                    val.large_icon = senderPhoto;
                }

                if (data != null)
                {
                    val.data = data;
                }

                if (dataSend.HasValue)
                {
                    val.send_after = dataSend.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss zzz");
                }

                val.include_player_ids = new JArray(list);
                dynamic val4 = JsonConvert.SerializeObject(val);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                restRequest.AddParameter("application/json", val4, ParameterType.RequestBody);
                IRestResponse<OneSignalResponse> result = client.Execute<OneSignalResponse>(restRequest).Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    result.Data.Erro = true;
                }

                OneSignalResponse data2 = result.Data;
                data2.StatusCode = (int)result.StatusCode;
                return data2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao enviar notificação onesignal.", innerException);
            }
        }

        public OneSignalResponse SendAllDevices(string senderName, string message, string groupName = null, string senderPhoto = null, string segments = "All", JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, int messagelength = 150, int titleLength = 50)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications");
                RestRequest restRequest = new RestRequest(Method.POST);
                dynamic val = Combine(new JObject(), settings);
                dynamic val2 = new JObject();
                val2.en = message.Truncate(messagelength);
                dynamic val3 = new JObject();
                val3.en = senderName.Truncate(titleLength);
                val.ios_sound = sound ?? _config.ONESIGNAL[indexKeys].SOUND;
                val.android_sound = RemoveExt(sound ?? _config.ONESIGNAL[indexKeys].SOUND);
                val.app_id = _config.ONESIGNAL[indexKeys].APPID;
                val.contents = val2;
                val.small_icon = customIcon ?? _config.ONESIGNAL[indexKeys].ICON;
                val.headings = val3;
                val.priority = priority;
                val.included_segments = new JArray(new List<string> { segments });
                if (!string.IsNullOrEmpty(groupName))
                {
                    val.android_group = "group_" + groupName;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    val.url = url;
                }

                if (data != null)
                {
                    val.data = data;
                }

                if (!string.IsNullOrWhiteSpace(senderPhoto))
                {
                    val.large_icon = senderPhoto;
                }

                if (dataSend.HasValue)
                {
                    val.send_after = dataSend.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss zzz");
                }

                dynamic val4 = JsonConvert.SerializeObject(val);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                restRequest.AddParameter("application/json", val4, ParameterType.RequestBody);
                IRestResponse<OneSignalResponse> result = client.Execute<OneSignalResponse>(restRequest).Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    result.Data.Erro = true;
                }

                OneSignalResponse data2 = result.Data;
                data2.StatusCode = (int)result.StatusCode;
                return data2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao enviar notificação onesignal.", innerException);
            }
        }

        public OneSignalModel GetNotification(string notificationId, int indexKeys = 0)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications/" + notificationId + "?app_id=" + _config.ONESIGNAL[indexKeys].APPID);
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                IRestResponse<OneSignalModel> result = client.Execute<OneSignalModel>(restRequest).Result;
                if (result.StatusCode != HttpStatusCode.OK && result.Data != null)
                {
                    result.Data.Erro = true;
                }

                return result.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao cancelar notificação onesignal.", innerException);
            }
        }

        public bool CancelPush(string notificationId, int indexKeys = 0)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications/" + notificationId + "?app_id=" + _config.ONESIGNAL[indexKeys].APPID);
                RestRequest restRequest = new RestRequest(Method.DELETE);
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                IRestResponse<OneSignalResponse> result = client.Execute<OneSignalResponse>(restRequest).Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    result.Data.Erro = true;
                }

                return result.Data.Success;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao cancelar notificação onesignal.", innerException);
            }
        }

        public async Task<OneSignalResponse> SendPushAsync(string senderName, string message, IEnumerable<string> devicePushId, string groupName = null, string senderPhoto = null, JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, bool configureAwait = false, int messagelength = 150, int titleLength = 50)
        {
            try
            {
                List<string> list = devicePushId?.Where((string x) => !string.IsNullOrEmpty(x)).Distinct().ToList() ?? new List<string>();
                if (list.Count == 0)
                {
                    return null;
                }

                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications");
                RestRequest restRequest = new RestRequest(Method.POST);
                dynamic val = Combine(new JObject(), settings);
                dynamic val2 = new JObject();
                val2.en = message.Truncate(messagelength);
                dynamic val3 = new JObject();
                val3.en = senderName.Truncate(titleLength);
                val.ios_sound = sound ?? _config.ONESIGNAL[indexKeys].SOUND;
                val.android_sound = RemoveExt(sound ?? _config.ONESIGNAL[indexKeys].SOUND);
                val.app_id = _config.ONESIGNAL[indexKeys].APPID;
                val.contents = val2;
                val.small_icon = customIcon ?? _config.ONESIGNAL[indexKeys].ICON;
                val.headings = val3;
                val.priority = priority;
                if (!string.IsNullOrEmpty(groupName))
                {
                    val.android_group = "group_" + groupName;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    val.url = url;
                }

                if (!string.IsNullOrWhiteSpace(senderPhoto))
                {
                    val.large_icon = senderPhoto;
                }

                if (data != null)
                {
                    val.data = data;
                }

                if (dataSend.HasValue)
                {
                    val.send_after = dataSend.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss zzz");
                }

                val.include_player_ids = new JArray(list);
                dynamic val4 = JsonConvert.SerializeObject(val);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                restRequest.AddParameter("application/json", val4, ParameterType.RequestBody);
                IRestResponse<OneSignalResponse> restResponse = await client.Execute<OneSignalResponse>(restRequest).ConfigureAwait(configureAwait);
                if (restResponse.StatusCode != HttpStatusCode.OK)
                {
                    restResponse.Data.Erro = true;
                }

                OneSignalResponse data2 = restResponse.Data;
                data2.StatusCode = (int)restResponse.StatusCode;
                return data2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao enviar notificação onesignal.", innerException);
            }
        }

        public async Task<OneSignalResponse> SendAllDevicesAsync(string senderName, string message, string groupName = null, string senderPhoto = null, string segments = "All", JObject data = null, DateTime? dataSend = null, int indexKeys = 0, int priority = 1, string url = null, string sound = null, string customIcon = null, JObject settings = null, bool configureAwait = false, int messagelength = 150, int titleLength = 50)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications");
                RestRequest restRequest = new RestRequest(Method.POST);
                dynamic val = Combine(new JObject(), settings);
                dynamic val2 = new JObject();
                val2.en = message.Truncate(messagelength);
                dynamic val3 = new JObject();
                val3.en = senderName.Truncate(titleLength);
                val.ios_sound = sound ?? _config.ONESIGNAL[indexKeys].SOUND;
                val.android_sound = RemoveExt(sound ?? _config.ONESIGNAL[indexKeys].SOUND);
                val.app_id = _config.ONESIGNAL[indexKeys].APPID;
                val.contents = val2;
                val.small_icon = customIcon ?? _config.ONESIGNAL[indexKeys].ICON;
                val.headings = val3;
                val.priority = priority;
                val.included_segments = new JArray(new List<string> { segments });
                if (!string.IsNullOrEmpty(groupName))
                {
                    val.android_group = "group_" + groupName;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    val.url = url;
                }

                if (data != null)
                {
                    val.data = data;
                }

                if (!string.IsNullOrWhiteSpace(senderPhoto))
                {
                    val.large_icon = senderPhoto;
                }

                if (dataSend.HasValue)
                {
                    val.send_after = dataSend.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss zzz");
                }

                dynamic val4 = JsonConvert.SerializeObject(val);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                restRequest.AddParameter("application/json", val4, ParameterType.RequestBody);
                IRestResponse<OneSignalResponse> restResponse = await client.Execute<OneSignalResponse>(restRequest).ConfigureAwait(configureAwait);
                if (restResponse.StatusCode != HttpStatusCode.OK)
                {
                    restResponse.Data.Erro = true;
                }

                OneSignalResponse data2 = restResponse.Data;
                data2.StatusCode = (int)restResponse.StatusCode;
                return data2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao enviar notificação onesignal.", innerException);
            }
        }

        public async Task<OneSignalModel> GetNotificationAsync(string notificationId, int indexKeys = 0, bool configureAwait = false)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications/" + notificationId + "?app_id=" + _config.ONESIGNAL[indexKeys].APPID);
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                IRestResponse<OneSignalModel> restResponse = await client.Execute<OneSignalModel>(restRequest).ConfigureAwait(configureAwait);
                if (restResponse.StatusCode != HttpStatusCode.OK && restResponse.Data != null)
                {
                    restResponse.Data.Erro = true;
                }

                return restResponse.Data;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao cancelar notificação onesignal.", innerException);
            }
        }

        public async Task<bool> CancelPushAsync(string notificationId, int indexKeys = 0, bool configureAwait = false)
        {
            try
            {
                RestClient client = new RestClient("https://onesignal.com/api/v1/notifications/" + notificationId + "?app_id=" + _config.ONESIGNAL[indexKeys].APPID);
                RestRequest restRequest = new RestRequest(Method.DELETE);
                restRequest.AddHeader("Authorization", "Basic " + _config.ONESIGNAL[indexKeys].KEY);
                IRestResponse<OneSignalResponse> restResponse = await client.Execute<OneSignalResponse>(restRequest).ConfigureAwait(configureAwait);
                if (restResponse.StatusCode != HttpStatusCode.OK)
                {
                    restResponse.Data.Erro = true;
                }

                return restResponse.Data.Success;
            }
            catch (Exception innerException)
            {
                throw new Exception("Ocorreu um erro ao cancelar notificação onesignal.", innerException);
            }
        }

        private static dynamic Combine(JObject item1, JObject item2)
        {
            if (item2 != null)
            {
                item1.Merge(item2, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            return item1;
        }

        private string RemoveExt(string file)
        {
            if (file == null)
            {
                return null;
            }

            return file.Split('.')[0];
        }
    }
}
