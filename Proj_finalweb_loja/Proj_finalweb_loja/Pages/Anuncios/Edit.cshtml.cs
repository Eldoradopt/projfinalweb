using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Anuncios
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EditModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public AnuncioEditInputModel Input { get; set; } = new AnuncioEditInputModel();

        public Anuncio Anuncio { get; set; } = null!;
        public SelectList CategoriasList { get; set; } = null!;

        public class AnuncioEditInputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "O título é obrigatório.")]
            [StringLength(100, MinimumLength = 5, ErrorMessage = "O título deve ter entre {2} e {1} caracteres.")]
            [Display(Name = "Título do Anúncio")]
            public string Titulo { get; set; } = string.Empty;

            [Required(ErrorMessage = "A descrição é obrigatória.")]
            [StringLength(4000, MinimumLength = 15, ErrorMessage = "A descrição deve ter entre {2} e {1} caracteres.")]
            [Display(Name = "Descrição Detalhada")]
            public string Descricao { get; set; } = string.Empty;

            [Required(ErrorMessage = "O preço é obrigatório.")]
            [Range(0.01, 1000000.00, ErrorMessage = "O preço deve ser superior a 0 €.")]
            [Display(Name = "Preço (€)")]
            public decimal Preco { get; set; }

            [Required(ErrorMessage = "O estado do anúncio é obrigatório.")]
            [Display(Name = "Estado do Anúncio")]
            public EstadoAnuncio Estado { get; set; }

            [Required(ErrorMessage = "O estado do produto é obrigatório.")]
            [Display(Name = "Estado do Produto")]
            public EstadoProduto EstadoProduto { get; set; }

            [Required(ErrorMessage = "A categoria é obrigatória.")]
            [Display(Name = "Categoria")]
            public int CategoriaFK { get; set; }

            [Display(Name = "URL de Imagem (opcional)")]
            public string? ImagemUrl { get; set; }

            [Display(Name = "Substituir Imagem Local")]
            public IFormFile? ImagemFicheiro { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            Anuncio = anuncio;

            // Check permissions
            bool isAuthorized = await CheckAuthorizationAsync(anuncio);
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "Não tens permissão para editar este anúncio.";
                return RedirectToPage("/Anuncios/Details", new { id = anuncio.Id });
            }

            // Populate input model
            Input.Id = anuncio.Id;
            Input.Titulo = anuncio.Titulo;
            Input.Descricao = anuncio.Descricao;
            Input.Preco = anuncio.Preco;
            Input.Estado = anuncio.Estado;
            Input.EstadoProduto = anuncio.EstadoProduto;
            Input.CategoriaFK = anuncio.CategoriaFK;

            var principalImagem = anuncio.Imagens.FirstOrDefault(i => i.Principal);
            if (principalImagem != null)
            {
                Input.ImagemUrl = principalImagem.CaminhoFicheiro;
            }

            await PopulateCategoriasDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // We need to reload Anuncio for the page template view rendering if form has validation errors
                var originalAnuncio = await _context.Anuncios
                    .Include(a => a.Imagens)
                    .Include(a => a.Vendedor)
                    .FirstOrDefaultAsync(m => m.Id == Input.Id);

                if (originalAnuncio == null)
                {
                    return NotFound();
                }

                Anuncio = originalAnuncio;
                await PopulateCategoriasDropdownAsync();
                return Page();
            }

            var anuncioToUpdate = await _context.Anuncios
                .Include(a => a.Imagens)
                .FirstOrDefaultAsync(a => a.Id == Input.Id);

            if (anuncioToUpdate == null)
            {
                return NotFound();
            }

            // Check permissions
            bool isAuthorized = await CheckAuthorizationAsync(anuncioToUpdate);
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "Não tens permissão para editar este anúncio.";
                return RedirectToPage("/Anuncios/Details", new { id = anuncioToUpdate.Id });
            }

            // Update fields
            anuncioToUpdate.Titulo = Input.Titulo;
            anuncioToUpdate.Descricao = Input.Descricao;
            anuncioToUpdate.Preco = Input.Preco;
            anuncioToUpdate.Estado = Input.Estado;
            anuncioToUpdate.EstadoProduto = Input.EstadoProduto;
            anuncioToUpdate.CategoriaFK = Input.CategoriaFK;

            // Handle Image updates
            if (Input.ImagemFicheiro != null && Input.ImagemFicheiro.Length > 0)
            {
                // Create uploads directory if not exists
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Input.ImagemFicheiro.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImagemFicheiro.CopyToAsync(fileStream);
                }

                string relativePath = "/uploads/" + uniqueFileName;

                // Update existing primary or add new one
                var primaryImage = anuncioToUpdate.Imagens.FirstOrDefault(i => i.Principal);
                if (primaryImage != null)
                {
                    primaryImage.CaminhoFicheiro = relativePath;
                }
                else
                {
                    var newImg = new Imagem
                    {
                        CaminhoFicheiro = relativePath,
                        Principal = true,
                        AnuncioFK = anuncioToUpdate.Id
                    };
                    _context.Imagens.Add(newImg);
                }
            }
            else if (!string.IsNullOrWhiteSpace(Input.ImagemUrl))
            {
                var primaryImage = anuncioToUpdate.Imagens.FirstOrDefault(i => i.Principal);
                if (primaryImage != null)
                {
                    primaryImage.CaminhoFicheiro = Input.ImagemUrl;
                }
                else
                {
                    var newImg = new Imagem
                    {
                        CaminhoFicheiro = Input.ImagemUrl,
                        Principal = true,
                        AnuncioFK = anuncioToUpdate.Id
                    };
                    _context.Imagens.Add(newImg);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "O anúncio '" + anuncioToUpdate.Titulo + "' foi atualizado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnuncioExists(anuncioToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Anuncios/Details", new { id = anuncioToUpdate.Id });
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.Id == id);
        }

        private async Task<bool> CheckAuthorizationAsync(Anuncio anuncio)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && (currentUser.Id == anuncio.VendedorFK || User.IsInRole("Admin")))
                {
                    return true;
                }
            }
            else
            {
                // Demomode fallback: allow if associated with the demo seller
                var demoSeller = await _userManager.FindByEmailAsync("demo@ipt.pt");
                if (demoSeller != null && anuncio.VendedorFK == demoSeller.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task PopulateCategoriasDropdownAsync()
        {
            var categorias = await _context.Categorias
                .Include(c => c.CategoriaPai)
                .OrderBy(c => c.CategoriaPaiFK != null ? c.CategoriaPai!.Nome : c.Nome)
                .ThenBy(c => c.Nome)
                .ToListAsync();

            var selectItems = categorias
                .Where(c => c.CategoriaPaiFK != null)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoriaPai!.Nome + " > " + c.Nome
                })
                .ToList();

            CategoriasList = new SelectList(selectItems, "Value", "Text");
        }
    }
}
