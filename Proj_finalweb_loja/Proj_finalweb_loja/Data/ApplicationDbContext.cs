using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data.Model;

namespace Proj_finalweb_loja.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Anuncio> Anuncios { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Imagem> Imagens { get; set; } = null!;
        public DbSet<Mensagem> Mensagens { get; set; } = null!;
        public DbSet<Avaliacao> Avaliacoes { get; set; } = null!;
        public DbSet<Favorito> Favoritos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure composite key for Favorito
            builder.Entity<Favorito>()
                .HasKey(f => new { f.UtilizadorFK, f.AnuncioFK });

            // Configure relationships for Favorito
            builder.Entity<Favorito>()
                .HasOne(f => f.Utilizador)
                .WithMany(u => u.Favoritos)
                .HasForeignKey(f => f.UtilizadorFK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorito>()
                .HasOne(f => f.Anuncio)
                .WithMany(a => a.Favoritos)
                .HasForeignKey(f => f.AnuncioFK)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Mensagem relationship (avoid multiple cascade paths)
            builder.Entity<Mensagem>()
                .HasOne(m => m.Remetente)
                .WithMany(u => u.MensagensEnviadas)
                .HasForeignKey(m => m.RemetenteFK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Mensagem>()
                .HasOne(m => m.Destinatario)
                .WithMany(u => u.MensagensRecebidas)
                .HasForeignKey(m => m.DestinatarioFK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Mensagem>()
                .HasOne(m => m.Anuncio)
                .WithMany(a => a.Mensagens)
                .HasForeignKey(m => m.AnuncioFK)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Avaliacao relationship (avoid multiple cascade paths)
            builder.Entity<Avaliacao>()
                .HasOne(a => a.Avaliador)
                .WithMany(u => u.AvaliacoesFeitas)
                .HasForeignKey(a => a.AvaliadorFK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Avaliacao>()
                .HasOne(a => a.Avaliando)
                .WithMany(u => u.AvaliacoesRecebidas)
                .HasForeignKey(a => a.AvaliandoFK)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Avaliacao>()
                .HasOne(a => a.Anuncio)
                .WithMany(an => an.Avaliacoes)
                .HasForeignKey(a => a.AnuncioFK)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

