using Microsoft.AspNetCore.Identity;
using Sapatus.Models;

namespace Sapatus.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Criar roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Criar admin
            var adminEmail = "admin@sapatus";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    PrimeiroNome = "Administrador",
                    UltimoNome = "Sapatus",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "@AdminSafe123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    
                    // Criar carteira para o admin
                    var wallet = new Wallet
                    {
                        UserId = adminUser.Id,
                        Saldo = 1000,
                        DataCriacao = DateTime.Now
                    };
                    context.Wallets.Add(wallet);
                    await context.SaveChangesAsync();
                }
            }

            // Adicionar produtos de exemplo se não existirem
            if (!context.Produtos.Any())
            {
                await SeedProdutos(context);
            }
        }

        private static async Task SeedProdutos(ApplicationDbContext context)
        {
            var produtos = new List<Produto>
            {
                new Produto
                {
                    Nome = "Nike Air Max 90",
                    Descricao = "Ténis icónico da Nike com tecnologia Air Max para máximo conforto. Design atemporal que nunca sai de moda.",
                    Preco = 129.99m,
                    Categoria = CategoriaProduto.Casual,
                    Marca = "Nike",
                    ImagemUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500",
                    EmDestaque = true,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "38", Cor = "Branco", Quantidade = 10 },
                        new StockItem { Tamanho = "39", Cor = "Branco", Quantidade = 15 },
                        new StockItem { Tamanho = "40", Cor = "Branco", Quantidade = 20 },
                        new StockItem { Tamanho = "41", Cor = "Branco", Quantidade = 18 },
                        new StockItem { Tamanho = "42", Cor = "Branco", Quantidade = 12 },
                        new StockItem { Tamanho = "38", Cor = "Preto", Quantidade = 8 },
                        new StockItem { Tamanho = "39", Cor = "Preto", Quantidade = 12 },
                        new StockItem { Tamanho = "40", Cor = "Preto", Quantidade = 15 },
                        new StockItem { Tamanho = "41", Cor = "Preto", Quantidade = 10 },
                        new StockItem { Tamanho = "42", Cor = "Preto", Quantidade = 8 },
                        new StockItem { Tamanho = "40", Cor = "Vermelho", Quantidade = 5 },
                        new StockItem { Tamanho = "41", Cor = "Vermelho", Quantidade = 7 },
                        new StockItem { Tamanho = "42", Cor = "Vermelho", Quantidade = 6 }
                    }
                },
                new Produto
                {
                    Nome = "Adidas Ultraboost 22",
                    Descricao = "Ténis de running de alta performance com tecnologia Boost para energia infinita. Conforto superior para longas distâncias.",
                    Preco = 179.99m,
                    Categoria = CategoriaProduto.Running,
                    Marca = "Adidas",
                    ImagemUrl = "https://images.unsplash.com/photo-1587563871167-1ee9c731aefb?w=500",
                    EmDestaque = true,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "39", Cor = "Preto", Quantidade = 20 },
                        new StockItem { Tamanho = "40", Cor = "Preto", Quantidade = 25 },
                        new StockItem { Tamanho = "41", Cor = "Preto", Quantidade = 22 },
                        new StockItem { Tamanho = "42", Cor = "Preto", Quantidade = 18 },
                        new StockItem { Tamanho = "43", Cor = "Preto", Quantidade = 15 },
                        new StockItem { Tamanho = "39", Cor = "Branco", Quantidade = 15 },
                        new StockItem { Tamanho = "40", Cor = "Branco", Quantidade = 18 },
                        new StockItem { Tamanho = "41", Cor = "Branco", Quantidade = 20 },
                        new StockItem { Tamanho = "42", Cor = "Branco", Quantidade = 16 },
                        new StockItem { Tamanho = "43", Cor = "Branco", Quantidade = 12 },
                        new StockItem { Tamanho = "40", Cor = "Azul", Quantidade = 10 },
                        new StockItem { Tamanho = "41", Cor = "Azul", Quantidade = 12 },
                        new StockItem { Tamanho = "42", Cor = "Azul", Quantidade = 8 }
                    }
                },
                new Produto
                {
                    Nome = "Puma RS-X3",
                    Descricao = "Ténis lifestyle com design arrojado e tecnologia Running System. Estilo urbano e conforto incomparável.",
                    Preco = 109.99m,
                    Categoria = CategoriaProduto.Lifestyle,
                    Marca = "Puma",
                    ImagemUrl = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?w=500",
                    EmDestaque = false,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "38", Cor = "Cinza", Quantidade = 12 },
                        new StockItem { Tamanho = "39", Cor = "Cinza", Quantidade = 15 },
                        new StockItem { Tamanho = "40", Cor = "Cinza", Quantidade = 18 },
                        new StockItem { Tamanho = "41", Cor = "Cinza", Quantidade = 14 },
                        new StockItem { Tamanho = "42", Cor = "Cinza", Quantidade = 10 },
                        new StockItem { Tamanho = "39", Cor = "Branco/Preto", Quantidade = 8 },
                        new StockItem { Tamanho = "40", Cor = "Branco/Preto", Quantidade = 10 },
                        new StockItem { Tamanho = "41", Cor = "Branco/Preto", Quantidade = 12 },
                        new StockItem { Tamanho = "42", Cor = "Branco/Preto", Quantidade = 9 }
                    }
                },
                new Produto
                {
                    Nome = "New Balance 574",
                    Descricao = "Clássico da New Balance com versatilidade para o dia a dia. Conforto duradouro e estilo atemporal.",
                    Preco = 89.99m,
                    Categoria = CategoriaProduto.Casual,
                    Marca = "New Balance",
                    ImagemUrl = "https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=500",
                    EmDestaque = true,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "37", Cor = "Cinza", Quantidade = 15 },
                        new StockItem { Tamanho = "38", Cor = "Cinza", Quantidade = 20 },
                        new StockItem { Tamanho = "39", Cor = "Cinza", Quantidade = 22 },
                        new StockItem { Tamanho = "40", Cor = "Cinza", Quantidade = 25 },
                        new StockItem { Tamanho = "41", Cor = "Cinza", Quantidade = 20 },
                        new StockItem { Tamanho = "42", Cor = "Cinza", Quantidade = 18 },
                        new StockItem { Tamanho = "38", Cor = "Azul Marinho", Quantidade = 10 },
                        new StockItem { Tamanho = "39", Cor = "Azul Marinho", Quantidade = 12 },
                        new StockItem { Tamanho = "40", Cor = "Azul Marinho", Quantidade = 15 },
                        new StockItem { Tamanho = "41", Cor = "Azul Marinho", Quantidade = 12 },
                        new StockItem { Tamanho = "42", Cor = "Azul Marinho", Quantidade = 10 }
                    }
                },
                new Produto
                {
                    Nome = "Nike Air Force 1",
                    Descricao = "O lendário ténis de basquetebol que se tornou um ícone da cultura urbana. Design clássico e durabilidade excecional.",
                    Preco = 99.99m,
                    Categoria = CategoriaProduto.Basquetebol,
                    Marca = "Nike",
                    ImagemUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=500",
                    EmDestaque = true,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "38", Cor = "Branco", Quantidade = 30 },
                        new StockItem { Tamanho = "39", Cor = "Branco", Quantidade = 35 },
                        new StockItem { Tamanho = "40", Cor = "Branco", Quantidade = 40 },
                        new StockItem { Tamanho = "41", Cor = "Branco", Quantidade = 38 },
                        new StockItem { Tamanho = "42", Cor = "Branco", Quantidade = 32 },
                        new StockItem { Tamanho = "43", Cor = "Branco", Quantidade = 25 },
                        new StockItem { Tamanho = "40", Cor = "Preto", Quantidade = 15 },
                        new StockItem { Tamanho = "41", Cor = "Preto", Quantidade = 18 },
                        new StockItem { Tamanho = "42", Cor = "Preto", Quantidade = 15 },
                        new StockItem { Tamanho = "43", Cor = "Preto", Quantidade = 12 }
                    }
                },
                new Produto
                {
                    Nome = "Asics Gel-Kayano 28",
                    Descricao = "Ténis de running premium com tecnologia GEL para amortecimento superior. Estabilidade e conforto para corredores exigentes.",
                    Preco = 159.99m,
                    Categoria = CategoriaProduto.Running,
                    Marca = "Asics",
                    ImagemUrl = "https://images.unsplash.com/photo-1562183241-b937e95585b6?w=500",
                    EmDestaque = false,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "39", Cor = "Azul/Branco", Quantidade = 12 },
                        new StockItem { Tamanho = "40", Cor = "Azul/Branco", Quantidade = 15 },
                        new StockItem { Tamanho = "41", Cor = "Azul/Branco", Quantidade = 18 },
                        new StockItem { Tamanho = "42", Cor = "Azul/Branco", Quantidade = 14 },
                        new StockItem { Tamanho = "43", Cor = "Azul/Branco", Quantidade = 10 },
                        new StockItem { Tamanho = "40", Cor = "Preto/Dourado", Quantidade = 8 },
                        new StockItem { Tamanho = "41", Cor = "Preto/Dourado", Quantidade = 10 },
                        new StockItem { Tamanho = "42", Cor = "Preto/Dourado", Quantidade = 8 },
                        new StockItem { Tamanho = "43", Cor = "Preto/Dourado", Quantidade = 6 }
                    }
                },
                new Produto
                {
                    Nome = "Vans Old Skool",
                    Descricao = "O clássico skate shoe da Vans com o icónico sidestripe. Estilo atemporal e durabilidade comprovada.",
                    Preco = 74.99m,
                    Categoria = CategoriaProduto.Casual,
                    Marca = "Vans",
                    ImagemUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?w=500",
                    EmDestaque = false,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "36", Cor = "Preto", Quantidade = 15 },
                        new StockItem { Tamanho = "37", Cor = "Preto", Quantidade = 18 },
                        new StockItem { Tamanho = "38", Cor = "Preto", Quantidade = 22 },
                        new StockItem { Tamanho = "39", Cor = "Preto", Quantidade = 25 },
                        new StockItem { Tamanho = "40", Cor = "Preto", Quantidade = 28 },
                        new StockItem { Tamanho = "41", Cor = "Preto", Quantidade = 24 },
                        new StockItem { Tamanho = "42", Cor = "Preto", Quantidade = 20 },
                        new StockItem { Tamanho = "38", Cor = "Xadrez", Quantidade = 10 },
                        new StockItem { Tamanho = "39", Cor = "Xadrez", Quantidade = 12 },
                        new StockItem { Tamanho = "40", Cor = "Xadrez", Quantidade = 15 },
                        new StockItem { Tamanho = "41", Cor = "Xadrez", Quantidade = 12 },
                        new StockItem { Tamanho = "42", Cor = "Xadrez", Quantidade = 10 }
                    }
                },
                new Produto
                {
                    Nome = "Jordan 1 Mid",
                    Descricao = "O icónico ténis de basquetebol que revolucionou o desporto e a moda. Um must-have para qualquer coleção.",
                    Preco = 129.99m,
                    Categoria = CategoriaProduto.Basquetebol,
                    Marca = "Nike",
                    ImagemUrl = "https://images.unsplash.com/photo-1552346154-21d32810aba3?w=500",
                    EmDestaque = true,
                    DataCriacao = DateTime.Now,
                    Stocks = new List<StockItem>
                    {
                        new StockItem { Tamanho = "38", Cor = "Vermelho/Preto/Branco", Quantidade = 8 },
                        new StockItem { Tamanho = "39", Cor = "Vermelho/Preto/Branco", Quantidade = 10 },
                        new StockItem { Tamanho = "40", Cor = "Vermelho/Preto/Branco", Quantidade = 12 },
                        new StockItem { Tamanho = "41", Cor = "Vermelho/Preto/Branco", Quantidade = 10 },
                        new StockItem { Tamanho = "42", Cor = "Vermelho/Preto/Branco", Quantidade = 8 },
                        new StockItem { Tamanho = "40", Cor = "Azul/Branco", Quantidade = 6 },
                        new StockItem { Tamanho = "41", Cor = "Azul/Branco", Quantidade = 8 },
                        new StockItem { Tamanho = "42", Cor = "Azul/Branco", Quantidade = 6 },
                        new StockItem { Tamanho = "43", Cor = "Azul/Branco", Quantidade = 5 }
                    }
                }
            };

            foreach (var produto in produtos)
            {
                context.Produtos.Add(produto);
            }

            await context.SaveChangesAsync();
        }
    }
}
