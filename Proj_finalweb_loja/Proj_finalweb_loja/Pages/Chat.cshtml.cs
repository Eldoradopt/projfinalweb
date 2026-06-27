using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;

namespace Proj_finalweb_loja.Pages
{
    [Authorize]
    public class ChatModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ChatModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Mensagem> HistoricoMensagens { get; set; } = new List<Mensagem>();
        
        // Propriedades para expor ao HTML da View
        public string DestinatarioId { get; set; } = string.Empty;
        public int AnuncioId { get; set; }
        public string RemetenteId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? anuncioId, string? destinatarioId)
        {
            RemetenteId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            DestinatarioId = destinatarioId ?? string.Empty;
            AnuncioId = anuncioId ?? 0;

            // Se não vierem parâmetros, apenas mostra a página vazia em vez de redirecionar
            if (string.IsNullOrEmpty(destinatarioId) || AnuncioId <= 0)
            {
                return Page();
            }

            // Carrega o histórico trocado entre estes dois utilizadores especificamente neste anúncio
            HistoricoMensagens = await _context.Mensagens
                .Include(m => m.Remetente)
                .Where(m => m.AnuncioFK == AnuncioId &&
                            ((m.RemetenteFK == RemetenteId && m.DestinatarioFK == DestinatarioId) ||
                            (m.RemetenteFK == DestinatarioId && m.DestinatarioFK == RemetenteId)))
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();

            return Page();
        }

        public async Task<JsonResult> OnGetObterContagemNaoLidasAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { count = 0 });
            }

            // Conta as mensagens não lidas destinadas ao utilizador atual
            var contagem = await _context.Mensagens
                .CountAsync(m => m.DestinatarioFK == userId && !m.Lida);

            return new JsonResult(new { count = contagem });
        }
    }
}