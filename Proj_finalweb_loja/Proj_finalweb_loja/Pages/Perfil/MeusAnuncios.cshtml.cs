using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Perfil
{
    public class MeusAnunciosModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MeusAnunciosModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Anuncio> MeusAnuncios { get; set; } = new List<Anuncio>();
        public decimal TotalVendido { get; set; }
        public int TotalAtivos { get; set; }
        public int TotalReservados { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Get logged-in user (fallback to demo seller)
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                currentUserId = user.Id;
            }
            else
            {
                var demoSeller = await _userManager.FindByEmailAsync("demo@ipt.pt");
                if (demoSeller == null)
                {
                    TempData["ErrorMessage"] = "Crie contas ou inicie sessão.";
                    return RedirectToPage("/Index");
                }
                currentUserId = demoSeller.Id;
            }

            // 2. Fetch all ads of this user
            MeusAnuncios = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Categoria)
                .Include(a => a.Favoritos)
                .Where(a => a.VendedorFK == currentUserId)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            // Calculate Dashboard Statistics
            TotalVendido = MeusAnuncios
                .Where(a => a.Estado == EstadoAnuncio.Vendido)
                .Sum(a => a.Preco);

            TotalAtivos = MeusAnuncios
                .Count(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel);

            TotalReservados = MeusAnuncios
                .Count(a => a.Estado == EstadoAnuncio.Reservado);

            return Page();
        }

        public async Task<IActionResult> OnPostMarkAsReservedAsync(int id)
        {
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.Id == id);
            if (anuncio == null) return NotFound();

            bool isOwner = await CheckOwnershipAsync(anuncio);
            if (!isOwner) return Challenge();

            anuncio.Estado = EstadoAnuncio.Reservado;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Artigo '" + anuncio.Titulo + "' marcado como Reservado.";
            return RedirectToPage("./MeusAnuncios");
        }

        public async Task<IActionResult> OnPostMarkAsSoldAsync(int id)
        {
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.Id == id);
            if (anuncio == null) return NotFound();

            bool isOwner = await CheckOwnershipAsync(anuncio);
            if (!isOwner) return Challenge();

            anuncio.Estado = EstadoAnuncio.Vendido;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Parabéns! Artigo '" + anuncio.Titulo + "' marcado como Vendido.";
            return RedirectToPage("./MeusAnuncios");
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var anuncio = await _context.Anuncios.FirstOrDefaultAsync(a => a.Id == id);
            if (anuncio == null) return NotFound();

            bool isOwner = await CheckOwnershipAsync(anuncio);
            if (!isOwner) return Challenge();

            anuncio.Ativo = !anuncio.Ativo;
            await _context.SaveChangesAsync();

            string statusText = anuncio.Ativo ? "ativado" : "desativado";
            TempData["SuccessMessage"] = "Anúncio '" + anuncio.Titulo + "' " + statusText + " com sucesso!";
            return RedirectToPage("./MeusAnuncios");
        }

        private async Task<bool> CheckOwnershipAsync(Anuncio anuncio)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                return user != null && user.Id == anuncio.VendedorFK;
            }
            else
            {
                var demo = await _userManager.FindByEmailAsync("demo@ipt.pt");
                return demo != null && anuncio.VendedorFK == demo.Id;
            }
        }
    }
}
