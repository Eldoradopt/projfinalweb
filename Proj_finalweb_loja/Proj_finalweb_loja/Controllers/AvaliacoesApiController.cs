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
    [Route("api/avaliacoes")]
    public class AvaliacoesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém as avaliações de um vendedor específico e a sua média geral.
        /// </summary>
        [HttpGet("{vendedorId}")]
        public async Task<IActionResult> GetAvaliacoesVendedor(string vendedorId)
        {
            var avaliacoes = await _context.Avaliacoes
                .Include(a => a.Avaliador)
                .Where(a => a.AvaliandoFK == vendedorId)
                .OrderByDescending(a => a.DataAvaliacao)
                .Select(a => new AvaliacaoDto
                {
                    Id = a.Id,
                    Nota = a.Nota,
                    Comentario = a.Comentario,
                    DataAvaliacao = a.DataAvaliacao,
                    AvaliadorId = a.AvaliadorFK,
                    AvaliadorNome = a.Avaliador != null ? a.Avaliador.Nome : "Anónimo"
                })
                .ToListAsync();

            var media = avaliacoes.Any() ? avaliacoes.Average(a => a.Nota) : 0;

            return Ok(new
            {
                TotalAvaliacoes = avaliacoes.Count,
                MediaClassificacao = Math.Round(media, 1),
                Avaliacoes = avaliacoes
            });
        }

        /// <summary>
        /// Submete uma nova avaliação para um utilizador. Requer autenticação.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarAvaliacao([FromBody] CriarAvaliacaoDto dto)
        {
            try
            {
                var avaliadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (avaliadorId == null)
                    return Unauthorized(new { mensagem = "Utilizador não autenticado." });

                if (avaliadorId == dto.AvaliandoId)
                    return BadRequest(new { mensagem = "Não pode avaliar-se a si próprio." });

                var existeAvaliado = await _context.Users.AnyAsync(u => u.Id == dto.AvaliandoId);
                if (!existeAvaliado)
                    return NotFound(new { mensagem = "O utilizador que pretende avaliar não existe." });

                // Regra extra: evitar múltiplas avaliações do mesmo utilizador no mesmo dia (opcional, mas demonstra cuidado)
                var jaAvaliouHoje = await _context.Avaliacoes.AnyAsync(a => 
                    a.AvaliadorFK == avaliadorId && 
                    a.AvaliandoFK == dto.AvaliandoId && 
                    a.DataAvaliacao.Date == DateTime.UtcNow.Date);

                if (jaAvaliouHoje)
                    return BadRequest(new { mensagem = "Já avaliou este vendedor hoje." });

                var novaAvaliacao = new Avaliacao
                {
                    Nota = dto.Nota,
                    Comentario = dto.Comentario,
                    AvaliandoFK = dto.AvaliandoId,
                    AvaliadorFK = avaliadorId,
                    DataAvaliacao = DateTime.UtcNow
                };

                _context.Avaliacoes.Add(novaAvaliacao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAvaliacoesVendedor), new { vendedorId = dto.AvaliandoId }, new { mensagem = "Avaliação submetida com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao submeter avaliação.", detalhe = ex.Message });
            }
        }
    }
}
