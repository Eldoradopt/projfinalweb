using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Tabela de junção (N-M) para representar os anúncios que cada utilizador marcou como favoritos.
    /// Utiliza chave composta por UtilizadorFK e AnuncioFK.
    /// </summary>
    public class Favorito
    {
        /// <summary>
        /// Chave estrangeira que aponta para o utilizador que guardou o anúncio (ApplicationUser.Id).
        /// </summary>
        [Required]
        [StringLength(450)]
        [Display(Name = "Utilizador")]
        public string UtilizadorFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do utilizador.
        /// </summary>
        [ForeignKey("UtilizadorFK")]
        public virtual ApplicationUser? Utilizador { get; set; }

        /// <summary>
        /// Chave estrangeira do anúncio marcado como favorito.
        /// </summary>
        [Required]
        [Display(Name = "Anúncio")]
        public int AnuncioFK { get; set; }

        /// <summary>
        /// Referência de navegação para o anúncio favorito.
        /// </summary>
        [ForeignKey("AnuncioFK")]
        public virtual Anuncio? Anuncio { get; set; }

        /// <summary>
        /// Data e hora em que o utilizador guardou este anúncio nos favoritos.
        /// </summary>
        [Display(Name = "Adicionado a")]
        public DateTime DataGuardado { get; set; } = DateTime.UtcNow;
    }
}
