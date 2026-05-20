using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Categorias
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CategoriaEditInputModel Input { get; set; } = new CategoriaEditInputModel();

        public Categoria Categoria { get; set; } = null!;
        public SelectList ParentCategoriesList { get; set; } = null!;

        public class CategoriaEditInputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
            [StringLength(100, ErrorMessage = "O nome não pode exceder {1} caracteres.")]
            [Display(Name = "Nome da Categoria")]
            public string Nome { get; set; } = string.Empty;

            [StringLength(50, ErrorMessage = "O ícone não pode exceder {1} caracteres.")]
            [Display(Name = "Ícone Bootstrap (Ex: laptop, bag-heart, bicycle)")]
            public string? Icone { get; set; }

            [Display(Name = "Categoria Pai (Deixar em branco se for categoria principal)")]
            public int? CategoriaPaiFK { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .Include(c => c.CategoriaPai)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            Categoria = categoria;

            Input.Id = categoria.Id;
            Input.Nome = categoria.Nome;
            Input.Icone = categoria.Icone;
            Input.CategoriaPaiFK = categoria.CategoriaPaiFK;

            await PopulateParentCategoriesDropdownAsync(categoria.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var originalCat = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == Input.Id);
                if (originalCat == null) return NotFound();

                Categoria = originalCat;
                await PopulateParentCategoriesDropdownAsync(Input.Id);
                return Page();
            }

            var categoriaToUpdate = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == Input.Id);
            if (categoriaToUpdate == null)
            {
                return NotFound();
            }

            // Prevent circular references
            if (Input.CategoriaPaiFK.HasValue && Input.CategoriaPaiFK.Value == Input.Id)
            {
                ModelState.AddModelError("Input.CategoriaPaiFK", "Uma categoria não pode ser pai de si própria.");
                Categoria = categoriaToUpdate;
                await PopulateParentCategoriesDropdownAsync(Input.Id);
                return Page();
            }

            categoriaToUpdate.Nome = Input.Nome;
            categoriaToUpdate.Icone = string.IsNullOrWhiteSpace(Input.Icone) ? "folder" : Input.Icone;
            categoriaToUpdate.CategoriaPaiFK = Input.CategoriaPaiFK;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Categoria '" + categoriaToUpdate.Nome + "' atualizada com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(categoriaToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }

        private async Task PopulateParentCategoriesDropdownAsync(int excludeId)
        {
            var parentCategories = await _context.Categorias
                .Where(c => c.CategoriaPaiFK == null && c.Id != excludeId)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            ParentCategoriesList = new SelectList(parentCategories, "Id", "Nome");
        }
    }
}
