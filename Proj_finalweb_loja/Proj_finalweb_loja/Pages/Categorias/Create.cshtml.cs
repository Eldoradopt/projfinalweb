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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CategoriaInputModel Input { get; set; } = new CategoriaInputModel();

        public SelectList ParentCategoriesList { get; set; } = null!;

        public class CategoriaInputModel
        {
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

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateParentCategoriesDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropdownAsync();
                return Page();
            }

            var categoria = new Categoria
            {
                Nome = Input.Nome,
                Icone = string.IsNullOrWhiteSpace(Input.Icone) ? "folder" : Input.Icone,
                CategoriaPaiFK = Input.CategoriaPaiFK
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Categoria '" + categoria.Nome + "' criada com sucesso!";
            return RedirectToPage("./Index");
        }

        private async Task PopulateParentCategoriesDropdownAsync()
        {
            var parentCategories = await _context.Categorias
                .Where(c => c.CategoriaPaiFK == null)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            ParentCategoriesList = new SelectList(parentCategories, "Id", "Nome");
        }
    }
}
