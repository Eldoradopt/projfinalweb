using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Mensagens
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<ChatThreadViewModel> Threads { get; set; } = new List<ChatThreadViewModel>();
        public ApplicationUser CurrentUser { get; set; } = null!;

        public class ChatThreadViewModel
        {
            public Anuncio? Anuncio { get; set; }
            public ApplicationUser? OtherUser { get; set; }
            public Mensagem LastMessage { get; set; } = null!;
            public int UnreadCount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Get current logged in user (or fallback to Maria to view chats, as Maria is seller/buyer)
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                CurrentUser = user;
                currentUserId = user.Id;
            }
            else
            {
                // Demo fallback: default to João to let user test chat threads out of the box!
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null)
                {
                    TempData["ErrorMessage"] = "Por favor crie contas ou inicie sessão.";
                    return RedirectToPage("/Index");
                }
                CurrentUser = demoUser;
                currentUserId = demoUser.Id;
            }

            // 2. Fetch all messages involving the current user
            var messages = await _context.Mensagens
                .Include(m => m.Anuncio)
                    .ThenInclude(a => a!.Imagens)
                .Include(m => m.Remetente)
                .Include(m => m.Destinatario)
                .Where(m => m.RemetenteFK == currentUserId || m.DestinatarioFK == currentUserId)
                .OrderByDescending(m => m.DataEnvio)
                .ToListAsync();

            // 3. Group messages into chat threads by (AnuncioId, OtherUserId)
            Threads = messages
                .GroupBy(m => new { 
                    AnuncioId = m.AnuncioFK, 
                    OtherUserId = m.RemetenteFK == currentUserId ? m.DestinatarioFK : m.RemetenteFK 
                })
                .Select(g => {
                    var lastMsg = g.First();
                    var otherUser = lastMsg.RemetenteFK == currentUserId ? lastMsg.Destinatario : lastMsg.Remetente;
                    return new ChatThreadViewModel
                    {
                        Anuncio = lastMsg.Anuncio,
                        OtherUser = otherUser,
                        LastMessage = lastMsg,
                        UnreadCount = g.Count(m => m.DestinatarioFK == currentUserId && !m.Lida)
                    };
                })
                .ToList();

            return Page();
        }

        public async Task<JsonResult> OnGetUnreadCountAsync()
        {
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                currentUserId = user?.Id ?? "";
            }
            else
            {
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                currentUserId = demoUser?.Id ?? "";
            }

            if (string.IsNullOrEmpty(currentUserId))
            {
                return new JsonResult(new { count = 0 });
            }

            var unreadCount = await _context.Mensagens
                .CountAsync(m => m.DestinatarioFK == currentUserId && !m.Lida);

            return new JsonResult(new { count = unreadCount });
        }
    }
}
