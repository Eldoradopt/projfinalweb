using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Anuncios
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Anuncio Anuncio { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .Include(a => a.AnuncioTags)
                    .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            Anuncio = anuncio;
            return Page();
        }

        public async Task<IActionResult> OnPostToggleFavoritoAsync(int anuncioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Verifica se o anúncio já está marcado como favorito por este utilizador
            var favoritoExistente = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UtilizadorFK == userId && f.AnuncioFK == anuncioId);

            if (favoritoExistente != null)
            {
                // Se já existe, remove dos favoritos
                _context.Favoritos.Remove(favoritoExistente);
            }
            else
            {
                // Se não existe, cria um novo registo de favorito
                var novoFavorito = new Favorito
                {
                    UtilizadorFK = userId,
                    AnuncioFK = anuncioId
                };
                _context.Favoritos.Add(novoFavorito);
            }

            await _context.SaveChangesAsync();

            // Recarrega a página de detalhes para refletir as alterações
            return RedirectToPage(new { id = anuncioId });
        }
    }
}