using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Anuncios
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
        public IList<Categoria> Categorias { get; set; } = new List<Categoria>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoriaId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrderBy { get; set; }

        public async Task OnGetAsync()
        {
            // Load Categories for filters
            Categorias = await _context.Categorias
                .Where(c => c.CategoriaPaiFK == null)
                .Include(c => c.Subcategorias)
                .ToListAsync();

            var query = _context.Anuncios
                .Where(a => a.Ativo)
                .Include(a => a.Imagens)
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .AsQueryable();

            // Apply Search filter
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(a => a.Titulo.Contains(SearchString) || a.Descricao.Contains(SearchString));
            }

            // Apply Category filter
            if (CategoriaId.HasValue)
            {
                var selectedCat = await _context.Categorias
                    .Include(c => c.Subcategorias)
                    .FirstOrDefaultAsync(c => c.Id == CategoriaId);

                if (selectedCat != null)
                {
                    if (selectedCat.CategoriaPaiFK == null)
                    {
                        var subIds = selectedCat.Subcategorias.Select(s => s.Id).ToList();
                        subIds.Add(selectedCat.Id);
                        query = query.Where(a => subIds.Contains(a.CategoriaFK));
                    }
                    else
                    {
                        query = query.Where(a => a.CategoriaFK == CategoriaId.Value);
                    }
                }
            }

            // Apply Price filters
            if (MinPrice.HasValue)
            {
                query = query.Where(a => a.Preco >= MinPrice.Value);
            }

            if (MaxPrice.HasValue)
            {
                query = query.Where(a => a.Preco <= MaxPrice.Value);
            }

            // Apply Sorting
            switch (OrderBy)
            {
                case "price_asc":
                    query = query.OrderBy(a => a.Preco);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(a => a.Preco);
                    break;
                case "recent":
                default:
                    query = query.OrderByDescending(a => a.DataPublicacao);
                    break;
            }

            Anuncios = await query.ToListAsync();
        }
    }
}
