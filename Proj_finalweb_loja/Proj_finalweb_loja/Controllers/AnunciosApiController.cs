using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnunciosApiController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Obtém todos os anúncios.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> GetAnuncios()
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.Vendedor)
                .Include(a => a.Categoria)
                .Include(a => a.Imagens)
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

        /// <summary>
        /// Obtém os detalhes de um anúncio específico.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AnuncioDto>> GetAnuncio(int id)
        {
            var a = await _context.Anuncios
                .Include(an => an.Vendedor)
                .Include(an => an.Categoria)
                .Include(an => an.Imagens)
                .FirstOrDefaultAsync(an => an.Id == id);

            if (a == null)
            {
                return NotFound(new { mensagem = "Anúncio não encontrado." });
            }

            var dto = new AnuncioDto
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
            };

            return Ok(dto);
        }

        /// <summary>
        /// Obtém a lista de anúncios ativos e disponíveis.
        /// Suporta filtros opcionais de pesquisa e categoria, com paginação.
        /// </summary>
        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> GetAnunciosAtivos([FromQuery] string? pesquisa = null, [FromQuery] int? categoriaId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
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

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var anuncios = await query
                .OrderByDescending(a => a.DataPublicacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        /// <summary>
        /// Remove um anúncio e os seus ficheiros físicos e registos na base de dados (Imagens, Favoritos, etc).
        /// Requer autenticação e autorização (Dono ou Admin).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnuncio(int id)
        {
            try
            {
                var anuncio = await _context.Anuncios
                    .Include(a => a.Imagens)
                    .Include(a => a.Favoritos)
                    .Include(a => a.Mensagens)
                    .Include(a => a.AnuncioTags)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (anuncio == null)
                {
                    return NotFound(new { mensagem = "Anúncio não encontrado." });
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (anuncio.VendedorFK != currentUserId && !isAdmin)
                {
                    return Forbid(); // Retorna 403 Forbidden se não for dono nem admin
                }

                // 1. Apagar ficheiros físicos de imagens associados ao anúncio
                foreach (var img in anuncio.Imagens)
                {
                    if (!string.IsNullOrEmpty(img.CaminhoFicheiro))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.CaminhoFicheiro.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                // 2. Limpar dependências na Base de Dados
                _context.Imagens.RemoveRange(anuncio.Imagens);
                _context.Favoritos.RemoveRange(anuncio.Favoritos);
                _context.Mensagens.RemoveRange(anuncio.Mensagens);
                _context.AnuncioTags.RemoveRange(anuncio.AnuncioTags);

                // 3. Remover o anúncio em si
                _context.Anuncios.Remove(anuncio);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Anúncio e todos os ficheiros de imagem associados foram removidos com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Ocorreu um erro interno ao processar a eliminação.", detalhe = ex.Message });
            }
        }
        /// <summary>
        /// Cria um novo anúncio. Requer autenticação.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarAnuncio([FromBody] CriarAnuncioDto dto)
        {
            try
            {
                var vendedorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (vendedorId == null)
                    return Unauthorized(new { mensagem = "Utilizador não autenticado." });

                var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
                if (!categoriaExiste)
                    return BadRequest(new { mensagem = "A categoria especificada não existe." });

                if (!Enum.TryParse<EstadoProduto>(dto.EstadoProduto, out var estadoProd))
                {
                    estadoProd = Data.Model.EstadoProduto.Novo; // default
                }

                var novoAnuncio = new Anuncio
                {
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    Preco = dto.Preco,
                    CategoriaFK = dto.CategoriaId,
                    VendedorFK = vendedorId,
                    DataPublicacao = DateTime.UtcNow,
                    Estado = EstadoAnuncio.Disponivel,
                    Ativo = true,
                    EstadoProduto = estadoProd
                };

                _context.Anuncios.Add(novoAnuncio);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAnuncio), new { id = novoAnuncio.Id }, new { mensagem = "Anúncio criado com sucesso.", id = novoAnuncio.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Ocorreu um erro interno ao criar o anúncio.", detalhe = ex.Message });
            }
        }
    }
}
