using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Categorias
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Categoria> Categorias { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Categorias = await _context.Categorias
                .Include(c => c.CategoriaPai)
                .Include(c => c.Subcategorias)
                .OrderBy(c => c.CategoriaPaiFK != null ? c.CategoriaPai!.Nome : c.Nome)
                .ThenBy(c => c.Nome)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Anuncios)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            if (categoria.Subcategorias.Any())
            {
                TempData["ErrorMessage"] = "Não podes eliminar esta categoria porque ela contém subcategorias.";
                return RedirectToPage("./Index");
            }

            if (categoria.Anuncios.Any())
            {
                TempData["ErrorMessage"] = "Não podes eliminar esta categoria porque existem anúncios ativos associados a ela.";
                return RedirectToPage("./Index");
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Categoria '" + categoria.Nome + "' eliminada com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
