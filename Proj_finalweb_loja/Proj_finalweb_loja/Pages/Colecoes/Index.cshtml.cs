using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Colecoes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ColecaoCard
        {
            public Tag Tag { get; set; } = null!;
            public int NumAnuncios { get; set; }
        }

        public IList<ColecaoCard> Colecoes { get; set; } = new List<ColecaoCard>();

        public async Task OnGetAsync()
        {
            var specialTags = await _context.Tags
                .Where(t => t.EColecaoEspecial)
                .Include(t => t.AnuncioTags)
                    .ThenInclude(at => at.Anuncio)
                .ToListAsync();

            Colecoes = specialTags.Select(t => new ColecaoCard
            {
                Tag = t,
                NumAnuncios = t.AnuncioTags.Count(at => at.Anuncio.Ativo && at.Anuncio.Estado == EstadoAnuncio.Disponivel)
            })
            .OrderByDescending(c => c.NumAnuncios)
            .ToList();
        }
    }
}
