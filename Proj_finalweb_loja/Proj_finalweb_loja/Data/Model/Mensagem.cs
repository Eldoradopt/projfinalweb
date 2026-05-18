using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe para representar as mensagens de chat enviadas entre utilizadores.
    /// Registra o remetente, destinatário, conteúdo da conversa e o anúncio de referência.
    /// </summary>
    public class Mensagem
    {
        /// <summary>
        /// Identificador único da mensagem.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Texto ou conteúdo da mensagem enviada.
        /// </summary>
        [Required(ErrorMessage = "O conteúdo da mensagem é obrigatório.")]
        [StringLength(1000, ErrorMessage = "A mensagem não pode exceder {1} caracteres.")]
        [Display(Name = "Mensagem")]
        public string Conteudo { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora exatas de envio da mensagem.
        /// </summary>
        [Display(Name = "Data de Envio")]
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Flag indicando se a mensagem já foi lida pelo destinatário.
        /// </summary>
        [Display(Name = "Lida?")]
        public bool Lida { get; set; } = false;

        /// <summary>
        /// Chave estrangeira do utilizador que enviou a mensagem (Remetente).
        /// </summary>
        [Required]
        [Display(Name = "Remetente")]
        public string RemetenteFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do Remetente.
        /// </summary>
        [ForeignKey("RemetenteFK")]
        public virtual ApplicationUser? Remetente { get; set; }

        /// <summary>
        /// Chave estrangeira do utilizador que recebe a mensagem (Destinatário).
        /// </summary>
        [Required]
        [Display(Name = "Destinatário")]
        public string DestinatarioFK { get; set; } = string.Empty;

        /// <summary>
        /// Referência de navegação para a conta do Destinatário.
        /// </summary>
        [ForeignKey("DestinatarioFK")]
        public virtual ApplicationUser? Destinatario { get; set; }

        /// <summary>
        /// Chave estrangeira do anúncio sobre o qual decorre a negociação.
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
