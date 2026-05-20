using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Colecoes
{
    public class DetalhesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetalhesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Tag Colecao { get; set; } = null!;
        public IList<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

        public async Task<IActionResult> OnGetAsync(int tagId)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId && t.EColecaoEspecial);
            if (tag == null) return NotFound();

            Colecao = tag;

            Anuncios = await _context.AnuncioTags
                .Where(at => at.TagFK == tagId && at.Anuncio.Ativo && at.Anuncio.Estado == EstadoAnuncio.Disponivel)
                .Include(at => at.Anuncio)
                    .ThenInclude(a => a.Imagens)
                .Include(at => at.Anuncio)
                    .ThenInclude(a => a.Vendedor)
                .Include(at => at.Anuncio)
                    .ThenInclude(a => a.Categoria)
                .Select(at => at.Anuncio)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            return Page();
        }
    }
}
