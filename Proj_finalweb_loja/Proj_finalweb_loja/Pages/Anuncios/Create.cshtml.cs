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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [BindProperty]
        public AnuncioInputModel Input { get; set; } = new AnuncioInputModel();

        public SelectList CategoriasList { get; set; } = null!;

        public class AnuncioInputModel
        {
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

            [Required(ErrorMessage = "O estado do produto é obrigatório.")]
            [Display(Name = "Estado do Produto")]
            public EstadoProduto EstadoProduto { get; set; } = EstadoProduto.BomEstado;

            [Required(ErrorMessage = "A categoria é obrigatória.")]
            [Display(Name = "Categoria")]
            public int CategoriaFK { get; set; }

            [Display(Name = "URL de Imagem (opcional)")]
            public string? ImagemUrl { get; set; }

            [Display(Name = "Carregar Imagem Local")]
            public IFormFile? ImagemFicheiro { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateCategoriasDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriasDropdownAsync();
                return Page();
            }

            // 1. Determine Seller ID
            string vendedorId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                vendedorId = user?.Id ?? throw new InvalidOperationException("Utilizador autenticado não encontrado.");
            }
            else
            {
                // FALLBACK FOR TESTING: If not logged in, associate with "demo@ipt.pt" automatically!
                var demoSeller = await _userManager.FindByEmailAsync("demo@ipt.pt");
                if (demoSeller == null)
                {
                    ModelState.AddModelError(string.Empty, "O utilizador Demo não foi encontrado. Por favor corra a aplicação para semear os dados.");
                    await PopulateCategoriasDropdownAsync();
                    return Page();
                }
                vendedorId = demoSeller.Id;
            }

            // 2. Create the Announcement
            var anuncio = new Anuncio
            {
                Titulo = Input.Titulo,
                Descricao = Input.Descricao,
                Preco = Input.Preco,
                Estado = EstadoAnuncio.Disponivel,
                EstadoProduto = Input.EstadoProduto,
                VendedorFK = vendedorId,
                CategoriaFK = Input.CategoriaFK,
                DataPublicacao = DateTime.UtcNow,
                Ativo = true
            };

            _context.Anuncios.Add(anuncio);
            await _context.SaveChangesAsync();

            // 3. Handle Image Saving
            string imagePath = "https://images.unsplash.com/photo-1531403009284-440f080d1e12?auto=format&fit=crop&w=600&q=80"; // Default stock image

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

                // Relative path to store in db
                imagePath = "/uploads/" + uniqueFileName;
            }
            else if (!string.IsNullOrWhiteSpace(Input.ImagemUrl))
            {
                imagePath = Input.ImagemUrl;
            }

            var imagem = new Imagem
            {
                CaminhoFicheiro = imagePath,
                Principal = true,
                AnuncioFK = anuncio.Id
            };

            _context.Imagens.Add(imagem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "O teu anúncio '" + anuncio.Titulo + "' foi criado com sucesso!";
            return RedirectToPage("/Index");
        }

        private async Task PopulateCategoriasDropdownAsync()
        {
            var categorias = await _context.Categorias
                .Include(c => c.CategoriaPai)
                .OrderBy(c => c.CategoriaPaiFK != null ? c.CategoriaPai!.Nome : c.Nome)
                .ThenBy(c => c.Nome)
                .ToListAsync();

            // Format category names beautifully to show hierarchy: "Eletrónica > Telemóveis"
            var selectItems = categorias
                .Where(c => c.CategoriaPaiFK != null) // Only allow picking subcategories
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
