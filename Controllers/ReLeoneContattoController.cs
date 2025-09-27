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
            if (!ModelState.IsValid) return BadRequest(ModelState);   // ✅ prima di tutto

            try
            {
                var ok = await _email.InviaEmailReLeoneAsync(request); // o InviaEmaiLAsync
                if (ok)
                    return Ok(new { messaggio = "Email inviata con successo", numeroPratica = request.NumeroPratica });

                return StatusCode(502, new { errore = "Invio verso Brevo fallito" }); // 502 > upstream
            }
            catch (Exception ex)
            {
                // logga ex.Message/ex.StackTrace
                return StatusCode(500, new { errore = "Errore interno", dettaglio = ex.Message });
            }
        }
    }
}