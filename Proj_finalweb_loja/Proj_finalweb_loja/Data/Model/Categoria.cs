using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe que representa uma Categoria ou Subcategoria de artigos no marketplace.
    /// Suporta auto-referenciação para organizar de forma hierárquica (Categoria Pai e Subcategorias).
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único da categoria.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da categoria (Ex: Eletrónica, Telemóveis, Roupa).
        /// </summary>
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder {1} caracteres.")]
        [Display(Name = "Categoria")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Nome do ícone da Bootstrap Icons utilizado no frontend (Ex: laptop, phone, bicycle).
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Ícone")]
        public string? Icone { get; set; }

        /// <summary>
        /// Chave estrangeira que aponta para a categoria pai, se for uma subcategoria.
        /// </summary>
        [Display(Name = "Categoria Pai")]
        public int? CategoriaPaiFK { get; set; }

        /// <summary>
        /// Referência de navegação para a categoria pai (auto-referência).
        /// </summary>
        [ForeignKey("CategoriaPaiFK")]
        [Display(Name = "Categoria Pai")]
        public virtual Categoria? CategoriaPai { get; set; }

        /// <summary>
        /// Lista de subcategorias associadas a esta categoria.
        /// </summary>
        public virtual ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

        /// <summary>
        /// Lista de anúncios publicados sob esta categoria.
        /// </summary>
        public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
    }
}
