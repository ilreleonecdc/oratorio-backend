using System;
using System.Collections.Generic;
using System.Linq;
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

            var body = new
            {
                sender = new { name = request.Nome, cognome = request.Cognome, email = request.Email },
                to = new[] { new { email = destinatario } },
                subject = "Messaggi dal sito",
                htmlContent = $"<p><b>Nome:</b> {request.Nome}</p><p><b>Email:</b> {request.Email}</p><p>{request.Messaggio}</p>"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Headers = { { "api-key", apiKey } },
                Content = JsonContent.Create(body)
            };

            var res = await _http.SendAsync(req);
            return res.IsSuccessStatusCode;
        }
    }
}