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
    }
}