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

namespace Proj_finalweb_loja.Pages.Perfil
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

        public ApplicationUser PerfilUser { get; set; } = null!;
        public IList<Anuncio> AnunciosAtivos { get; set; } = new List<Anuncio>();
        public double MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            string targetUserId;

            if (string.IsNullOrEmpty(id))
            {
                // If no user ID is specified, check if current user is logged in
                if (User.Identity?.IsAuthenticated == true)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return NotFound();
                    targetUserId = currentUser.Id;
                }
                else
                {
                    // Fallback to Demo seller for easy testing in development
                    var demoSeller = await _userManager.FindByEmailAsync("demo@ipt.pt");
                    if (demoSeller == null)
                    {
                        TempData["ErrorMessage"] = "Utilizador não encontrado.";
                        return RedirectToPage("/Index");
                    }
                    targetUserId = demoSeller.Id;
                }
            }
            else
            {
                targetUserId = id;
            }

            // Fetch target user with their reviews and details
            var user = await _context.Users
                .Include(u => u.AvaliacoesRecebidas)
                    .ThenInclude(r => r.Avaliador)
                .FirstOrDefaultAsync(u => u.Id == targetUserId);

            if (user == null)
            {
                return NotFound();
            }

            PerfilUser = user;

            // Fetch active listings for this user
            AnunciosAtivos = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Categoria)
                .Where(a => a.VendedorFK == targetUserId && a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            // Calculate feedback average
            if (user.AvaliacoesRecebidas.Any())
            {
                MediaAvaliacoes = Math.Round(user.AvaliacoesRecebidas.Average(r => r.Nota), 1);
                TotalAvaliacoes = user.AvaliacoesRecebidas.Count;
            }
            else
            {
                MediaAvaliacoes = 0;
                TotalAvaliacoes = 0;
            }

            return Page();
        }
    }
}
