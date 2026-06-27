using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Admin
{
    public class ModeracaoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModeracaoModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<ApplicationUser> Utilizadores { get; set; } = new List<ApplicationUser>();
        public IList<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
        public bool IsAdminDemo { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            // Autorização: Se estiver logado, tem de ser Admin.
            // Se não estiver logado, ativamos o "Modo Admin Demo" para facilitar testes imediatos.
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                var isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "Acesso negado. Apenas administradores podem aceder a esta página.";
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                IsAdminDemo = true;
            }

            await LoadDataAsync();
            return Page();
        }

        private async Task LoadDataAsync()
        {
            // Carregar todos os utilizadores (exceto o admin principal para evitar auto-bloqueio)
            Utilizadores = await _context.Users
                .Where(u => u.Email != "admin@ipt.pt")
                .Include(u => u.Anuncios)
                .OrderBy(u => u.Nome)
                .ToListAsync();

            // Carregar todos os anúncios (ativos ou não)
            Anuncios = await _context.Anuncios
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();
        }

        // Toggles a user's Suspeito status
        public async Task<IActionResult> OnPostToggleSuspeitoAsync(string userId)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var curUser = await _userManager.GetUserAsync(User);
                if (curUser == null || !await _userManager.IsInRoleAsync(curUser, "Admin"))
                {
                    return Challenge();
                }
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.Suspeito = !user.Suspeito;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = user.Suspeito 
                    ? $"Utilizador '{user.Nome}' marcado como suspeito!" 
                    : $"Removida a suspeita sobre o utilizador '{user.Nome}'.";
            }

            return RedirectToPage();
        }

        // Safely removes a user account and cleans up all related SQL tables
        public async Task<IActionResult> OnPostRemoverContaAsync(string userId)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var curUser = await _userManager.GetUserAsync(User);
                if (curUser == null || !await _userManager.IsInRoleAsync(curUser, "Admin"))
                {
                    return Challenge();
                }
            }

            var user = await _context.Users
                .Include(u => u.Anuncios)
                    .ThenInclude(a => a.Imagens)
                .Include(u => u.Anuncios)
                    .ThenInclude(a => a.Favoritos)
                .Include(u => u.Anuncios)
                    .ThenInclude(a => a.Mensagens)
                .Include(u => u.Anuncios)
                    .ThenInclude(a => a.AnuncioTags)
                .Include(u => u.Favoritos)
                .Include(u => u.AvaliacoesFeitas)
                .Include(u => u.AvaliacoesRecebidas)
                .Include(u => u.MensagensEnviadas)
                .Include(u => u.MensagensRecebidas)
                .Include(u => u.VendedoresFavoritos)
                .Include(u => u.Seguidores)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                // 1. Remover Avaliações (enviadas e recebidas)
                _context.Avaliacoes.RemoveRange(user.AvaliacoesFeitas);
                _context.Avaliacoes.RemoveRange(user.AvaliacoesRecebidas);

                // 2. Remover Favoritos associados a este utilizador
                _context.Favoritos.RemoveRange(user.Favoritos);

                // 3. Remover relações de seguir vendedores
                _context.VendedoresFavoritos.RemoveRange(user.VendedoresFavoritos);
                _context.VendedoresFavoritos.RemoveRange(user.Seguidores);

                // 4. Remover Mensagens (enviadas e recebidas) no Chat
                _context.Mensagens.RemoveRange(user.MensagensEnviadas);
                _context.Mensagens.RemoveRange(user.MensagensRecebidas);

                // 5. Remover todos os anúncios do utilizador e as suas imagens/favoritos/mensagens
                foreach (var ad in user.Anuncios)
                {
                    _context.Imagens.RemoveRange(ad.Imagens);
                    _context.Favoritos.RemoveRange(ad.Favoritos);
                    _context.Mensagens.RemoveRange(ad.Mensagens);
                    _context.AnuncioTags.RemoveRange(ad.AnuncioTags);
                }
                _context.Anuncios.RemoveRange(user.Anuncios);

                // 6. Finalmente remover o utilizador do Identity
                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Conta de '{user.Nome}' e todos os seus dados foram permanentemente removidos!";
            }

            return RedirectToPage();
        }

        // Safely removes a product (anúncio)
        public async Task<IActionResult> OnPostRemoverProdutoAsync(int anuncioId)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var curUser = await _userManager.GetUserAsync(User);
                if (curUser == null || !await _userManager.IsInRoleAsync(curUser, "Admin"))
                {
                    return Challenge();
                }
            }

            var ad = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Favoritos)
                .Include(a => a.Mensagens)
                .Include(a => a.AnuncioTags)
                .FirstOrDefaultAsync(a => a.Id == anuncioId);

            if (ad != null)
            {
                // Limpeza segura dos registos dependentes
                _context.Imagens.RemoveRange(ad.Imagens);
                _context.Favoritos.RemoveRange(ad.Favoritos);
                _context.Mensagens.RemoveRange(ad.Mensagens);
                _context.AnuncioTags.RemoveRange(ad.AnuncioTags);

                _context.Anuncios.Remove(ad);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"O anúncio '{ad.Titulo}' foi eliminado com sucesso pela administração!";
            }

            return RedirectToPage();
        }
    }
}
