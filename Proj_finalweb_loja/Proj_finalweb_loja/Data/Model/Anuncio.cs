using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Enumeração para representar o estado atual de publicação do anúncio.
    /// </summary>
    public enum EstadoAnuncio
    {
        /// <summary>
        /// O artigo está disponível para compra e listado no catálogo.
        /// </summary>
        [Display(Name = "Disponível")]
        Disponivel,

        /// <summary>
        /// O artigo foi reservado temporariamente por um comprador.
        /// </summary>
        [Display(Name = "Reservado")]
        Reservado,

        /// <summary>
        /// O negócio foi concluído e o produto foi vendido.
        /// </summary>
        [Display(Name = "Vendido")]
        Vendido
    }

    /// <summary>
    /// Enumeração para representar a condição de desgaste físico do produto à venda.
    /// </summary>
    public enum EstadoProduto
    {
        /// <summary>
        /// Produto novo, na caixa original ou com etiquetas seladas.
        /// </summary>
        [Display(Name = "Novo")]
        Novo,

        /// <summary>
        /// Produto sem marcas de uso, como se tivesse saído da loja.
        /// </summary>
        [Display(Name = "Como Novo")]
        ComoNovo,

        /// <summary>
        /// Produto com pequenos riscos ou desgaste ligeiro e normal.
        /// </summary>
        [Display(Name = "Em Bom Estado")]
        BomEstado,

        /// <summary>
        /// Produto com marcas evidentes de utilização, mas 100% operacional.
        /// </summary>
        [Display(Name = "Usado")]
        Usado,

        /// <summary>
        /// Produto danificado ou com avarias, ideal para aproveitamento de componentes.
        /// </summary>
        [Display(Name = "Para Peças")]
        ParaPecas
    }

    /// <summary>
    /// Classe para representar os anúncios de artigos publicados pelos utilizadores no Bazar.
    /// Contém o título, preço, descrição, estado físico e referências ao vendedor e categoria.
    /// </summary>
    public class Anuncio
    {
        /// <summary>
        /// Identificador único do anúncio.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Título chamativo do anúncio de venda.
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O título deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição minuciosa dos detalhes do produto à venda.
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(4000, MinimumLength = 15, ErrorMessage = "A descrição deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Preço de venda pretendido pelo utilizador em euros.
        /// </summary>
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, 1000000.00, ErrorMessage = "O preço deve ser superior a 0.")]
        [Column(TypeName = "decimal(9, 2)")]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Estado transacional do anúncio (Disponível, Reservado, Vendido).
        /// </summary>
        [Required]
        [Display(Name = "Estado do Anúncio")]
        public EstadoAnuncio Estado { get; set; } = EstadoAnuncio.Disponivel;

        /// <summary>
        /// Estado físico de desgaste do produto (Novo, Como Novo, Usado, etc.).
        /// </summary>
        [Required(ErrorMessage = "O estado do produto é obrigatório.")]
        [Display(Name = "Estado do Produto")]
        public EstadoProduto EstadoProduto { get; set; } = EstadoProduto.BomEstado;

        /// <summary>
        /// Data e hora de publicação ou criação do anúncio.
        /// </summary>
        [Display(Name = "Data de Publicação")]
        public DateTime DataPublicacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Flag para indicar se o anúncio está ativo e visível na pesquisa pública.
        /// </summary>
        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Chave estrangeira que referencia o utilizador vendedor.
        /// </summary>
        [Required]
        [Display(Name = "Vendedor")]
        public string VendedorFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do vendedor (ApplicationUser).
        /// </summary>
        [ForeignKey("VendedorFK")]
        public virtual ApplicationUser? Vendedor { get; set; }

        /// <summary>
        /// Chave estrangeira que referencia a categoria na qual o artigo está inserido.
        /// </summary>
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public int CategoriaFK { get; set; }

        /// <summary>
        /// Referência de navegação para a categoria do anúncio.
        /// </summary>
        [ForeignKey("CategoriaFK")]
        public virtual Categoria? Categoria { get; set; }

        /// <summary>
        /// Coleção de fotografias associadas a este anúncio.
        /// </summary>
        public virtual ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();

        /// <summary>
        /// Coleção de mensagens trocadas no chat relacionadas com este anúncio.
        /// </summary>
        public virtual ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();

        /// <summary>
        /// Avaliações geradas após a venda deste artigo.
        /// </summary>
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// Lista de favoritos em que este anúncio está inserido.
        /// </summary>
        public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    }
}
