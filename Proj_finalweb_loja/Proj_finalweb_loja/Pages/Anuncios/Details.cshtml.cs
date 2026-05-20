using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
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
    }
}
