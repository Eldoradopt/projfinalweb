using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proj_finalweb_loja.Data.Model
{
    /// <summary>
    /// Classe para representar os utilizadores da aplicação,
    /// ou seja, os dados que identificam cada utilizador com registo efetuado.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder {1} caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Morada do utilizador (Rua, Número, Porta).
        /// </summary>
        [StringLength(200, ErrorMessage = "A morada não pode exceder {1} caracteres.")]
        [Display(Name = "Morada")]
        public string? Morada { get; set; }

        /// <summary>
        /// Cidade onde reside o utilizador.
        /// </summary>
        [StringLength(100, ErrorMessage = "A cidade não pode exceder {1} caracteres.")]
        [Display(Name = "Cidade")]
        public string? Cidade { get; set; }

        /// <summary>
        /// Caminho relativo ou URL da fotografia de perfil do utilizador.
        /// </summary>
        [Display(Name = "Foto de Perfil")]
        public string? FotoPerfilPath { get; set; }

        /// <summary>
        /// Data de nascimento do utilizador.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? DataNascimento { get; set; }

        /// <summary>
        /// Data e hora de criação da conta.
        /// </summary>
        [Display(Name = "Data de Registo")]
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Coleção de anúncios publicados por este utilizador.
        /// </summary>
        public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

        /// <summary>
        /// Coleção de mensagens enviadas por este utilizador no chat.
        /// </summary>
        public virtual ICollection<Mensagem> MensagensEnviadas { get; set; } = new List<Mensagem>();

        /// <summary>
        /// Coleção de mensagens recebidas por este utilizador no chat.
        /// </summary>
        public virtual ICollection<Mensagem> MensagensRecebidas { get; set; } = new List<Mensagem>();

        /// <summary>
        /// Coleção de avaliações efetuadas por este utilizador a outros.
        /// </summary>
        public virtual ICollection<Avaliacao> AvaliacoesFeitas { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// Coleção de avaliações recebidas por este utilizador de outros compradores/vendedores.
        /// </summary>
        public virtual ICollection<Avaliacao> AvaliacoesRecebidas { get; set; } = new List<Avaliacao>();

        /// <summary>
        /// Coleção de anúncios marcados como favoritos por este utilizador.
        /// </summary>
        public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

        /// <summary>
        /// Coleção de vendedores que este utilizador segue/marcou como favoritos.
        /// </summary>
        public virtual ICollection<VendedorFavorito> VendedoresFavoritos { get; set; } = new List<VendedorFavorito>();

        /// <summary>
        /// Coleção de utilizadores que seguem este vendedor.
        /// </summary>
        public virtual ICollection<VendedorFavorito> Seguidores { get; set; } = new List<VendedorFavorito>();

        /// <summary>
        /// Indica se a conta do utilizador foi marcada como suspeita pela administração.
        /// </summary>
        [Display(Name = "Conta Suspeita")]
        public bool Suspeito { get; set; } = false;
    }
}
