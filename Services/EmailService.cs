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
            var apiKey = _config["Brevo:ApiKey"];
            var mittente = _config["Brevo:MittenteLeone"];
            var destinatario = _config["Brevo:Destinatario"]; // nuovo
            var mittenteName = "Re Leone CDC - Il Musical";

            request.NumeroPratica = $"RL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var logoUrl = "https://ilreleonecdc.it/assets/images/loghi/LOGO_Rafiki.png";
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new CultureInfo("it-IT"));
            var html = $@"
<!DOCTYPE html>
<html lang=""it"">
  <head>
    <meta charset=""UTF-8"">
    <title>Nuovo messaggio dal form contatti</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  </head>
  <body style=""margin:0;padding:0;background:#f4f4f7;"">
    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:#f4f4f7;padding:24px 0;"">
      <tr>
        <td align=""center"">
          <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""width:600px;max-width:100%;background:#ffffff;border-radius:8px;overflow:hidden;border:1px solid #e6e6eb;"">
            <!-- Header -->
            <tr>
              <td style=""background:#F88D27;color:#ffffff;padding:20px 24px;font-family:Arial,Helvetica,sans-serif;font-size:18px;font-weight:bold;"">
                📥 Nuovo messaggio dal tuo sito
              </td>
            </tr>

            <!-- Intro -->
            <tr>
              <td style=""padding:20px 24px;font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:14px;line-height:20px;"">
                Hai ricevuto un nuovo contatto dalla pagina <strong>Contatti</strong>.
                Di seguito i dettagli inviati dall’utente.
              </td>
            </tr>

            <!-- Dati principali -->
            <tr>
              <td style=""padding:0 24px 8px 24px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse:collapse;font-family:Arial,Helvetica,sans-serif;font-size:14px;color:#111827;"">
                  <tr>
                    <td style=""padding:10px 0;width:160px;color:#6b7280;"">Nome</td>
                    <td style=""padding:10px 0;""><strong>{{name}}</strong></td>
                  </tr>
                  <tr>
                    <td style=""padding:10px 0;color:#6b7280;"">Email</td>
                    <td style=""padding:10px 0;""><a href=""mailto:{{email}}"" style=""color:#2563eb;text-decoration:none;"">{{email}}</a></td>
                  </tr>
                  <tr>
                    <td style=""padding:10px 0;color:#6b7280;"">Telefono</td>
                    <td style=""padding:10px 0;"">{{phone}}</td>
                  </tr>
                  <tr>
                    <td style=""padding:10px 0;color:#6b7280;"">Oggetto</td>
                    <td style=""padding:10px 0;"">{{subject}}</td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- Messaggio -->
            <tr>
              <td style=""padding:16px 24px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border:1px solid #e5e7eb;border-radius:6px;"">
                  <tr>
                    <td style=""background:#f9fafb;padding:10px 12px;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#6b7280;"">
                      Messaggio
                    </td>
                  </tr>
                  <tr>
                    <td style=""padding:14px 12px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:21px;color:#111827;white-space:pre-wrap;"">
                      {{message}}
                    </td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- Metadati -->
            <tr>
              <td style=""padding:8px 24px 20px 24px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#6b7280;"">
                  <tr>
                    <td style=""padding:6px 0;width:160px;"">Consenso privacy</td>
                    <td style=""padding:6px 0;""><strong>{{privacy_consent}}</strong></td>
                  </tr>
                  <tr>
                    <td style=""padding:6px 0;"">Data invio</td>
                    <td style=""padding:6px 0;"">{{submitted_at}}</td>
                  </tr>
                  <tr>
                    <td style=""padding:6px 0;"">IP utente</td>
                    <td style=""padding:6px 0;"">{{ip_address}}</td>
                  </tr>
                  <tr>
                    <td style=""padding:6px 0;"">Pagina origine</td>
                    <td style=""padding:6px 0;"">{{source_url}}</td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""background:#fafafa;color:#6b7280;padding:14px 24px;font-family:Arial,Helvetica,sans-serif;font-size:12px;"">
                Email generata automaticamente dal modulo contatti del sito. Rispondi direttamente a <a href=""mailto:{{email}}"" style=""color:#2563eb;text-decoration:none;"">{{email}}</a>.
              </td>
            </tr>
          </table>

          <!-- CTA rapida -->
          <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""width:600px;max-width:100%;margin-top:12px;"">
            <tr>
              <td align=""center"" style=""font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#6b7280;"">
                <a href=""mailto:{{email}}?subject=Re:%20{{subject | urlencode}}"" style=""display:inline-block;padding:10px 14px;border:1px solid #F88D27;border-radius:6px;text-decoration:none;color:#F88D27;"">Rispondi ora</a>
              </td>
            </tr>
          </table>

        </td>
      </tr>
    </table>
  </body>
</html>
";

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