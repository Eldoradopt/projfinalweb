using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe que representa uma Tag (etiqueta) que pode ser associada a anúncios.
    /// Quando EColecaoEspecial = true, a tag torna-se uma Coleção Especial com visual destacado.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Identificador único da tag.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome da tag (ex: "iPhone 13", "PS5", "Nike", "Vintage").
        /// </summary>
        [Required(ErrorMessage = "O nome da tag é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Tag")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Indica se esta tag é uma Coleção Especial com página dedicada e visual premium.
        /// </summary>
        [Display(Name = "Coleção Especial?")]
        public bool EColecaoEspecial { get; set; } = false;

        /// <summary>
        /// Nome do ícone Bootstrap Icons para usar na coleção especial (ex: "phone", "laptop").
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Ícone")]
        public string? Icone { get; set; }

        /// <summary>
        /// Cor hexadecimal do badge/banner da coleção (ex: "#6366f1", "#f59e0b").
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Cor (Hex)")]
        public string? CorHex { get; set; }

        /// <summary>
        /// Breve descrição da coleção especial (exibida na página da coleção).
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Lista de associações entre este tag e os anúncios.
        /// </summary>
        public virtual ICollection<AnuncioTag> AnuncioTags { get; set; } = new List<AnuncioTag>();
    }
}
