using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;

namespace Proj_finalweb_loja.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string destinatarioId, int anuncioId, string message)
        {
            var remetenteId = Context.UserIdentifier;
            
            if (string.IsNullOrEmpty(remetenteId))
            {
                throw new HubException("Utilizador não autenticado.");
            }

            // Se o anuncioId for 0, vai buscar o primeiro anúncio qualquer da BD para não quebrar a FK
            if (anuncioId == 0)
            {
                var anuncioSuplente = _context.Anuncios.FirstOrDefault();
                if (anuncioSuplente != null)
                {
                    anuncioId = anuncioSuplente.Id;
                }
            }

            // Se o destinatarioId vier vazio ou for igual ao remetente, envia para o admin ou para outro user existente
            if (string.IsNullOrEmpty(destinatarioId) || destinatarioId == remetenteId)
            {
                var outroUtilizador = _context.Users.FirstOrDefault(u => u.Id != remetenteId);
                if (outroUtilizador != null)
                {
                    destinatarioId = outroUtilizador.Id;
                }
            }

            var novaMensagem = new Mensagem
            {
                Conteudo = message,
                RemetenteFK = remetenteId,
                DestinatarioFK = destinatarioId,
                AnuncioFK = anuncioId,
                DataEnvio = DateTime.UtcNow,
                Lida = false
            };

            _context.Mensagens.Add(novaMensagem);
            await _context.SaveChangesAsync();

            // Broadcast full info so the Chat page can filter by conversation
            await Clients.All.SendAsync("ReceiveMessage", remetenteId, destinatarioId, anuncioId, message);

            // Notify recipient of unread badge
            if (!string.IsNullOrEmpty(destinatarioId))
            {
                await Clients.User(destinatarioId).SendAsync("ReceiveUnreadNotification");
            }
        }
    }
}