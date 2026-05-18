using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Categoria> CategoriasPrincipais { get; set; } = new List<Categoria>();
        public IList<Anuncio> AnunciosRecentes { get; set; } = new List<Anuncio>();

        public async Task OnGetAsync()
        {
            CategoriasPrincipais = await _context.Categorias
                .Where(c => c.CategoriaPaiFK == null)
                .ToListAsync();

            AnunciosRecentes = await _context.Anuncios
                .Where(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
                .Include(a => a.Imagens)
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .OrderByDescending(a => a.DataPublicacao)
                .Take(4)
                .ToListAsync();
        }
    }
}
