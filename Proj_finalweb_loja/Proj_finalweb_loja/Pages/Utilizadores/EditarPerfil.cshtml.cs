using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Utilizadores
{
    public class EditarPerfilModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditarPerfilModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public IFormFile? FotoPerfil { get; set; }

        public string? FotoAtual { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
            [Display(Name = "Nome Completo")]
            public string Nome { get; set; } = string.Empty;

            [Phone(ErrorMessage = "Número de telemóvel inválido.")]
            [Display(Name = "Telemóvel")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Cidade")]
            public string? Cidade { get; set; }

            [Display(Name = "Morada")]
            public string? Morada { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            Input = new InputModel
            {
                Nome = user.Nome,
                PhoneNumber = user.PhoneNumber,
                Cidade = user.Cidade,
                Morada = user.Morada
            };

            FotoAtual = user.FotoPerfilPath;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid)
            {
                FotoAtual = user.FotoPerfilPath;
                return Page();
            }

            user.Nome = Input.Nome;
            user.PhoneNumber = Input.PhoneNumber;
            user.Cidade = Input.Cidade;
            user.Morada = Input.Morada;

            if (FotoPerfil != null)
            {
                // Verify directory exists
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // File name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(FotoPerfil.FileName)}";
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FotoPerfil.CopyToAsync(fileStream);
                }

                // Delete old photo if it exists and is local
                if (!string.IsNullOrWhiteSpace(user.FotoPerfilPath) && user.FotoPerfilPath.StartsWith("/images/users/"))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.FotoPerfilPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch { /* ignore */ }
                    }
                }

                user.FotoPerfilPath = $"/images/users/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                return RedirectToPage("/Utilizadores/Perfil", new { id = user.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            FotoAtual = user.FotoPerfilPath;
            return Page();
        }
    }
}
