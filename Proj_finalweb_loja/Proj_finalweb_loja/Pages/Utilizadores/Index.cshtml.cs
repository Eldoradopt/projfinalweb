using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Utilizadores
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

        public class VendedorCard
        {
            public ApplicationUser Utilizador { get; set; } = null!;
            public int NumAnuncios { get; set; }
            public int NumVendas { get; set; }
            public double MediaAvaliacoes { get; set; }
            public int NumAvaliacoes { get; set; }
        }

        public IList<VendedorCard> Vendedores { get; set; } = new List<VendedorCard>();

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Anuncios)
                .Include(u => u.AvaliacoesRecebidas)
                .ToListAsync();

            Vendedores = users.Select(u => new VendedorCard
            {
                Utilizador = u,
                NumAnuncios = u.Anuncios.Count(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel),
                NumVendas = u.Anuncios.Count(a => a.Estado == EstadoAnuncio.Vendido),
                MediaAvaliacoes = u.AvaliacoesRecebidas.Any() ? u.AvaliacoesRecebidas.Average(a => a.Nota) : 0,
                NumAvaliacoes = u.AvaliacoesRecebidas.Count
            })
            .OrderByDescending(v => v.NumAnuncios)
            .ToList();
        }
    }
}
