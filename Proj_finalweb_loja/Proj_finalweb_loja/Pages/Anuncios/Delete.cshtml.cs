using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Anuncios
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            Anuncio = anuncio;

            // Check authorization
            bool isAuthorized = await CheckAuthorizationAsync(anuncio);
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "Não tens permissão para eliminar este anúncio.";
                return RedirectToPage("/Anuncios/Details", new { id = anuncio.Id });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Imagens)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            // Check authorization
            bool isAuthorized = await CheckAuthorizationAsync(anuncio);
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "Não tens permissão para eliminar este anúncio.";
                return RedirectToPage("/Anuncios/Details", new { id = anuncio.Id });
            }

            // Remove associated images
            if (anuncio.Imagens.Any())
            {
                _context.Imagens.RemoveRange(anuncio.Imagens);
            }

            // Remove advertisement
            _context.Anuncios.Remove(anuncio);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "O teu anúncio '" + anuncio.Titulo + "' foi eliminado com sucesso.";
            return RedirectToPage("/Anuncios/Index");
        }

        private async Task<bool> CheckAuthorizationAsync(Anuncio anuncio)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && (currentUser.Id == anuncio.VendedorFK || User.IsInRole("Admin")))
                {
                    return true;
                }
            }
            else
            {
                // Demo fallback
                var demoSeller = await _userManager.FindByEmailAsync("demo@ipt.pt");
                if (demoSeller != null && anuncio.VendedorFK == demoSeller.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
