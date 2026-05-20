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
        public static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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

            // 3. Semear Roles e Utilizadores de teste se a tabela estiver vazia
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Utilizador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Utilizador"));
            }

            if (!await userManager.Users.AnyAsync())
            {
                // Criar Administrador
                var admin = new ApplicationUser
                {
                    UserName = "admin@ipt.pt",
                    Email = "admin@ipt.pt",
                    Nome = "Administrador do Bazar",
                    Cidade = "Tomar",
                    Morada = "Serviços Centrais IPT",
                    EmailConfirmed = true,
                    FotoPerfilPath = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?auto=format&fit=crop&w=150&q=80",
                    PhoneNumber = "249328100"
                };
                var adminResult = await userManager.CreateAsync(admin, "Admin123!");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

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
                    },
                    new ApplicationUser
                    {
                        UserName = "pedro.santos@ipt.pt",
                        Email = "pedro.santos@ipt.pt",
                        Nome = "Pedro Santos",
                        Cidade = "Tomar",
                        Morada = "Rua Marquês de Pombal, 12",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345601"
                    },
                    new ApplicationUser
                    {
                        UserName = "ana.oliveira@ipt.pt",
                        Email = "ana.oliveira@ipt.pt",
                        Nome = "Ana Oliveira",
                        Cidade = "Abrantes",
                        Morada = "Avenida de Aljubarrota, 45",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345602"
                    },
                    new ApplicationUser
                    {
                        UserName = "rui.costa@ipt.pt",
                        Email = "rui.costa@ipt.pt",
                        Nome = "Rui Costa",
                        Cidade = "Tomar",
                        Morada = "Praceta das Oliveiras, 3",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345603"
                    },
                    new ApplicationUser
                    {
                        UserName = "sofia.martins@ipt.pt",
                        Email = "sofia.martins@ipt.pt",
                        Nome = "Sofia Martins",
                        Cidade = "Abrantes",
                        Morada = "Rua do Castelo, 18",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345604"
                    },
                    new ApplicationUser
                    {
                        UserName = "diogo.lopes@ipt.pt",
                        Email = "diogo.lopes@ipt.pt",
                        Nome = "Diogo Lopes",
                        Cidade = "Tomar",
                        Morada = "Rua Infantaria 15, 8",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345605"
                    },
                    new ApplicationUser
                    {
                        UserName = "catarina.sousa@ipt.pt",
                        Email = "catarina.sousa@ipt.pt",
                        Nome = "Catarina Sousa",
                        Cidade = "Tomar",
                        Morada = "Estrada do Prado, 22",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345606"
                    },
                    new ApplicationUser
                    {
                        UserName = "miguel.ferreira@ipt.pt",
                        Email = "miguel.ferreira@ipt.pt",
                        Nome = "Miguel Ferreira",
                        Cidade = "Abrantes",
                        Morada = "Rua da Barca, 5",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345607"
                    },
                    new ApplicationUser
                    {
                        UserName = "beatriz.gomes@ipt.pt",
                        Email = "beatriz.gomes@ipt.pt",
                        Nome = "Beatriz Gomes",
                        Cidade = "Tomar",
                        Morada = "Rua do Centro Histórico, 11",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345608"
                    },
                    new ApplicationUser
                    {
                        UserName = "tiago.rodrigues@ipt.pt",
                        Email = "tiago.rodrigues@ipt.pt",
                        Nome = "Tiago Rodrigues",
                        Cidade = "Tomar",
                        Morada = "Rua de São Pedro, 34",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345609",
                        Suspeito = true
                    },
                    new ApplicationUser
                    {
                        UserName = "ines.pereira@ipt.pt",
                        Email = "ines.pereira@ipt.pt",
                        Nome = "Inês Pereira",
                        Cidade = "Abrantes",
                        Morada = "Rua Dom João I, 6",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1554151228-14d9def656e4?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345610"
                    },
                    new ApplicationUser
                    {
                        Id = "24f08119-7675-4e6a-b2e6-c3f708e67463",
                        UserName = "email@gmial.com",
                        Email = "email@gmial.com",
                        Nome = "testenumero1234567890",
                        Cidade = "Tomar",
                        Morada = "Campus do IPT, Residências Académicas",
                        EmailConfirmed = true,
                        FotoPerfilPath = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=150&q=80",
                        PhoneNumber = "912345679"
                    }
                };

                foreach (var user in users)
                {
                    var userResult = await userManager.CreateAsync(user, "Password123!");
                    if (userResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Utilizador");
                    }
                }
            }

            // 4. Semear Anúncios de demonstração se a tabela estiver vazia
            if (!await context.Anuncios.AnyAsync())
            {
                var sellerJoao = await userManager.FindByEmailAsync("joao@ipt.pt");
                var sellerMaria = await userManager.FindByEmailAsync("maria@ipt.pt");
                var sellerDemo = await userManager.FindByEmailAsync("demo@ipt.pt");
                var sellerPedro = await userManager.FindByEmailAsync("pedro.santos@ipt.pt");
                var sellerAna = await userManager.FindByEmailAsync("ana.oliveira@ipt.pt");
                var sellerRui = await userManager.FindByEmailAsync("rui.costa@ipt.pt");
                var sellerSofia = await userManager.FindByEmailAsync("sofia.martins@ipt.pt");
                var sellerDiogo = await userManager.FindByEmailAsync("diogo.lopes@ipt.pt");
                var sellerCatarina = await userManager.FindByEmailAsync("catarina.sousa@ipt.pt");
                var sellerMiguel = await userManager.FindByEmailAsync("miguel.ferreira@ipt.pt");
                var sellerBeatriz = await userManager.FindByEmailAsync("beatriz.gomes@ipt.pt");
                var sellerTiago = await userManager.FindByEmailAsync("tiago.rodrigues@ipt.pt");
                var sellerInes = await userManager.FindByEmailAsync("ines.pereira@ipt.pt");
                var sellerTest = await userManager.FindByEmailAsync("email@gmial.com");

                var catTelemoveis = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Telemóveis e Tablets");
                var catComputadores = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Computadores e Acessórios");
                var catConsolas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Consolas e Jogos");
                var catRoupa = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Roupa");
                var catCalcado = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Calçado");
                var catMalas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Malas e Carteiras");
                var catBicicletas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Bicicletas");
                var catFitness = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Equipamento de Fitness");
                var catMoveis = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Móveis");
                var catEletrodomesticos = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Eletrodomésticos");
                var catDecoracao = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Decoração");
                var catAutoPecas = await context.Categorias.FirstOrDefaultAsync(c => c.Nome == "Peças e Acessórios");

                if (sellerJoao != null && sellerMaria != null && sellerDemo != null &&
                    sellerPedro != null && sellerAna != null && sellerRui != null && sellerSofia != null &&
                    sellerDiogo != null && sellerCatarina != null && sellerMiguel != null && sellerBeatriz != null &&
                    sellerTiago != null && sellerInes != null && sellerTest != null)
                {
                    var ads = new List<Anuncio>
                    {
                        // --- Anúncios Originais ---
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
                        },

                        // --- Anúncios do Pedro Santos (1-5) ---
                        new Anuncio
                        {
                            Titulo = "Samsung Galaxy S22 Ultra 5G - 128GB Preto",
                            Descricao = "Samsung S22 Ultra em estado impecável. Sempre utilizado com película e capa protetora. Ecrã AMOLED curvo incrível de 120Hz e câmeras de 108MP com zoom ótico de 10x. Acompanha caixa original e cabo original. Sem qualquer risco.",
                            Preco = 640.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerPedro.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "T-shirt Vintage Rock de Banda Clássica",
                            Descricao = "T-shirt com estampado da mítica banda Led Zeppelin. Tamanho L masculino. Algodão macio de excelente qualidade, com um aspeto deslavado vintage muito bonito. Excelente para looks urbanos casuais.",
                            Preco = 15.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerPedro.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Halteres de Ferro Fundido 10kg (Par)",
                            Descricao = "Par de halteres de ferro fundido de 10kg cada um (total 20kg). Perfeitos para treino de força em casa. Pegas recartilhadas para melhor aderência e segurança durante os exercícios de musculação. Muito resistentes.",
                            Preco = 32.50m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerPedro.Id,
                            CategoriaFK = catFitness?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1517838277536-f5f99be501cd?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Estante de Livros IKEA Billy Branca",
                            Descricao = "Estante clássica IKEA Billy em cor branca. Dimensões padrão 80x28x202 cm. Tem algumas marcas ligeiras nas prateleiras inferiores, mas está perfeitamente estável e em excelente estado funcional. Já montada, ideal para transporte direto.",
                            Preco = 40.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerPedro.Id,
                            CategoriaFK = catMoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1594620302200-9a762244a156?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Powerbank Carregador Portátil 20000mAh",
                            Descricao = "Bateria externa de alta capacidade com 20000mAh. Duas saídas USB de carregamento rápido (2.1A) e uma entrada USB-C/Micro-USB. Ideal para viagens ou longos dias no campus do IPT. Carrega um telemóvel até 5 vezes.",
                            Preco = 22.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerPedro.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1609592424109-dd9892f1b17c?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios da Ana Oliveira (6-10) ---
                        new Anuncio
                        {
                            Titulo = "Teclado Mecânico RGB Switch Brown",
                            Descricao = "Teclado mecânico compacto layout PT (60%) com switches táteis Brown, muito silenciosos e perfeitos para digitação ou jogos. Retroiluminação RGB com múltiplos modos customizáveis. Inclui cabo USB-C amovível e extrator de teclas.",
                            Preco = 55.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerAna.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Vestido de Festa Comprido Elegante Preto",
                            Descricao = "Vestido comprido preto de gala, muito elegante com decote em V nas costas. Utilizado apenas uma vez num casamento. Tecido com caimento fluido, tamanho M/L (38-40). Sem qualquer fio puxado ou defeito.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerAna.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1595777457583-95e059d581b8?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Tapete de Yoga Antiderrapante Premium",
                            Descricao = "Tapete de yoga ecológico em TPE de dupla face com espessura de 6mm, garantindo excelente amortecimento para articulações. Linhas de alinhamento corporal gravadas na superfície para ajudar na postura. Completamente novo e selado.",
                            Preco = 19.90m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerAna.Id,
                            CategoriaFK = catFitness?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Conjunto de 6 Pratos em Cerâmica Tradicional",
                            Descricao = "Lindo conjunto de 6 pratos rasos de cerâmica pintados à mão de forma tradicional, comprados em Coimbra. Perfeitos para decoração de salas ou para servir refeições de forma rústica e sofisticada. Sem falhas ou lascas.",
                            Preco = 30.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerAna.Id,
                            CategoriaFK = catDecoracao?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-8),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1577140917170-285929fb55b7?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Sapatilhas de Corrida Nike Air Zoom",
                            Descricao = "Ténis de corrida originais Nike Air Zoom Pegasus. Tamanho 38. Muito leves e confortáveis, com sola de alta durabilidade e excelente amortecimento reativo. Utilizadas cerca de 3 vezes em caminhadas curtas. Como novas.",
                            Preco = 60.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerAna.Id,
                            CategoriaFK = catCalcado?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios do Rui Costa (11-15) ---
                        new Anuncio
                        {
                            Titulo = "Nintendo Switch Neon 32GB com Capa",
                            Descricao = "Consola Nintendo Switch V2 com excelente autonomia de bateria. Inclui os Joy-Cons originais Azul e Vermelho Neon, cabo HDMI, adaptador AC, Dock e capa de transporte premium. Ecrã sem qualquer risco, sempre usado com película.",
                            Preco = 210.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerRui.Id,
                            CategoriaFK = catConsolas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1578301978693-85fa9c0320b9?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Casaco Corta-vento Corrida Kalenji Azul",
                            Descricao = "Casaco impermeável e corta-vento para corrida ou caminhadas. Tamanho M. Muito leve e respirável, com detalhes refletores de segurança para corrida noturna. Vários bolsos com fechos funcionais. Em bom estado.",
                            Preco = 20.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerRui.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1548883354-7622d03aca27?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Capacete de Ciclismo Giro MTB Regulável",
                            Descricao = "Capacete de excelente proteção para BTT/MTB regulável. Tamanho M (54-61cm). Estrutura In-Mold ultraleve e resistente. Múltiplos canais de ventilação. Nunca sofreu quedas ou impactos.",
                            Preco = 30.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerRui.Id,
                            CategoriaFK = catBicicletas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1557053910-d9eadeed1c58?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Candeeiro de Mesa Minimalista em Madeira",
                            Descricao = "Candeeiro de pé fabricado de forma artesanal em madeira maciça de pinho natural. Ideal para secretárias ou mesas de cabeceira em quartos de estudantes. Inclui lâmpada de filamento LED estilo vintage decorativa.",
                            Preco = 18.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerRui.Id,
                            CategoriaFK = catDecoracao?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Mochila Fjallraven Kanken Clássica",
                            Descricao = "Mochila unissexo modelo Kanken Clássica em cor azul-petróleo. 16 litros de capacidade. Fabricada em tecido Vinylon F super durável e repelente de água. Algumas marcas de sujidade na base laváveis, mas sem rasgos.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerRui.Id,
                            CategoriaFK = catMalas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1622560480605-d83c853bc5c3?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios da Sofia Martins (16-20) ---
                        new Anuncio
                        {
                            Titulo = "iPad Air 4ª Geração 64GB Space Gray",
                            Descricao = "Apple iPad Air 4 (Ecrã Liquid Retina de 10.9 polegadas). Saúde da bateria a 92%, sem marcas de quedas nem arranhões. Ideal para ler apontamentos e assistir a aulas. Entrego na caixa original com carregador.",
                            Preco = 390.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerSofia.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Blusão de Ganga Levi's Vintage",
                            Descricao = "Blusão de ganga clássico Levi's dos anos 90, ganga muito espessa de excelente qualidade. Tamanho M oversized. Tem aquele aspeto envelhecido natural de muito bom gosto. Muito confortável de vestir.",
                            Preco = 40.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerSofia.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1576995853123-5a10305d93c0?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Bicicleta Urbana Órbita Vintage Vermelha",
                            Descricao = "Bicicleta clássica de cidade de fabrico nacional (Órbita). Quadro de aço em cor vermelha vibrante, guarda-lamas, porta-bagagens traseiro e selim largo muito confortável. Tem marcas de oxidação ligeiras mas rola perfeitamente.",
                            Preco = 110.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerSofia.Id,
                            CategoriaFK = catBicicletas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Espelho de Parede Redondo Dourado",
                            Descricao = "Elegante espelho de parede redondo com moldura metálica decorativa em tom dourado envelhecido. Diâmetro de 60cm. Ideal para decorar o hall de entrada, quarto ou casa de banho. Em perfeito estado.",
                            Preco = 25.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerSofia.Id,
                            CategoriaFK = catDecoracao?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1618220179428-22790b461013?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Mala de Ombro Tiracolo em Pele Camel",
                            Descricao = "Mala tiracolo feminina em pele genuína camel. Vários compartimentos com fecho zipper e alça regulável muito robusta. Perfeita para carregar o tablet, livros e outros bens pessoais com elegância no dia-a-dia.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerSofia.Id,
                            CategoriaFK = catMalas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1590874103328-eac38a683ce7?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios do Diogo Lopes (21-25) ---
                        new Anuncio
                        {
                            Titulo = "Auscultadores Bluetooth ANC Sony WH-XB910N",
                            Descricao = "Auscultadores sem fios premium da Sony com cancelamento ativo de ruído inteligente (ANC) e reforço de graves EXTRA BASS. Autonomia fabulosa de 30 horas. Conexão multiponto estável. Inclui estojo rígido e cabos.",
                            Preco = 95.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerDiogo.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Camisola Oficial do Benfica Adidas 2023/24",
                            Descricao = "Camisola de futebol oficial do Sport Lisboa e Benfica, temporada 2023/24 com emblema bordado. Tamanho M. Muito pouco usada, está lavada e sem qualquer marca de desgaste ou rasgo. 100% original.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerDiogo.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1577219491135-ce391730fb2c?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Bola de Futebol Adidas Champions League",
                            Descricao = "Bola de futebol tamanho 5 padrão da UEFA Champions League. Costuras seladas termicamente e painéis texturizados de grande qualidade para maior aerodinâmica e controlo de trajetória. Completamente nova e cheia.",
                            Preco = 20.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerDiogo.Id,
                            CategoriaFK = catFitness?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Puff Gigante Confortável Cinzento",
                            Descricao = "Puff em formato de pera de grande tamanho, estofado em tecido de microfibra de veludo cinzento escuro, extremamente macio. Enchimento interior em pérolas de esferovite premium para máximo conforto e postura relaxada.",
                            Preco = 38.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerDiogo.Id,
                            CategoriaFK = catMoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Rato Gaming Sem Fios Logitech G305",
                            Descricao = "Rato wireless gaming ultraleve equipado com sensor ótico HERO de alta precisão (até 12000 DPI). Resposta ultrarrápida de 1ms através da tecnologia Lightspeed. Autonomia absurda com apenas uma pilha AA. Apenas testado.",
                            Preco = 35.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerDiogo.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios da Catarina Sousa (26-30) ---
                        new Anuncio
                        {
                            Titulo = "Monitor Gaming ASUS 24\" IPS 144Hz 1ms",
                            Descricao = "Monitor gaming ASUS VP249QGR com ecrã IPS Full HD. Taxa de atualização fluida de 144Hz com tempo de resposta de 1ms e FreeSync. Ângulos de visão amplos e cores excelentes. Inclui suporte de mesa original e cabo HDMI.",
                            Preco = 120.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerCatarina.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-1),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Botas de Pele Camel Timberland Clássicas",
                            Descricao = "Clássicas botas Timberland unissexo amarelas em pele nobuck impermeável premium. Tamanho 39. Sola de borracha de alta tração e costuras seladas de grande resistência. Pouco uso, muito quentes e confortáveis.",
                            Preco = 75.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerCatarina.Id,
                            CategoriaFK = catCalcado?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1520639888713-7851133b1ed0?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Raquete de Padel Bullpadel Raider Pro",
                            Descricao = "Raquete de Padel Bullpadel Raider com formato em gota, ideal para jogadores regulares que procuram uma excelente relação entre controle e potência no campo. Possui protetor de cabeça instalado contra impactos.",
                            Preco = 60.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerCatarina.Id,
                            CategoriaFK = catFitness?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Planta Artificial Decorativa com Vaso Cerâmica",
                            Descricao = "Belo vaso decorativo de cerâmica cinzenta com planta artificial de folhas verdes muito realistas. Perfeito para trazer um pouco de cor à tua secretária ou quarto de estudo sem a preocupação de regar. Dimensões compactas.",
                            Preco = 12.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerCatarina.Id,
                            CategoriaFK = catDecoracao?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1517576597727-2c9748b94cc8?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Porta-moedas Pequeno em Pele Genuína",
                            Descricao = "Porta-moedas e cartões em pele genuína curtida de cor castanho-escuro. Possui compartimento interior com fecho zipper e ranhuras exteriores de rápido acesso para cartões de estudante e multibanco. Feito à mão.",
                            Preco = 18.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerCatarina.Id,
                            CategoriaFK = catMalas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1627124718515-552fdc96a941?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios do Miguel Ferreira (31-35) ---
                        new Anuncio
                        {
                            Titulo = "Comando PlayStation 5 DualSense Branco",
                            Descricao = "Comando original sem fios DualSense branco para PS5. Gatilhos adaptativos e feedback háptico excelentes e fully funcionais. Bateria de longa duração. Sem qualquer desvio de analógico (drift). Muito bem cuidado.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerMiguel.Id,
                            CategoriaFK = catConsolas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Sweatshirt com Capuz Champion Cinzenta",
                            Descricao = "Sweatshirt hoodie desportiva com capuz ajustável e bolso canguru da marca Champion. Cor cinzento mesclado. Tamanho M. Muito confortável de vestir, tecido quente com excelente forro escovado interior.",
                            Preco = 25.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerMiguel.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1556911220-e15b29be8c8f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Bicicleta de Estrada Btwin Triban RC 120",
                            Descricao = "Bicicleta de estrada Triban ideal para iniciação ao ciclismo. Quadro leve de alumínio tamanho L, garfo dianteiro de carbono que absorve vibrações, transmissão Shimano de 2x8 velocidades e pneus novos de 28mm antifuro. Excelente estado.",
                            Preco = 280.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerMiguel.Id,
                            CategoriaFK = catBicicletas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Cafeteira de Filtro Elétrica Bosch Vermelha",
                            Descricao = "Cafeteira elétrica Bosch de filtro com jarra de vidro resistente de 1.25 litros. Prepara de forma super rápida e silenciosa até 10 chávenas de café quente. Sistema antigotas integrado e base de aquecimento ativa.",
                            Preco = 20.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerMiguel.Id,
                            CategoriaFK = catEletrodomesticos?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1570968915860-54d5c301fc9f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Suporte Magnético de Telemóvel para Carro",
                            Descricao = "Suporte de grelha de ventilação universal para telemóveis, com íman de neodímio ultra forte. Inclui duas placas metálicas adesivas redondas para colar no telemóvel ou na capa de proteção. Completamente novo em caixa.",
                            Preco = 8.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerMiguel.Id,
                            CategoriaFK = catAutoPecas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1586105251261-72a756497a11?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios da Beatriz Gomes (36-40) ---
                        new Anuncio
                        {
                            Titulo = "Máquina de Café Nespresso Krups Pixie",
                            Descricao = "Máquina de café Nespresso modelo Pixie, compacta e de cor cinzenta. Elevada pressão de 19 bar para espressos com creme espesso. Aquecimento em apenas 25 segundos e paragem de fluxo programável. Muito cuidada.",
                            Preco = 45.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerBeatriz.Id,
                            CategoriaFK = catEletrodomesticos?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Óculos de Sol Aviador Clássicos Vintage",
                            Descricao = "Óculos de sol estilo aviador clássico com moldura metálica prateada e lentes polarizadas de cor verde-garrafa. Oferecem excelente proteção UV400. Inclui estojo rígido e pano de microfibra de limpeza.",
                            Preco = 15.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerBeatriz.Id,
                            CategoriaFK = catRoupa?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1511499767150-a48a237f0083?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Mala de Viagem Grande Samsonite Rígida",
                            Descricao = "Mala rígida de grande dimensão com 4 rodas giratórias a 360º de extrema fluidez. Fechadura de segurança padrão TSA integrada. Fabricada em policarbonato ultraleve e flexível. Algumas marcas de manuseamento aeroportuário normais.",
                            Preco = 65.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerBeatriz.Id,
                            CategoriaFK = catMalas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1565026057447-bc90a3dceb87?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Estojo Completo de Desenho e Carvão Novo",
                            Descricao = "Estojo de transporte com fecho contendo 24 peças de materiais de desenho profissional: lápis de grafite de diversas durezas (H a B), carvões macios, esfuminhos de papel, lixas e borrachas. Totalmente novo, ideal para estudantes de artes.",
                            Preco = 25.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerBeatriz.Id,
                            CategoriaFK = catFitness?.Id ?? 1, // Lazer
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Relógio de Pulso Digital Casio Clássico",
                            Descricao = "O clássico relógio de pulso digital unissexo Casio em resina preta com bracelete de aço prateada. Possui luz integrada, cronómetro de alta precisão, alarme diário e calendário automático. Bateria de longa durabilidade. Como novo.",
                            Preco = 22.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerBeatriz.Id,
                            CategoriaFK = catRoupa?.Id ?? 1, // Acessórios
                            DataPublicacao = DateTime.UtcNow.AddDays(-7),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios do Tiago Rodrigues (41-45) [VENDEDOR SUSPEITO] ---
                        new Anuncio
                        {
                            Titulo = "iPhone 14 Pro Max 256GB Deep Purple - Urgente",
                            Descricao = "Vendo iPhone 14 Pro Max de 256GB por motivos de saúde urgentes, daí o preço ser tão baixo. Ecrã e parte traseira em estado perfeito, sem riscos. Saúde da bateria de 96%, livre de operador e iCloud. Apenas respondo por correio registado pré-pago.",
                            Preco = 590.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTiago.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-1),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "PlayStation 5 Nova em Caixa Selada",
                            Descricao = "Vendo PS5 Edição Física nova com leitor Blu-ray, embalagem original completamente selada de fábrica. Inclui fatura de compra e garantia de 3 anos nacional. Não aceito trocas nem entregas presenciais, envio apenas por transportadora.",
                            Preco = 300.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerTiago.Id,
                            CategoriaFK = catConsolas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Ténis Balenciaga Triple S Pretos Originais",
                            Descricao = "Ténis luxo Balenciaga Triple S originais pretos. Tamanho 42. Têm algumas marcas de uso ligeiras mas mantêm o design icónico intacto. Muito robustos e com sola de grande aderência. Caixa e fatura não disponíveis.",
                            Preco = 175.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerTiago.Id,
                            CategoriaFK = catCalcado?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Apple Watch Series 8 GPS 45mm Alumínio",
                            Descricao = "Smartwatch Apple Watch Series 8 com caixa de 45mm de alumínio cinzento escuro e bracelete desportiva preta. Inclui carregador por indução e caixa original. Envio imediato após boa cobrança por MBWay.",
                            Preco = 190.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTiago.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1517502884422-41eaaced0168?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Trotinete Elétrica Xiaomi Mi 3 Preta",
                            Descricao = "Trotinete Xiaomi 3 em excelente estado de conservação. Bateria com autonomia de 30km por carga e velocidade máxima regulável até 25km/h. Pneus em bom estado e travões revistos. Apenas vendo por correio expresso.",
                            Preco = 180.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerTiago.Id,
                            CategoriaFK = catBicicletas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1558981806-ec527fa84c39?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios da Inês Pereira (46-50) ---
                        new Anuncio
                        {
                            Titulo = "Torradeira Vintage Vermelha Ariete",
                            Descricao = "Torradeira de estilo retro vintage de duas fatias largas. Permite 6 níveis de tostagem diferentes e inclui funções de descongelamento e reaquecimento. Gaveta de migalhas removível e fácil de limpar. Muito bonita.",
                            Preco = 22.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.BomEstado,
                            VendedorFK = sellerInes.Id,
                            CategoriaFK = catEletrodomesticos?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1584269600464-37b1b58a9fe7?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Colar de Prata 925 com Pendente de Coração",
                            Descricao = "Delicado colar fabricado em prata de lei 925 contrastada, com um pendente em formato de coração cravejado com brilhantes de zircónia cúbica. Comprimento da corrente ajustável. Acompanha caixa de oferta.",
                            Preco = 25.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerInes.Id,
                            CategoriaFK = catRoupa?.Id ?? 1, // Acessórios
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Quadro Decorativo Abstrato Pintado à Mão",
                            Descricao = "Lindo quadro abstrato em tela montada sobre moldura de madeira, pintado à mão com tinta acrílica em tons pastel de azul, rosa e dourado. Dimensões: 50x70cm. Pronto a pendurar para embelezar qualquer sala.",
                            Preco = 28.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerInes.Id,
                            CategoriaFK = catDecoracao?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-6),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1579783902614-a3fb3927b6a5?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Mini Aspirador USB para Secretária",
                            Descricao = "Aspirador compacto e portátil recarregável por USB. Bocal com escova amovível de grande eficiência para limpar migalhas, poeiras e resíduos de teclados de computadores ou secretárias de estudo. Apenas testado.",
                            Preco = 12.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Novo,
                            VendedorFK = sellerInes.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Sapatilhas Vans Old Skool Pretas",
                            Descricao = "Os clássicos ténis de skate Vans Old Skool em lona e camurça de cor preta com a icónica risca lateral branca. Tamanho 37. Têm marcas de desgaste visíveis na lona mas a sola está perfeitamente funcional.",
                            Preco = 35.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.Usado,
                            VendedorFK = sellerInes.Id,
                            CategoriaFK = catCalcado?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },

                        // --- Anúncios do Utilizador de Teste (51-55) ---
                        new Anuncio
                        {
                            Titulo = "iPhone 15 Pro Max - 512GB Titânio Natural",
                            Descricao = "iPhone 15 Pro Max em estado rigorosamente impecável, cor Titânio Natural. 512GB de capacidade. Saúde de bateria a 98%, sempre carregado com cuidado. Sem riscos, mossas ou qualquer sinal de uso. Inclui caixa original com acessórios por estrear e fatura de compra com garantia.",
                            Preco = 1050.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTest.Id,
                            CategoriaFK = catTelemoveis?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-1),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "MacBook Pro 14\" M3 Pro - 18GB / 512GB SSD",
                            Descricao = "MacBook Pro de 14 polegadas equipado com o poderosíssimo chip M3 Pro (CPU 12-core, GPU 18-core). 18GB de memória unificada e 512GB SSD. Cor Preto Espacial. Estado absolutamente imaculado, sem qualquer marca. Apenas 12 ciclos de bateria. Inclui caixa, carregador MagSafe de 96W e fatura.",
                            Preco = 1950.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTest.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-2),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Consola PlayStation 5 Slim 1TB com Comando Extra",
                            Descricao = "PlayStation 5 modelo Slim com leitor de discos e armazenamento de 1TB. Menos de 3 meses de uso ocasional. Inclui o comando original DualSense, cabos originais e base de suporte. Ofereço jogo EA Sports FC 24 em formato físico. Caixa original intacta.",
                            Preco = 420.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTest.Id,
                            CategoriaFK = catConsolas?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-3),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Auscultadores Apple AirPods Max Cinzento Espacial",
                            Descricao = "AirPods Max em cor Space Gray em estado rigorosamente como novo. Excelente cancelamento de ruído ativo e áudio espacial com rastreio dinâmico da cabeça. Almofadas limpas e higienizadas, sem marcas de uso. Inclui a Smart Case e cabo Lightning para USB-C original.",
                            Preco = 380.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTest.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-4),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1613040809024-b4ef7ba99bc3?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        },
                        new Anuncio
                        {
                            Titulo = "Teclado Custom Mecânico GMMK Pro 75%",
                            Descricao = "Teclado mecânico customizado Glorious GMMK Pro layout 75% ANSI. Caixa em alumínio cnc cinzento. Equipado com switches lineares lubrificados NovelKeys Cream e keycaps premium double-shot PBT. Placa de latão para um som 'thocky' incrível. Cabo em espiral Glorious incluído.",
                            Preco = 240.00m,
                            Estado = EstadoAnuncio.Disponivel,
                            EstadoProduto = EstadoProduto.ComoNovo,
                            VendedorFK = sellerTest.Id,
                            CategoriaFK = catComputadores?.Id ?? 1,
                            DataPublicacao = DateTime.UtcNow.AddDays(-5),
                            Imagens = new List<Imagem> { new Imagem { CaminhoFicheiro = "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&w=600&q=80", Principal = true } }
                        }
                    };

                    await context.Anuncios.AddRangeAsync(ads);
                    await context.SaveChangesAsync();

                    // 5. Associar Tags aos anúncios de seed
                    var tagIphone13 = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "iPhone 13");
                    var tagIphone14 = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "iPhone 14");
                    var tagMacBook = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "MacBook");
                    var tagPS5 = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "PS5");
                    var tagApple = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Apple");
                    var tagVintage = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Vintage");
                    var tagGaming = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Gaming");
                    var tagCaixaOriginal = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Caixa Original");
                    var tagMontanha = await context.Tags.FirstOrDefaultAsync(t => t.Nome == "Montanha");

                    var anuncioIphone = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("iPhone 13"));
                    var anuncioIphone14 = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("iPhone 14"));
                    var anuncioMac = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("MacBook"));
                    var anuncioPS5 = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("PlayStation 5 Digital"));
                    var anuncioPS5Tiago = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("PlayStation 5 Nova"));
                    var anuncioCasaco = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("Casaco"));
                    var anuncioBici = await context.Anuncios.FirstOrDefaultAsync(a => a.Titulo.Contains("Bicicleta de Montanha"));

                    var anuncioTags = new List<AnuncioTag>();

                    if (anuncioIphone != null)
                    {
                        if (tagIphone13 != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagIphone13.Id });
                        if (tagApple != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagApple.Id });
                        if (tagCaixaOriginal != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone.Id, TagFK = tagCaixaOriginal.Id });
                    }
                    if (anuncioIphone14 != null)
                    {
                        if (tagIphone14 != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone14.Id, TagFK = tagIphone14.Id });
                        if (tagApple != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioIphone14.Id, TagFK = tagApple.Id });
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
                    if (anuncioPS5Tiago != null)
                    {
                        if (tagPS5 != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioPS5Tiago.Id, TagFK = tagPS5.Id });
                        if (tagGaming != null) anuncioTags.Add(new AnuncioTag { AnuncioFK = anuncioPS5Tiago.Id, TagFK = tagGaming.Id });
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

                    // 6. Semear avaliações cruzadas realistas para os novos e antigos utilizadores
                    var avaliacoes = new List<Avaliacao>
                    {
                        new Avaliacao { Nota = 5, Comentario = "Vendedor fantástico, muito atencioso e rápido na entrega!", AvaliadorFK = sellerMaria.Id, AvaliandoFK = sellerJoao.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-10) },
                        new Avaliacao { Nota = 4, Comentario = "Boa experiência, artigo tal como descrito.", AvaliadorFK = sellerDemo.Id, AvaliandoFK = sellerJoao.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-8) },
                        new Avaliacao { Nota = 5, Comentario = "Excelente vendedora, muito simpática!", AvaliadorFK = sellerJoao.Id, AvaliandoFK = sellerMaria.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-12) },
                        new Avaliacao { Nota = 5, Comentario = "Muito profissional e honesto.", AvaliadorFK = sellerJoao.Id, AvaliandoFK = sellerDemo.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-11) },
                        new Avaliacao { Nota = 3, Comentario = "Demorou um pouco mas o produto chegou em perfeitas condições.", AvaliadorFK = sellerMaria.Id, AvaliandoFK = sellerDemo.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-15) },

                        // Novas avaliações
                        new Avaliacao { Nota = 5, Comentario = "O Pedro vendeu-me o telemóvel exatamente como novo. Super atencioso!", AvaliadorFK = sellerAna.Id, AvaliandoFK = sellerPedro.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-2) },
                        new Avaliacao { Nota = 5, Comentario = "Compradora excelente, pontual no pagamento e muito simpática.", AvaliadorFK = sellerPedro.Id, AvaliandoFK = sellerAna.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-2) },
                        new Avaliacao { Nota = 4, Comentario = "Teclado mecânico impecável, transição super rápida de entrega.", AvaliadorFK = sellerDiogo.Id, AvaliandoFK = sellerAna.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-1) },
                        new Avaliacao { Nota = 5, Comentario = "Excelente vendedor, a Nintendo Switch está em perfeito estado e com capa de oferta.", AvaliadorFK = sellerMiguel.Id, AvaliandoFK = sellerRui.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-3) },
                        new Avaliacao { Nota = 5, Comentario = "Fizemos a troca no campus do IPT, super fiável!", AvaliadorFK = sellerSofia.Id, AvaliandoFK = sellerRui.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-4) },
                        new Avaliacao { Nota = 5, Comentario = "O blusão vintage é lindo e veste lindamente. Recomendo imenso a Sofia!", AvaliadorFK = sellerInes.Id, AvaliandoFK = sellerSofia.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-5) },
                        new Avaliacao { Nota = 4, Comentario = "Muito satisfeito com os auscultadores Sony. Som incrível e cancelamento de ruído ótimo.", AvaliadorFK = sellerPedro.Id, AvaliandoFK = sellerDiogo.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-1) },
                        new Avaliacao { Nota = 5, Comentario = "Vaso decorativo muito elegante, exatamente como nas fotos.", AvaliadorFK = sellerBeatriz.Id, AvaliandoFK = sellerCatarina.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-6) },
                        new Avaliacao { Nota = 5, Comentario = "A raquete de padel está em ótimo estado. Bom negócio!", AvaliadorFK = sellerRui.Id, AvaliandoFK = sellerCatarina.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-4) },
                        new Avaliacao { Nota = 5, Comentario = "Comando de PS5 impecável e a funcionar a 100%. Recomendo o Miguel.", AvaliadorFK = sellerPedro.Id, AvaliandoFK = sellerMiguel.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-3) },
                        new Avaliacao { Nota = 5, Comentario = "O estojo de pintura é fantástico, completamente novo. Muito obrigada Beatriz!", AvaliadorFK = sellerInes.Id, AvaliandoFK = sellerBeatriz.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-2) },
                        new Avaliacao { Nota = 5, Comentario = "A torradeira retro funciona lindamente e fica perfeita na cozinha. Excelente vendedora!", AvaliadorFK = sellerSofia.Id, AvaliandoFK = sellerInes.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-2) },
                        
                        // Avaliações negativas para o Tiago Rodrigues (Vendedor Suspeito)
                        new Avaliacao { Nota = 1, Comentario = "Tentei comprar o iPhone mas exigiu-me pagamento MBWay adiantado e nunca mais me respondeu. Muito cuidado!", AvaliadorFK = sellerPedro.Id, AvaliandoFK = sellerTiago.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-1) },
                        new Avaliacao { Nota = 1, Comentario = "Burlador! Recebeu o dinheiro da PS5 por transferência imediata e bloqueou-me no chat. Denunciado à polícia!", AvaliadorFK = sellerMaria.Id, AvaliandoFK = sellerTiago.Id, DataAvaliacao = DateTime.UtcNow.AddDays(-3) }
                    };

                    await context.Avaliacoes.AddRangeAsync(avaliacoes);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
