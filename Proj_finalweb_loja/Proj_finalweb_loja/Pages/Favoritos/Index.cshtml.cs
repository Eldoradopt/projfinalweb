using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Favoritos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Favorito> MeusFavoritos { get; set; } = new List<Favorito>();

        public async Task<IActionResult> OnGetAsync()
        {
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                currentUserId = user.Id;
            }
            else
            {
                // Demo mode fallback: João's favorites
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null)
                {
                    TempData["ErrorMessage"] = "Por favor crie contas ou inicie sessão.";
                    return RedirectToPage("/Index");
                }
                currentUserId = demoUser.Id;
            }

            MeusFavoritos = await _context.Favoritos
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a!.Imagens)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a!.Categoria)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a!.Vendedor)
                .Where(f => f.UtilizadorFK == currentUserId && f.Anuncio!.Ativo)
                .OrderByDescending(f => f.DataGuardado)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int anuncioId)
        {
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                currentUserId = user.Id;
            }
            else
            {
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null) return RedirectToPage("/Index");
                currentUserId = demoUser.Id;
            }

            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UtilizadorFK == currentUserId && f.AnuncioFK == anuncioId);

            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Artigo removido dos favoritos.";
            }

            return RedirectToPage("./Index");
        }

        // Toggle Favorite helper handler that can be called from catalog pages
        public async Task<IActionResult> OnPostToggleAsync(int anuncioId, string returnUrl)
        {
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                currentUserId = user.Id;
            }
            else
            {
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null) return RedirectToPage("/Index");
                currentUserId = demoUser.Id;
            }

            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UtilizadorFK == currentUserId && f.AnuncioFK == anuncioId);

            if (favorito == null)
            {
                // Add to favorites
                var newFav = new Favorito
                {
                    UtilizadorFK = currentUserId,
                    AnuncioFK = anuncioId,
                    DataGuardado = DateTime.UtcNow
                };
                _context.Favoritos.Add(newFav);
                TempData["SuccessMessage"] = "Artigo adicionado aos teus favoritos!";
            }
            else
            {
                // Remove from favorites
                _context.Favoritos.Remove(favorito);
                TempData["SuccessMessage"] = "Artigo removido dos teus favoritos.";
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToPage("/Anuncios/Index");
        }
    }
}
