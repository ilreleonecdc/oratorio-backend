using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oratorio_backend.Models
{
    public class ContattoRequest
    {
        public required string Nome { get; set; } = default!;
        public required string Cognome { get; set; } = default!;
        public required string Email { get; set; } = default!;
        public required string Messaggio { get; set; } = default!;

        public ContattoRequest() { }
    }
}