using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using Sapatus.Models.ViewModels;

namespace Sapatus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Produtos
        public async Task<IActionResult> Produtos()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Stocks)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return View(produtos);
        }

        // GET: Admin/CriarProduto
        public IActionResult CriarProduto()
        {
            ViewBag.Categorias = Enum.GetValues(typeof(CategoriaProduto));
            return View();
        }

        // POST: Admin/CriarProduto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarProduto(ProdutoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var produto = new Produto
                {
                    Nome = model.Nome,
                    Descricao = model.Descricao,
                    Preco = model.Preco,
                    Categoria = model.Categoria,
                    Marca = model.Marca,
                    ImagemUrl = model.ImagemUrl,
                    EmDestaque = model.EmDestaque,
                    DataCriacao = DateTime.Now,
                    Privado = false,
                    FoiComprado = false
                };

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();

                // Adicionar stock items se fornecidos
                if (model.Stocks != null && model.Stocks.Any())
                {
                    foreach (var stock in model.Stocks)
                    {
                        if (!string.IsNullOrEmpty(stock.Tamanho) && !string.IsNullOrEmpty(stock.Cor))
                        {
                            _context.StockItems.Add(new StockItem
                            {
                                ProdutoId = produto.Id,
                                Tamanho = stock.Tamanho,
                                Cor = stock.Cor,
                                Quantidade = stock.Quantidade
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Produto criado com sucesso!";
                return RedirectToAction(nameof(Produtos));
            }

            ViewBag.Categorias = Enum.GetValues(typeof(CategoriaProduto));
            return View(model);
        }

        // GET: Admin/EditarProduto/5
        public async Task<IActionResult> EditarProduto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            var viewModel = new ProdutoViewModel
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Categoria = produto.Categoria,
                Marca = produto.Marca,
                ImagemUrl = produto.ImagemUrl,
                EmDestaque = produto.EmDestaque,
                Privado = produto.Privado,
                FoiComprado = produto.FoiComprado,
                Stocks = produto.Stocks?.Select(s => new StockItemViewModel
                {
                    Id = s.Id,
                    Tamanho = s.Tamanho,
                    Cor = s.Cor,
                    Quantidade = s.Quantidade
                }).ToList()
            };

            ViewBag.Categorias = Enum.GetValues(typeof(CategoriaProduto));
            return View(viewModel);
        }

        // POST: Admin/EditarProduto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProduto(int id, ProdutoViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var produto = await _context.Produtos
                        .Include(p => p.Stocks)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (produto == null)
                    {
                        return NotFound();
                    }

                    produto.Nome = model.Nome;
                    produto.Descricao = model.Descricao;
                    produto.Preco = model.Preco;
                    produto.Categoria = model.Categoria;
                    produto.Marca = model.Marca;
                    produto.ImagemUrl = model.ImagemUrl;
                    produto.EmDestaque = model.EmDestaque;
                    produto.Privado = model.Privado;

                    _context.Update(produto);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Produto atualizado com sucesso!";
                    return RedirectToAction(nameof(Produtos));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categorias = Enum.GetValues(typeof(CategoriaProduto));
            return View(model);
        }

        // POST: Admin/RemoverProduto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            // Se o produto já foi comprado, apenas torná-lo privado
            if (produto.FoiComprado)
            {
                produto.Privado = true;
                _context.Update(produto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produto foi tornado privado (não pode ser eliminado pois já foi comprado).";
            }
            else
            {
                // Remover stock items primeiro
                var stockItems = await _context.StockItems.Where(s => s.ProdutoId == id).ToListAsync();
                _context.StockItems.RemoveRange(stockItems);

                // Remover itens do carrinho
                var carrinhoItens = await _context.CarrinhoItems.Where(c => c.ProdutoId == id).ToListAsync();
                _context.CarrinhoItems.RemoveRange(carrinhoItens);

                // Remover produto
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Produto removido com sucesso!";
            }

            return RedirectToAction(nameof(Produtos));
        }

        // POST: Admin/TornarPrivado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TornarPrivado(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            produto.Privado = !produto.Privado;
            _context.Update(produto);
            await _context.SaveChangesAsync();

            var status = produto.Privado ? "privado" : "público";
            TempData["Success"] = $"Produto tornado {status} com sucesso!";

            return RedirectToAction(nameof(Produtos));
        }

        // GET: Admin/GerirStock/5
        public async Task<IActionResult> GerirStock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Admin/AdicionarStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarStock(int produtoId, string tamanho, string cor, int quantidade)
        {
            if (string.IsNullOrEmpty(tamanho) || string.IsNullOrEmpty(cor) || quantidade < 0)
            {
                TempData["Error"] = "Dados inválidos para o stock.";
                return RedirectToAction(nameof(GerirStock), new { id = produtoId });
            }

            // Verificar se já existe este tamanho/cor
            var stockExistente = await _context.StockItems
                .FirstOrDefaultAsync(s => s.ProdutoId == produtoId && s.Tamanho == tamanho && s.Cor == cor);

            if (stockExistente != null)
            {
                stockExistente.Quantidade += quantidade;
                _context.Update(stockExistente);
            }
            else
            {
                _context.StockItems.Add(new StockItem
                {
                    ProdutoId = produtoId,
                    Tamanho = tamanho,
                    Cor = cor,
                    Quantidade = quantidade
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Stock atualizado com sucesso!";

            return RedirectToAction(nameof(GerirStock), new { id = produtoId });
        }

        // POST: Admin/RemoverStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverStock(int id)
        {
            var stockItem = await _context.StockItems.FindAsync(id);

            if (stockItem == null)
            {
                return NotFound();
            }

            var produtoId = stockItem.ProdutoId;

            // Remover itens do carrinho que usam este stock
            var carrinhoItens = await _context.CarrinhoItems.Where(c => c.StockItemId == id).ToListAsync();
            _context.CarrinhoItems.RemoveRange(carrinhoItens);

            _context.StockItems.Remove(stockItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item de stock removido com sucesso!";
            return RedirectToAction(nameof(GerirStock), new { id = produtoId });
        }

        // GET: Admin/RegistoCompras
        public async Task<IActionResult> RegistoCompras()
        {
            var compras = await _context.Compras
                .Include(c => c.Utilizador)
                .Include(c => c.Itens)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            var viewModel = new RegistoComprasAdminViewModel
            {
                Compras = compras.Select(c => new CompraAdminViewModel
                {
                    Id = c.Id,
                    DataCompra = c.DataCompra,
                    NomeUtilizador = c.Utilizador?.UserName,
                    EmailUtilizador = c.Utilizador?.Email,
                    Total = c.Total,
                    Estado = c.Estado,
                    QuantidadeItens = c.Itens?.Count ?? 0
                }).ToList(),
                TotalVendas = compras.Sum(c => c.Total),
                TotalCompras = compras.Count
            };

            return View(viewModel);
        }

        // GET: Admin/DetalhesCompra/5
        public async Task<IActionResult> DetalhesCompra(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .Include(c => c.Itens)
                .ThenInclude(i => i.StockItem)
                .Include(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            var viewModel = new CompraViewModel
            {
                Id = compra.Id,
                DataCompra = compra.DataCompra,
                Total = compra.Total,
                Estado = compra.Estado,
                MoradaEntrega = compra.MoradaEntrega,
                Itens = compra.Itens?.Select(i => new CompraItemViewModel
                {
                    Id = i.Id,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagem = i.Produto?.ImagemUrl,
                    Tamanho = i.StockItem?.Tamanho,
                    Cor = i.StockItem?.Cor,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Quantidade * i.PrecoUnitario
                }).ToList()
            };

            ViewBag.NomeUtilizador = compra.Utilizador?.UserName;
            ViewBag.EmailUtilizador = compra.Utilizador?.Email;

            return View(viewModel);
        }

        // POST: Admin/AtualizarEstadoCompra/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarEstadoCompra(int id, EstadoCompra estado)
        {
            var compra = await _context.Compras.FindAsync(id);

            if (compra == null)
            {
                return NotFound();
            }

            compra.Estado = estado;
            _context.Update(compra);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Estado da compra atualizado com sucesso!";
            return RedirectToAction(nameof(RegistoCompras));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
