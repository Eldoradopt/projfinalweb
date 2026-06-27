using System;
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
    [Route("api/anuncios")]
    public class AnunciosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnunciosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de anúncios ativos e disponíveis.
        /// Suporta filtros opcionais de pesquisa e categoria.
        /// </summary>
        /// <param name="pesquisa">Termo de pesquisa opcional.</param>
        /// <param name="categoriaId">ID de categoria opcional.</param>
        /// <returns>Lista de anúncios ativos.</returns>
        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> GetAnunciosAtivos([FromQuery] string? pesquisa = null, [FromQuery] int? categoriaId = null)
        {
            var query = _context.Anuncios
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .Include(a => a.Imagens)
                .Where(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.ToLower();
                query = query.Where(a => a.Titulo.ToLower().Contains(pesquisa) || a.Descricao.ToLower().Contains(pesquisa));
            }

            if (categoriaId.HasValue)
            {
                query = query.Where(a => a.CategoriaFK == categoriaId.Value);
            }

            var anuncios = await query
                .OrderByDescending(a => a.DataPublicacao)
                .Select(a => new AnuncioDto
                {
                    Id = a.Id,
                    Titulo = a.Titulo,
                    Descricao = a.Descricao,
                    Preco = a.Preco,
                    Estado = a.Estado.ToString(),
                    EstadoProduto = a.EstadoProduto.ToString(),
                    DataPublicacao = a.DataPublicacao,
                    VendedorId = a.VendedorFK,
                    VendedorNome = a.Vendedor != null ? a.Vendedor.Nome : "Desconhecido",
                    CategoriaNome = a.Categoria != null ? a.Categoria.Nome : "Sem Categoria",
                    ImagemPrincipal = a.Imagens.FirstOrDefault(i => i.Principal) != null 
                        ? a.Imagens.FirstOrDefault(i => i.Principal)!.CaminhoFicheiro 
                        : (a.Imagens.FirstOrDefault() != null ? a.Imagens.FirstOrDefault()!.CaminhoFicheiro : null)
                })
                .ToListAsync();

            return Ok(anuncios);
        }
    }
}
