using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Utilizadores
{
    public class PerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PerfilModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser Vendedor { get; set; } = null!;
        public IList<Anuncio> AnunciosAtivos { get; set; } = new List<Anuncio>();
        public IList<Anuncio> VendasConcluidas { get; set; } = new List<Anuncio>();
        public IList<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
        public double MediaAvaliacoes { get; set; }
        public bool JaAvaliou { get; set; } = false;
        public bool IsOwnProfile { get; set; } = false;
        public bool IsFavorito { get; set; } = false;

        [BindProperty]
        public RatingInputModel RatingInput { get; set; } = new();

        public class RatingInputModel
        {
            [Required]
            public string VendedorId { get; set; } = string.Empty;

            [Required(ErrorMessage = "A nota é obrigatória.")]
            [Range(1, 5, ErrorMessage = "Seleciona entre 1 e 5 estrelas.")]
            public int Nota { get; set; }

            [StringLength(500)]
            [Display(Name = "Comentário (opcional)")]
            public string? Comentario { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            Vendedor = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id) ?? null!;

            if (Vendedor == null) return NotFound();

            AnunciosAtivos = await _context.Anuncios
                .Where(a => a.VendedorFK == id && a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
                .Include(a => a.Imagens)
                .Include(a => a.Categoria)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            VendasConcluidas = await _context.Anuncios
                .Where(a => a.VendedorFK == id && a.Estado == EstadoAnuncio.Vendido)
                .Include(a => a.Imagens)
                .Include(a => a.Categoria)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            Avaliacoes = await _context.Avaliacoes
                .Where(a => a.AvaliandoFK == id)
                .Include(a => a.Avaliador)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            MediaAvaliacoes = Avaliacoes.Any() ? Avaliacoes.Average(a => a.Nota) : 0;

            // Check if logged-in user already rated this seller
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                IsOwnProfile = currentUser.Id == id;
                JaAvaliou = await _context.Avaliacoes
                    .AnyAsync(a => a.AvaliadorFK == currentUser.Id && a.AvaliandoFK == id);
                IsFavorito = await _context.VendedoresFavoritos
                    .AnyAsync(vf => vf.SeguidorFK == currentUser.Id && vf.VendedorFK == id);
            }

            RatingInput.VendedorId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid)
                return await ReloadPageAsync(RatingInput.VendedorId);

            // Prevent self-rating
            if (currentUser.Id == RatingInput.VendedorId)
            {
                ModelState.AddModelError("", "Não podes avaliar o teu próprio perfil.");
                return await ReloadPageAsync(RatingInput.VendedorId);
            }

            // Prevent duplicate rating
            bool jaAvaliou = await _context.Avaliacoes
                .AnyAsync(a => a.AvaliadorFK == currentUser.Id && a.AvaliandoFK == RatingInput.VendedorId);

            if (jaAvaliou)
            {
                ModelState.AddModelError("", "Já avaliaste este vendedor.");
                return await ReloadPageAsync(RatingInput.VendedorId);
            }

            var avaliacao = new Avaliacao
            {
                Nota = RatingInput.Nota,
                Comentario = RatingInput.Comentario,
                AvaliadorFK = currentUser.Id,
                AvaliandoFK = RatingInput.VendedorId,
                DataAvaliacao = DateTime.UtcNow
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Avaliação submetida com sucesso!";
            return RedirectToPage("/Utilizadores/Perfil", new { id = RatingInput.VendedorId });
        }

        public async Task<IActionResult> OnPostToggleFavoritoAsync(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (currentUser.Id == id)
            {
                TempData["ErrorMessage"] = "Não podes adicionar-te a ti mesmo aos favoritos.";
                return RedirectToPage("/Utilizadores/Perfil", new { id });
            }

            var favorito = await _context.VendedoresFavoritos
                .FirstOrDefaultAsync(vf => vf.SeguidorFK == currentUser.Id && vf.VendedorFK == id);

            if (favorito != null)
            {
                _context.VendedoresFavoritos.Remove(favorito);
                TempData["SuccessMessage"] = "Vendedor removido dos favoritos.";
            }
            else
            {
                var novoFavorito = new VendedorFavorito
                {
                    SeguidorFK = currentUser.Id,
                    VendedorFK = id,
                    DataAdicionado = DateTime.UtcNow
                };
                _context.VendedoresFavoritos.Add(novoFavorito);
                TempData["SuccessMessage"] = "Vendedor adicionado aos favoritos!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Utilizadores/Perfil", new { id });
        }

        public async Task<IActionResult> OnPostRemoverVendaAsync(int anuncioId, string vendedorId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (currentUser.Id != vendedorId)
            {
                TempData["ErrorMessage"] = "Não tens permissão para remover esta venda.";
                return RedirectToPage("/Utilizadores/Perfil", new { id = vendedorId });
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Imagens) // include child references if needed
                .FirstOrDefaultAsync(a => a.Id == anuncioId && a.VendedorFK == vendedorId);

            if (anuncio != null)
            {
                _context.Anuncios.Remove(anuncio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venda removida com sucesso!";
            }

            return RedirectToPage("/Utilizadores/Perfil", new { id = vendedorId });
        }

        private async Task<IActionResult> ReloadPageAsync(string id)
        {
            await OnGetAsync(id);
            return Page();
        }
    }
}
