using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using Sapatus.Models.ViewModels;

namespace Sapatus.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrinhoItens = await _context.CarrinhoItems
                .Include(c => c.Produto)
                .Include(c => c.StockItem)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!carrinhoItens.Any())
            {
                TempData["Error"] = "O seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            var viewModelItens = carrinhoItens.Select(c => new CarrinhoItemViewModel
            {
                Id = c.Id,
                ProdutoId = c.ProdutoId,
                ProdutoNome = c.Produto?.Nome,
                ProdutoImagem = c.Produto?.ImagemUrl,
                ProdutoPreco = c.Produto?.Preco ?? 0,
                Tamanho = c.StockItem?.Tamanho,
                Cor = c.StockItem?.Cor,
                Quantidade = c.Quantidade,
                Subtotal = c.Quantidade * (c.Produto?.Preco ?? 0)
            }).ToList();

            var carrinhoViewModel = new CarrinhoViewModel
            {
                Itens = viewModelItens,
                Total = viewModelItens.Sum(i => i.Subtotal),
                TotalItens = viewModelItens.Sum(i => i.Quantidade)
            };

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            var viewModel = new CheckoutViewModel
            {
                Carrinho = carrinhoViewModel,
                SaldoDisponivel = wallet?.Saldo ?? 0,
                TemSaldoSuficiente = (wallet?.Saldo ?? 0) >= carrinhoViewModel.Total
            };

            return View(viewModel);
        }

        // POST: Checkout/Processar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Processar(string moradaEntrega)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrinhoItens = await _context.CarrinhoItems
                .Include(c => c.Produto)
                .Include(c => c.StockItem)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!carrinhoItens.Any())
            {
                TempData["Error"] = "O seu carrinho está vazio.";
                return RedirectToAction("Index", "Carrinho");
            }

            var total = carrinhoItens.Sum(c => c.Quantidade * (c.Produto?.Preco ?? 0));

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null || wallet.Saldo < total)
            {
                TempData["Error"] = "Saldo insuficiente na carteira.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(moradaEntrega))
            {
                TempData["Error"] = "Por favor, indique uma morada de entrega.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar stock disponível
            foreach (var item in carrinhoItens)
            {
                if (item.StockItem == null || item.Quantidade > item.StockItem.Quantidade)
                {
                    TempData["Error"] = $"Stock insuficiente para {item.Produto?.Nome} - Tamanho: {item.StockItem?.Tamanho}, Cor: {item.StockItem?.Cor}";
                    return RedirectToAction("Index", "Carrinho");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Criar a compra
                var compra = new Compra
                {
                    UserId = userId,
                    DataCompra = DateTime.Now,
                    Total = total,
                    Estado = EstadoCompra.Pago,
                    MoradaEntrega = moradaEntrega
                };
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // Criar os itens da compra e atualizar stock
                foreach (var item in carrinhoItens)
                {
                    var compraItem = new CompraItem
                    {
                        CompraId = compra.Id,
                        ProdutoId = item.ProdutoId,
                        StockItemId = item.StockItemId,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.Produto?.Preco ?? 0
                    };
                    _context.CompraItems.Add(compraItem);

                    // Atualizar stock
                    if (item.StockItem != null)
                    {
                        item.StockItem.Quantidade -= item.Quantidade;
                    }

                    // Marcar produto como comprado
                    if (item.Produto != null)
                    {
                        item.Produto.FoiComprado = true;
                    }
                }

                // Debitar da carteira
                wallet.Saldo -= total;
                wallet.DataUltimaAtualizacao = DateTime.Now;

                // Registrar transação de débito
                var transacao = new TransacaoWallet
                {
                    WalletId = wallet.Id,
                    Tipo = TipoTransacao.Debito,
                    Valor = total,
                    Descricao = $"Compra #{compra.Id}",
                    DataTransacao = DateTime.Now,
                    CompraId = compra.Id
                };
                _context.TransacaoWallets.Add(transacao);

                // Limpar carrinho
                _context.CarrinhoItems.RemoveRange(carrinhoItens);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Compra realizada com sucesso! Número da compra: #{compra.Id}";
                return RedirectToAction("Detalhes", "Compras", new { id = compra.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log do erro para debugging
                Console.WriteLine($"Erro no checkout: {ex.Message}");
                TempData["Error"] = "Ocorreu um erro ao processar a compra. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
