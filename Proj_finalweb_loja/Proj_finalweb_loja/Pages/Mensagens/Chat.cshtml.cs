using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Pages.Mensagens
{
    public class ChatModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IList<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
        public ApplicationUser CurrentUser { get; set; } = null!;
        public ApplicationUser Recipient { get; set; } = null!;
        public Anuncio Anuncio { get; set; } = null!;

        [BindProperty]
        [Required(ErrorMessage = "Não podes enviar uma mensagem vazia.")]
        [StringLength(1000, ErrorMessage = "A mensagem não pode exceder {1} caracteres.")]
        public string NovoConteudo { get; set; } = string.Empty;

        [BindProperty]
        public int AnuncioId { get; set; }

        [BindProperty]
        public string RecipientId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int anuncioId, string destinatarioId)
        {
            // 1. Get current logged in user (or fallback to test user)
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
                // Demo fallback: default to João Pereira (buyer of most ads)
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null)
                {
                    TempData["ErrorMessage"] = "Crie contas ou inicie sessão para usar o chat.";
                    return RedirectToPage("/Index");
                }
                CurrentUser = demoUser;
                currentUserId = demoUser.Id;
            }

            // Prevent chatting with oneself
            if (currentUserId == destinatarioId)
            {
                TempData["ErrorMessage"] = "Não podes iniciar uma conversa contigo próprio.";
                return RedirectToPage("/Anuncios/Details", new { id = anuncioId });
            }

            // 2. Fetch Anuncio
            var anuncio = await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(a => a.Id == anuncioId);

            if (anuncio == null)
            {
                return NotFound();
            }
            Anuncio = anuncio;
            AnuncioId = anuncio.Id;

            // 3. Fetch Recipient
            var recipient = await _userManager.FindByIdAsync(destinatarioId);
            if (recipient == null)
            {
                return NotFound();
            }
            Recipient = recipient;
            RecipientId = recipient.Id;

            // 4. Fetch Message Thread
            Mensagens = await _context.Mensagens
                .Include(m => m.Remetente)
                .Include(m => m.Destinatario)
                .Where(m => m.AnuncioFK == anuncioId &&
                           ((m.RemetenteFK == currentUserId && m.DestinatarioFK == destinatarioId) ||
                            (m.RemetenteFK == destinatarioId && m.DestinatarioFK == currentUserId)))
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();

            // 5. Mark incoming messages as read
            var unreadMessages = Mensagens
                .Where(m => m.DestinatarioFK == currentUserId && !m.Lida)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.Lida = true;
                }
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Get current logged in user (or fallback to test user)
            string currentUserId;
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                currentUserId = user.Id;
            }
            else
            {
                var demoUser = await _userManager.FindByEmailAsync("joao@ipt.pt");
                if (demoUser == null) return RedirectToPage("/Index");
                currentUserId = demoUser.Id;
            }

            if (!ModelState.IsValid)
            {
                // Reload data to display page with errors
                var anuncio = await _context.Anuncios.Include(a => a.Imagens).FirstOrDefaultAsync(a => a.Id == AnuncioId);
                var recipient = await _userManager.FindByIdAsync(RecipientId);
                if (anuncio == null || recipient == null) return NotFound();

                Anuncio = anuncio;
                Recipient = recipient;
                CurrentUser = await _userManager.FindByIdAsync(currentUserId) ?? new ApplicationUser();

                Mensagens = await _context.Mensagens
                    .Where(m => m.AnuncioFK == AnuncioId &&
                               ((m.RemetenteFK == currentUserId && m.DestinatarioFK == RecipientId) ||
                                (m.RemetenteFK == RecipientId && m.DestinatarioFK == currentUserId)))
                    .OrderBy(m => m.DataEnvio)
                    .ToListAsync();

                return Page();
            }

            // 2. Insert new message
            var novaMensagem = new Mensagem
            {
                Conteudo = NovoConteudo.Trim(),
                RemetenteFK = currentUserId,
                DestinatarioFK = RecipientId,
                AnuncioFK = AnuncioId,
                DataEnvio = DateTime.UtcNow,
                Lida = false
            };

            _context.Mensagens.Add(novaMensagem);
            await _context.SaveChangesAsync();

            // Broadcast message via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", currentUserId, RecipientId, AnuncioId, novaMensagem.Conteudo);

            var isWidget = Request.Query["isWidget"] == "true";
            return RedirectToPage("./Chat", new { anuncioId = AnuncioId, destinatarioId = RecipientId, isWidget = isWidget ? "true" : null });
        }
    }
}
