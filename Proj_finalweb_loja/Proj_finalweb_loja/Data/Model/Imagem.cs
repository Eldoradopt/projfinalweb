using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe para representar as fotografias associadas a cada anúncio.
    /// Armazena o caminho do ficheiro físico ou um URL remoto de imagem.
    /// </summary>
    public class Imagem
    {
        /// <summary>
        /// Identificador único da imagem.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Caminho relativo do ficheiro na pasta uploads ou URL externa direta.
        /// </summary>
        [Required]
        [Display(Name = "Caminho da Imagem")]
        public string CaminhoFicheiro { get; set; } = string.Empty;

        /// <summary>
        /// Flag que indica se esta é a imagem de capa e destaque do anúncio.
        /// </summary>
        [Display(Name = "Imagem Principal?")]
        public bool Principal { get; set; } = false;

        /// <summary>
        /// Chave estrangeira que referencia o anúncio a que pertence a fotografia.
        /// </summary>
        [Required]
        [Display(Name = "Anúncio")]
        public int AnuncioFK { get; set; }

        /// <summary>
        /// Referência de navegação para o anúncio associado.
        /// </summary>
        [ForeignKey("AnuncioFK")]
        public virtual Anuncio? Anuncio { get; set; }
    }
}
