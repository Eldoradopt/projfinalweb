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
    /// Cria as categorias padrão, utilizadores de demonstração, anúncios de teste e tags/coleções.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Semeia os dados iniciais da aplicação caso a base de dados se encontre vazia.
        /// </summary>
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

            // 2. Semear Tags e Coleções Especiais
            if (!await context.Tags.AnyAsync())
            {
                var tags = new[]
                {
                    // Coleções Especiais (EColecaoEspecial = true)
                    new Tag { Nome = "iPhone 13", EColecaoEspecial = true, Icone = "phone-fill", CorHex = "#6366f1", Descricao = "Todos os iPhone 13, 13 Pro e 13 Pro Max disponíveis no BazarIPT." },
                    new Tag { Nome = "iPhone 14", EColecaoEspecial = true, Icone = "phone-fill", CorHex = "#8b5cf6", Descricao = "Encontra o teu iPhone 14 ou 14 Pro a preços imbatíveis." },
                    new Tag { Nome = "iPhone 15", EColecaoEspecial = true, Icone = "phone-fill", CorHex = "#a855f7", Descricao = "A linha mais recente da Apple a preços de segunda mão." },
                    new Tag { Nome = "MacBook", EColecaoEspecial = true, Icone = "laptop-fill", CorHex = "#0ea5e9", Descricao = "MacBook Air e MacBook Pro de todas as gerações." },
                    new Tag { Nome = "PS5", EColecaoEspecial = true, Icone = "controller", CorHex = "#2563eb", Descricao = "PlayStation 5 — consola, comandos e acessórios." },
                    new Tag { Nome = "Nintendo Switch", EColecaoEspecial = true, Icone = "joystick-fill", CorHex = "#ef4444", Descricao = "Nintendo Switch, Switch Lite e Switch OLED." },
                    new Tag { Nome = "AirPods", EColecaoEspecial = true, Icone = "headphones", CorHex = "#14b8a6", Descricao = "AirPods de todas as gerações e AirPods Pro." },
                    new Tag { Nome = "Samsung Galaxy", EColecaoEspecial = true, Icone = "phone", CorHex = "#f59e0b", Descricao = "Toda a linha Samsung Galaxy S, A e Z." },

                    // Tags normais
                    new Tag { Nome = "Apple", EColecaoEspecial = false },
                    new Tag { Nome = "Samsung", EColecaoEspecial = false },
                    new Tag { Nome = "Vintage", EColecaoEspecial = false },
                    new Tag { Nome = "Nike", EColecaoEspecial = false },
                    new Tag { Nome = "Adidas", EColecaoEspecial = false },
                    new Tag { Nome = "Gaming", EColecaoEspecial = false },
                    new Tag { Nome = "Montanha", EColecaoEspecial = false },
                    new Tag { Nome = "Caixa Original", EColecaoEspecial = false },
                    new Tag { Nome = "Urgente", EColecaoEspecial = false },
                    new Tag { Nome = "Troca Aceite", EColecaoEspecial = false },
                };

                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }

            // 3. Semear Utilizadores de teste se a tabela estiver vazia
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

            // 4. Semear Anúncios de demonstração se a tabela estiver vazia
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

                    // 5. Associar Tags aos anúncios de seed
                    var tagIphone13 = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "iPhone 13");
                    var tagMacBook = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "MacBook");
                    var tagPS5 = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "PS5");
                    var tagApple = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Apple");
                    var tagVintage = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Vintage");
                    var tagGaming = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Gaming");
                    var tagCaixaOriginal = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Caixa Original");
                    var tagMontanha = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Montanha");

                    var anuncioIphone = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("iPhone 13"));
                    var anuncioMac = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("MacBook"));
                    var anuncioPS5 = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("PlayStation"));
                    var anuncioCasaco = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("Casaco"));
                    var anuncioBici = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("Bicicleta"));

                    var anuncioTags = new List<AnuncioTag>();

                    if (anuncioIphone != null)
                    {
                        if (tagIphone13 != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagIphone13.Id });
                        if (tagApple != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagApple.Id });
                        if (tagCaixaOriginal != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagCaixaOriginal.Id });
                    }
                    if (anuncioMac != null)
                    {
                        if (tagMacBook != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioMac.Id, TagFK = tagMacBook.Id });
                        if (tagApple != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioMac.Id, TagFK = tagApple.Id });
                    }
                    if (anuncioPS5 != null)
                    {
                        if (tagPS5 != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioPS5.Id, TagFK = tagPS5.Id });
                        if (tagGaming != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioPS5.Id, TagFK = tagGaming.Id });
                    }
                    if (anuncioCasaco != null)
                    {
                        if (tagVintage != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioCasaco.Id, TagFK = tagVintage.Id });
                    }
                    if (anuncioBici != null)
                    {
                        if (tagMontanha != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioBici.Id, TagFK = tagMontanha.Id });
                    }

                    if (anuncioTags.Any())
                    {
                        await context.AnuncioTags.AddRangeAsync(anuncioTags);
                        await context.SaveChangesAsync();
                    }

                    // 6. Seed some demo ratings
                    var avaliacoes = new List<Avaliacao>();
                    if (sellerJoao != null && sellerMaria != null && sellerDemo != null)
                    {
                        avaliacoes.Add(new Avaliacao { Nota = 5, Comentario = "Vendedor fantástico, muito atencioso e rápido na entrega!", AvaliadorFK = sellerMaria.Id, AvaliandoFK = sellerJoao.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-2) });
                        avaliacoes.Add(new Avaliacao { Nota = 4, Comentario = "Boa experiência, artigo tal como descrito.", AvaliadorFK = sellerDemo.Id, AvaliandoFK = sellerJoao.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-1) });
                        avaliacoes.Add(new Avaliacao { Nota = 5, Comentario = "Excelente vendedora, muito simpática!", AvaliadorFK = sellerJoao.Id, AvaliandoFK = sellerMaria.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-3) });
                        avaliacoes.Add(new Avaliacao { Nota = 5, Comentario = "Muito profissional e honesto.", AvaliadorFK = sellerJoao.Id, AvaliandoFK = sellerDemo.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-4) });
                        avaliacoes.Add(new Avaliacao { Nota = 3, Comentario = "Demorou um pouco mas o produto chegou em perfeitas condições.", AvaliadorFK = sellerMaria.Id, AvaliandoFK = sellerDemo.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-5) });
                    }

                    if (avaliacoes.Any())
                    {
                        await context.Avaliacoes.AddRangeAsync(avaliacoes);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
