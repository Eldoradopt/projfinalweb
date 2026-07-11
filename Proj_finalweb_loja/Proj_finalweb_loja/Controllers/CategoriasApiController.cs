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
    [Route("api/categorias")]
    public class CategoriasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as categorias com a contagem de anúncios ativos em cada uma.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Icone = c.Icone,
                    TotalAnunciosAtivos = c.Anuncios.Count(a => a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
                })
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return Ok(categorias);
        }

        /// <summary>
        /// Obtém os anúncios ativos para uma categoria específica.
        /// </summary>
        [HttpGet("{id}/anuncios")]
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> GetAnunciosPorCategoria(int id)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == id);
            if (!categoriaExiste)
            {
                return NotFound(new { mensagem = "Categoria não encontrada." });
            }

            var anuncios = await _context.Anuncios
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .Include(a => a.Imagens)
                .Where(a => a.CategoriaFK == id && a.Ativo && a.Estado == EstadoAnuncio.Disponivel)
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
