using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Security.Claims;

namespace Proj_finalweb_loja.Pages
{
    public class FavoritosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FavoritosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ajusta o tipo "Anuncio" se o nome do teu modelo de artigos for diferente
        public IList<Anuncio> MeusFavoritos { get; set; } = new List<Anuncio>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Carrega os anúncios favoritos do utilizador autenticado
            // Nota: Ajusta os nomes das propriedades ("Favoritos", "Anuncio", "UtilizadorFK") conforme a tua tabela intermédia
            MeusFavoritos = await _context.Favoritos
                .Where(f => f.UtilizadorFK == userId)
                .Select(f => f.Anuncio)
                .ToListAsync();

            return Page();
        }
    }
}