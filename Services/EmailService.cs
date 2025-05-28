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
                sender = new { nome = request.Nome, cognome = request.Cognome, email = mittente },
                to = new[] { new { email = destinatario } },
                subject = $"[{request.NumeroPratica}]: {request.OggettoRichiesta}",
                htmlContent = $@"
                                <p><b>Numero pratica:</b> {request.NumeroPratica}</p>
                                <p><b>Nome:</b> {request.Nome}</p>
                                <p><b>Email:</b> {request.Email}</p>
                                <p><b>Oggetto:</b> {request.OggettoRichiesta}</p>
                                <p>{request.Messaggio}</p>"
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