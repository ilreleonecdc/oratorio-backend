using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Core;
using oratorio_backend.Models;

namespace oratorio_backend.Services
{
    public class EmailService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public EmailService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<bool> InviaEmaiLAsync(ContattoRequest request)
        {
            var apiKey = _config["Brevo:ApiKey"];
            var mittente = _config["Brevo:Mittente"];
            var destinatario = _config["Brevo:Destinatario"];
            var mittenteName = "Assistenza Oratorio PerDiQua";

            request.NumeroPratica = $"PR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var logoUrl = "https://oratorioperdiqua.it/assets/logo.png";
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new CultureInfo("it-IT"));
            var html = $@"
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-family:Verdana,sans-serif;background:#eef2f5;padding:20px"">
                <tr><td align=""center"">
                    <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:6px;overflow:hidden"">
                                    <!-- Header -->
                                    <tr>
                                    <td style=""background:#007acc;padding:15px;text-align:center"">
                                        <img src=""{logoUrl}"" alt=""Oratorio"" width=""100"" style=""display:block;margin:0 auto""/>
                                        <h2 style=""color:#ffffff;margin:10px 0 0;font-size:20px"">Nuova richiesta di assistenza</h2>
                                    </td>
                                    </tr>
                                    <!-- Info utente -->
                                    <tr>
                                    <td style=""padding:15px;font-size:14px;color:#333333"">
                                        <p><strong>Data/Ora:</strong> {now} UTC</p>
                                        <p><strong>Nome:</strong> {WebUtility.HtmlEncode(request.Nome)} {WebUtility.HtmlEncode(request.Cognome)}</p>
                                        <p><strong>Email utente:</strong> {WebUtility.HtmlEncode(request.Email)}</p>
                                        <p><strong>Oggetto:</strong> {WebUtility.HtmlEncode(request.OggettoRichiesta)}</p>
                                    </td>
                                    </tr>
                                    <!-- Messaggio -->
                                    <tr>
                                    <td style=""padding:0 15px 15px;font-size:14px;color:#555555;line-height:1.5;white-space:pre-wrap;"">
                                        {WebUtility.HtmlEncode(request.Messaggio)}
                                    </td>
                                    </tr>
                                    <!-- Footer -->
                                    <tr>
                                    <td style=""background:#f1f1f1;padding:10px;text-align:center;font-size:12px;color:#777777;"">
                                Questo messaggio è stato inviato dal form di assistenza tecnica su <a href=""https://oratorioperdiqua.it"" style=""color:#007acc;text-decoration:none"">oratorioperdiqua.it</a>
                            </td>
                        </tr>
                    </table>
                </td></tr>
            </table>";

            var body = new
            {
                sender = new
                {
                    name = mittenteName,
                    email = mittente
                },
                to = new[] {
                    new { email = destinatario }
                },
                replyTo = new { email = request.Email, name = $"{request.Nome} {request.Cognome}" },
                subject = $"[{request.NumeroPratica}]: {request.OggettoRichiesta}",
                htmlContent = html,
                header = new Dictionary<string, string>
                {
                    { "X-Priority", "1 (Highest)" },
                    { "X-MSMail-Priority", "High" },
                    { "Importance", "High" }
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Headers = { { "api-key", apiKey } },
                Content = JsonContent.Create(body)
            };

            var res = await _http.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"[Brevo] Status: {(int)res.StatusCode} - Body: {content}");
            return res.IsSuccessStatusCode;
        }
        public async Task<bool> InviaEmailReLeoneAsync(ContattoRequest request)
        {
            var apiKey = _config["Brevo:ApiKey"] ?? throw new InvalidOperationException("Brevo:ApiKey mancante");
            var mittente = _config["Brevo:MittenteLeone"] ?? throw new InvalidOperationException("Brevo:MittenteLeone mancante");
            var destinatario = _config["Brevo:Destinatario"] ?? throw new InvalidOperationException("Brevo:Destinatario mancante"); // nuovo
            var mittenteName = "Re Leone CDC - Il Musical";

            request.NumeroPratica = $"RL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new CultureInfo("it-IT"));
            var html = $@"
                            <!DOCTYPE html>
                            <html lang=""it"">
                            <head>
                            <meta charset=""utf-8"">
                            <title>Nuovo messaggio – Il Re Leone CDC</title>
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                            </head>
                            <body style=""margin:0; padding:0; background:#f6f7fb;"">
                            <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f6f7fb;"">
                                <tr>
                                <td align=""center"" style=""padding:24px 12px;"">
                                    <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""width:600px; max-width:100%; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 6px 24px rgba(0,0,0,.08);"">
                                    <tr>
                                        <td style=""background:#2f408c; padding:18px 20px; text-align:center;"">
                                        <img src=""https://ilreleonecdc.it/assets/images/loghi/LOGO_Rafiki.png"" alt=""Il Re Leone CDC"" width=""72"" height=""72"" style=""display:inline-block;border-radius:50%; border:3px solid #face06;"">
                                        <div style=""font-family:Verdana,Arial,sans-serif; color:#ffffff; font-size:18px; font-weight:bold; margin-top:8px;"">
                                            IL RE LEONE CDC · <span style=""color:#face06;"">IL MUSICAL</span>
                                        </div>
                                        <div style=""font-family:Verdana,Arial,sans-serif; color:#e8e8ef; font-size:12px; margin-top:4px;"">
                                            Pratica <strong style=""color:#face06;"">{request.NumeroPratica}</strong> • {DateTime.Now:dd/MM/yyyy HH:mm}
                                        </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:20px 24px; font-family:Verdana,Arial,sans-serif; font-size:14px; color:#333;"">
                                        <p><strong>Nome:</strong> {request.Nome} {request.Cognome}</p>
                                        <p><strong>Email:</strong> {request.Email}</p>
                                        <p><strong>Oggetto:</strong> {request.OggettoRichiesta}</p>
                                        <div style=""margin-top:12px; padding:12px; background:#fff7ea; border:1px solid #fde2c2; border-radius:10px;"">
                                            {request.Messaggio}
                                        </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""background:#2f408c; padding:14px 20px; text-align:center; font-family:Verdana,Arial,sans-serif; font-size:12px; color:#e8e8ef;"">
                                        Questo messaggio è stato inviato da <a href=""https://ilreleonecdc.it"" style=""color:#face06; font-weight:bold; text-decoration:none;"">ilreleonecdc.it</a>
                                        </td>
                                    </tr>
                                    </table>
                                </td>
                                </tr>
                            </table>
                            </body>
                            </html>";


            var body = new
            {
                sender = new
                {
                    name = mittenteName,
                    email = mittente
                },
                to = new[] {
                    new { email = destinatario }
                },
                replyTo = new { email = request.Email, name = $"{request.Nome} {request.Cognome}" },
                subject = $"[{request.NumeroPratica}]: {request.OggettoRichiesta}",
                htmlContent = html,
                header = new Dictionary<string, string>
                {
                    { "X-Priority", "1 (Highest)" },
                    { "X-MSMail-Priority", "High" },
                    { "Importance", "High" }
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Headers = { { "api-key", apiKey } },
                Content = JsonContent.Create(body)
            };

            var res = await _http.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"[Brevo - ReLeone] Status: {(int)res.StatusCode} {res.ReasonPhrase} - Body: {content}");
            return res.IsSuccessStatusCode;
        }
    }
}