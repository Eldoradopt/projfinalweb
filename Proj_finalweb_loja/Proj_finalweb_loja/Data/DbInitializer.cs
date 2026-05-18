using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proj_finalweb_loja.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proj_finalweb_loja.Data
{
    /// <summary>
    /// Classe responsável pela inicialização e povoamento (seeding) da base de dados.
    /// Cria as categorias padrão, utilizadores de demonstração e anúncios de teste.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Semeia os dados iniciais da aplicação caso a base de dados se encontre vazia.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        /// <param name="userManager">O gestor de utilizadores do ASP.NET Identity.</param>
        public static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // 1. Semear Categorias se a tabela estiver vazia
            if (!await context.Categorias.AnyAsync())
            {
                var categories = new[]
                {
                    new Categoria { Nome = "Eletrónica", Icone = "laptop" },
                    new Categoria { Nome = "Moda e Acessórios", Icone = "bag-heart" },
                    new Categoria { Nome = "Desporto e Lazer", Icone = "bicycle" },
                    new Categoria { Nome = "Casa e Jardim", Icone = "house-door" },
                    new Categoria { Nome = "Automóveis e Motos", Icone = "car-front" }
                };

                await context.Categorias.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var eletronica = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Eletrónica");
                var moda = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Moda e Acessórios");
                var desporto = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Desporto e Lazer");
                var casa = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Casa e Jardim");
                var auto = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Automóveis e Motos");

                var subcategories = new[]
                {
                    new Categoria { Nome = "Telemóveis e Tablets", CategoriaPaiFK = eletronica?.Id, Icone = "phone" },
                    new Categoria { Nome = "Computadores e Acessórios", CategoriaPaiFK = eletronica?.Id, Icone = "pc-display" },
                    new Categoria { Nome = "Consolas e Jogos", CategoriaPaiFK = eletronica?.Id, Icone = "controller" },

                    new Categoria { Nome = "Roupa", CategoriaPaiFK = moda?.Id, Icone = "gender-ambiguous" },
                    new Categoria { Nome = "Calçado", CategoriaPaiFK = moda?.Id, Icone = "smartwatch" },
                    new Categoria { Nome = "Malas e Carteiras", CategoriaPaiFK = moda?.Id, Icone = "wallet2" },

                    new Categoria { Nome = "Bicicletas", CategoriaPaiFK = desporto?.Id, Icone = "bicycle" },
                    new Categoria { Nome = "Equipamento de Fitness", CategoriaPaiFK = desporto?.Id, Icone = "activity" },

                    new Categoria { Nome = "Móveis", CategoriaPaiFK = casa?.Id, Icone = "table" },
                    new Categoria { Nome = "Eletrodomésticos", CategoriaPaiFK = casa?.Id, Icone = "plug" },
                    new Categoria { Nome = "Decoração", CategoriaPaiFK = casa?.Id, Icone = "flower1" },

                    new Categoria { Nome = "Carros e Carrinhas", CategoriaPaiFK = auto?.Id, Icone = "car-front-fill" },
                    new Categoria { Nome = "Motociclos", CategoriaPaiFK = auto?.Id, Icone = "speedometer" },
                    new Categoria { Nome = "Peças e Acessórios", CategoriaPaiFK = auto?.Id, Icone = "wrench" }
                };

                await context.Categorias.AddRangeAsync(subcategories);
                await context.SaveChangesAsync();
            }

            // 2. Semear Utilizadores de teste se a tabela estiver vazia
            if (!await userManager.Users.AnyAsync())
            {
                var users = new[]
                {
                    new ApplicationUser
                    {
                        UserName = "demo@ipt.pt",
                        Email = "demo@ipt.pt",
                        Nome = "Vendedor Demo",
                        Cidade = "Tomar",
                        Morada = "Campus do IPT",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345678"
                    },
                    new ApplicationUser
                    {
                        UserName = "maria@ipt.pt",
                        Email = "maria@ipt.pt",
                        Nome = "Maria Silva",
                        Cidade = "Abrantes",
                        Morada = "Rua das Flores, 14",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "965432109"
                    },
                    new ApplicationUser
                    {
                        UserName = "joao@ipt.pt",
                        Email = "joao@ipt.pt",
                        Nome = "João Pereira",
                        Cidade = "Tomar",
                        Morada = "Rua Silva Magalhães, 5",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "934567890"
                    }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Password123!");
                }
            }

            // 3. Semear Anúncios de demonstração se a tabela estiver vazia
            if (!await context.Anuncios.AnyAsync())
            {
                var sellerJoao = await userManager.FindByEmailAsync("joao@ipt.pt");
                var sellerMaria = await userManager.FindByEmailAsync("maria@ipt.pt");
                var sellerDemo = await userManager.FindByEmailAsync("demo@ipt.pt");

                var catTelemoveis = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Telemóveis e Tablets");
                var catComputadores = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Computadores e Acessórios");
                var catConsolas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Consolas e Jogos");
                var catRoupa = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Roupa");
                var catBicicletas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Bicicletas");

                if (sellerJoao != null && sellerMaria != null && sellerDemo != null)
                {
                    var ads = new List<Anuncio>
                    {
                        new Anuncio
                        {
                            Titulo = "iPhone 13 Pro Max - 256GB Azul Sierra",
                            Descricao = "Vendo iPhone 13 Pro Max em excelente estado de conservação, sem qualquer risco no ecrã ou na traseira. Saúde da bateria a 89%. Inclui caixa original, cabo de carregamento e fatura de compra. Sempre usado com película de vidro e capa de proteção.",
                            Preco = 690.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerJoao.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem>
                            {
                                new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1632661676136-8b438db02c9a?auto=format&fit=crop&w=600&q=80", Principal = true }
                            }
                        },
                        new Anuncio
                        {
                            Titulo = "MacBook Pro 13\" M1 - 8GB RAM / 256GB SSD",
                            Descricao = "MacBook Pro com chip Apple M1 em estado imaculado. Perfeito para estudantes e profissionais de desenvolvimento web. Apenas 45 ciclos de bateria, saúde a 100%. Vendo por ter recebido portátil da empresa. Entrego em mãos no campus do IPT em Tomar.",
                            Preco = 850.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerDemo.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-1),
                            Imagens = new List<Imagem>
                            {
                                new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=600&q=80", Principal = true }
                            }
                        },
                        new Anuncio
                        {
                            Titulo = "Bicicleta de Montanha Rockrider 520",
                            Descricao = "Bicicleta de montanha em bom estado geral, com algumas marcas normais de uso. Quadro de alumínio tamanho L, suspensão dianteira regulável de 80mm, transmissão SRAM de 24 velocidades e travões de disco mecânicos. Ideal para passeios ou deslocações diárias para as aulas.",
                            Preco = 180.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerJoao.Id,
                            CategoriaFK = catBicicletas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem>
                            {
                                new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", Principal = true }
                            }
                        },
                        new Anuncio
                        {
                            Titulo = "Casaco de Cabedal Vintage Clássico",
                            Descricao = "Casaco de cabedal preto vintage, estilo motoqueiro dos anos 90. Cabedal legítimo e muito resistente. Tamanho M. Em excelente estado de conservação, sem cortes nem rasgos. Forro interior muito confortável.",
                            Preco = 75.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerMaria.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem>
                            {
                                new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1551028719-00167b16eac5?auto=format&fit=crop&w=600&q=80", Principal = true }
                            }
                        },
                        new Anuncio
                        {
                            Titulo = "PlayStation 5 Digital Edition + Comando Extra",
                            Descricao = "Vendo consola PS5 Edição Digital em perfeito estado de funcionamento. Silenciosa e rápida. Inclui 2 comandos DualSense originais em branco, cabo HDMI, cabo de alimentação e caixa. Vendo por falta de uso devido aos exames.",
                            Preco = 320.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerDemo.Id,
                            CategoriaFK = catConsolas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddHours(-12),
                            Imagens = new List<Imagem>
                            {
                                new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&w=600&q=80", Principal = true }
                            }
                        }
                    };

                    await context.Anuncios.AddRangeAsync(ads);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
