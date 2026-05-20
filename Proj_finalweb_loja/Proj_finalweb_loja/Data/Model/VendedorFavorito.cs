using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Representa a relação entre um utilizador (Seguidor) que marcou outro utilizador como favorito (Vendedor).
    /// </summary>
    public class VendedorFavorito
    {
        [Required]
        [StringLength(450)]
        public string SeguidorFK { get; set; } = string.Empty;

        [ForeignKey("SeguidorFK")]
        public virtual ApplicationUser? Seguidor { get; set; }

        [Required]
        [StringLength(450)]
        public string VendedorFK { get; set; } = string.Empty;

        [ForeignKey("VendedorFK")]
        public virtual ApplicationUser? Vendedor { get; set; }

        public DateTime DataAdicionado { get; set; } = DateTime.UtcNow;
    }
}
