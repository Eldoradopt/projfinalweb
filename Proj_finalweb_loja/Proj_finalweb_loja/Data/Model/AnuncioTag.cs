using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Tabela pivot para a relação Many-to-Many entre Anuncio e Tag.
    /// Um anúncio pode ter várias tags, e uma tag pode estar em vários anúncios.
    /// </summary>
    public class AnuncioTag
    {
        /// <summary>
        /// Chave estrangeira do anúncio.
        /// </summary>
        public int AnuncioFK { get; set; }

        /// <summary>
        /// Referência de navegação para o anúncio.
        /// </summary>
        [ForeignKey("AnuncioFK")]
        public virtual Anuncio Anuncio { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira da tag.
        /// </summary>
        public int TagFK { get; set; }

        /// <summary>
        /// Referência de navegação para a tag.
        /// </summary>
        [ForeignKey("TagFK")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
