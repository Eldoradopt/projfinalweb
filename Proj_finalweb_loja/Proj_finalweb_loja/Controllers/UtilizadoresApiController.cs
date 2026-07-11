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
    }
}
