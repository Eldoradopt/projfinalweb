using System;

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
}
