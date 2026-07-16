using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data;
using Proj_finalweb_loja.Data.Model;

namespace Proj_finalweb_loja.Controllers
{
    [ApiController]
    [Route("api/utilizadores")]
    public class UtilizadoresApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém os detalhes públicos de um utilizador registado.
        /// </summary>
        /// <param name="id">ID do utilizador.</param>
        /// <returns>Detalhes do utilizador.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UtilizadorDto>> GetUtilizadorDetalhes(string id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Anuncios)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(new { mensagem = "Utilizador não encontrado." });
                }

                var totalAnunciosAtivos = user.Anuncios.Count(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel);

                var dto = new UtilizadorDto
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Cidade = user.Cidade,
                    FotoPerfilPath = user.FotoPerfilPath,
                    DataRegisto = user.DataRegisto,
                    TotalAnunciosAtivos = totalAnunciosAtivos
                };

                return Ok(dto);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { mensagem = "Ocorreu um erro interno ao processar o pedido.", detalhe = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a lista dos vendedores com mais anúncios ativos.
        /// </summary>
        /// <param name="limit">Número máximo de vendedores a devolver (default: 10).</param>
        /// <returns>Lista de vendedores em destaque.</returns>
        [HttpGet("destaque")]
        [ResponseCache(Duration = 120)]
        public async Task<ActionResult<IEnumerable<UtilizadorDto>>> GetVendedoresDestaque([FromQuery] int limit = 10)
        {
            try
            {
                if (limit < 1) limit = 10;
                if (limit > 50) limit = 50;

                var users = await _context.Users
                    .Include(u => u.Anuncios)
                    .Where(u => u.Anuncios.Any(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel))
                    .ToListAsync();

                var topSellers = users
                    .Select(u => new UtilizadorDto
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        Cidade = u.Cidade,
                        FotoPerfilPath = u.FotoPerfilPath,
                        DataRegisto = u.DataRegisto,
                        TotalAnunciosAtivos = u.Anuncios.Count(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
                    })
                    .OrderByDescending(u => u.TotalAnunciosAtivos)
                    .Take(limit)
                    .ToList();

                return Ok(topSellers);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { mensagem = "Ocorreu um erro interno ao processar o pedido.", detalhe = ex.Message });
            }
        }
    }
}
