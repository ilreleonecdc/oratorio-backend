using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oratorio_backend.Models;
using oratorio_backend.Services;

namespace oratorio_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContattoController : ControllerBase
    {
        private readonly EmailService _email;

        public ContattoController(EmailService email)
        {
            _email = email;
        }

        [HttpPost]
        public async Task<IActionResult> InviaMessaggio([FromBody] ContattoRequest request)
        {
            var success = await _email.InviaEmaiLAsync(request);
            if (success)
                return Ok(new { messaggio = "Email inviata con successo" });

            return StatusCode(500, new { errore = "Errore durante l'invio" });
        }
    }
}