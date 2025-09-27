using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using oratorio_backend.Models;
using oratorio_backend.Services;

namespace oratorio_backend.Controllers
{
    [ApiController]
    [Route("api/releone/contatto")]
    public class ReLeoneContattoController : ControllerBase
    {
        private readonly EmailService _email;

        public ReLeoneContattoController(EmailService email)
        {
            _email = email;
        }

        [HttpPost]
        public async Task<IActionResult> InviaMessaggioAsync([FromBody] ContattoRequest request)
        {
            var succes = await _email.InviaEmailReLeoneAsync(request);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (succes) return Ok(new
            {
                messaggio = "Email inviata con successo!",
                numeroPratica = request.NumeroPratica
            });

            return StatusCode(500, new { errore = "Errore durante l'invio" });
        }
    }
}