using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe para representar a reputação ou feedback de transação (avaliação de 1 a 5 estrelas).
    /// Associa o avaliador (quem pontua), o avaliado (quem recebe nota) e o anúncio transacionado.
    /// </summary>
    public class Avaliacao
    {
        /// <summary>
        /// Identificador único da avaliação.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nota ou classificação numérica atribuída (de 1 a 5 estrelas).
        /// </summary>
        [Required(ErrorMessage = "A nota é obrigatória.")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5 estrelas.")]
        [Display(Name = "Classificação")]
        public int Nota { get; set; }

        /// <summary>
        /// Comentário facultativo descrevendo a experiência de compra/venda.
        /// </summary>
        [StringLength(500, ErrorMessage = "O comentário não pode exceder {1} caracteres.")]
        [Display(Name = "Comentário")]
        public string? Comentario { get; set; }

        /// <summary>
        /// Data e hora em que a avaliação foi registada.
        /// </summary>
        [Display(Name = "Data da Avaliação")]
        public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Chave estrangeira do utilizador que efetua a avaliação (Avaliador).
        /// </summary>
        [Required]
        [Display(Name = "Avaliador")]
        public string AvaliadorFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do Avaliador.
        /// </summary>
        [ForeignKey("AvaliadorFK")]
        public virtual ApplicationUser? Avaliador { get; set; }

        /// <summary>
        /// Chave estrangeira do utilizador que é classificado (Avaliado).
        /// </summary>
        [Required]
        [Display(Name = "Avaliado")]
        public string AvaliandoFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do Avaliado.
        /// </summary>
        [ForeignKey("AvaliandoFK")]
        public virtual ApplicationUser? Avaliando { get; set; }

        /// <summary>
        /// Chave estrangeira do anúncio que motivou a transação e avaliação.
        /// </summary>
        [Required]
        [Display(Name = "Anúncio")]
        public int AnuncioFK { get; set; }

        /// <summary>
        /// Referência de navegação para o anúncio relacionado.
        /// </summary>
        [ForeignKey("AnuncioFK")]
        public virtual Anuncio? Anuncio { get; set; }
    }
}
