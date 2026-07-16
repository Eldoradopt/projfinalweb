using System;
using System.Collections.Generic;

namespace Proj_finalweb_loja.Data.Model
{
    public class AnuncioDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string EstadoProduto { get; set; } = string.Empty;
        public DateTime DataPublicacao { get; set; }
        public string VendedorId { get; set; } = string.Empty;
        public string VendedorNome { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string? ImagemPrincipal { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class UtilizadorDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Cidade { get; set; }
        public string? FotoPerfilPath { get; set; }
        public DateTime DataRegisto { get; set; }
        public int TotalAnunciosAtivos { get; set; }
    }
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Icone { get; set; }
        public int TotalAnunciosAtivos { get; set; }
    }

    public class AvaliacaoDto
    {
        public int Id { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
        public DateTime DataAvaliacao { get; set; }
        public string AvaliadorId { get; set; } = string.Empty;
        public string AvaliadorNome { get; set; } = string.Empty;
    }

    public class CriarAvaliacaoDto
    {
        public string AvaliandoId { get; set; } = string.Empty;
        public int Nota { get; set; }
        public string? Comentario { get; set; }
    }

    public class FavoritoDto
    {
        public int Id { get; set; }
        public int AnuncioId { get; set; }
        public string AnuncioTitulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public DateTime DataAdicionado { get; set; }
    }

    public class CriarAnuncioDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public string EstadoProduto { get; set; } = "Novo"; // Novo, Usado, etc
    }
}
