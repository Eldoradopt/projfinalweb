using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;

namespace Proj_finalweb_loja.Controllers
{
    [ApiController]
    [Route("api/favoritos")]
    [Authorize] // Todos os endpoints aqui requerem login
    public class FavoritosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de anúncios guardados nos favoritos do utilizador autenticado.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoritoDto>>> GetMeusFavoritos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var favoritos = await _context.Favoritos
                .Include(f => f.Anuncio)
                .Where(f => f.UtilizadorFK == userId)
                .OrderByDescending(f => f.DataAdicionado)
                .Select(f => new FavoritoDto
                {
                    Id = f.Id,
                    AnuncioId = f.AnuncioFK,
                    AnuncioTitulo = f.Anuncio != null ? f.Anuncio.Titulo : "Desconhecido",
                    Preco = f.Anuncio != null ? f.Anuncio.Preco : 0,
                    DataAdicionado = f.DataAdicionado
                })
                .ToListAsync();

            return Ok(favoritos);
        }

        /// <summary>
        /// Adiciona um anúncio aos favoritos.
        /// </summary>
        [HttpPost("{anuncioId}")]
        public async Task<IActionResult> AdicionarFavorito(int anuncioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Verifica se o anúncio existe
            var anuncioExiste = await _context.Anuncios.AnyAsync(a => a.Id == anuncioId);
            if (!anuncioExiste)
                return NotFound(new { mensagem = "Anúncio não encontrado." });

            // Verifica se já está nos favoritos
            var jaExiste = await _context.Favoritos.AnyAsync(f => f.AnuncioFK == anuncioId && f.UtilizadorFK == userId);
            if (jaExiste)
                return BadRequest(new { mensagem = "Este anúncio já se encontra nos seus favoritos." });

            var favorito = new Favorito
            {
                AnuncioFK = anuncioId,
                UtilizadorFK = userId,
                DataAdicionado = DateTime.UtcNow
            };

            _context.Favoritos.Add(favorito);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Anúncio adicionado aos favoritos com sucesso." });
        }

        /// <summary>
        /// Remove um anúncio da lista de favoritos.
        /// </summary>
        [HttpDelete("{anuncioId}")]
        public async Task<IActionResult> RemoverFavorito(int anuncioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.AnuncioFK == anuncioId && f.UtilizadorFK == userId);

            if (favorito == null)
                return NotFound(new { mensagem = "Favorito não encontrado." });

            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Removido dos favoritos com sucesso." });
        }
    }
}
