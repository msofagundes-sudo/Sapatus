using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using Sapatus.Models.ViewModels;

namespace Sapatus.Controllers
{
    [Authorize]
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carrinho
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
                StockDisponivel = c.StockItem?.Quantidade ?? 0,
                Subtotal = c.Quantidade * (c.Produto?.Preco ?? 0)
            }).ToList();

            var viewModel = new CarrinhoViewModel
            {
                Itens = viewModelItens,
                Total = viewModelItens.Sum(i => i.Subtotal),
                TotalItens = viewModelItens.Sum(i => i.Quantidade)
            };

            return View(viewModel);
        }

        // POST: Carrinho/AtualizarQuantidade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarQuantidade(int id, int quantidade)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrinhoItem = await _context.CarrinhoItems
                .Include(c => c.StockItem)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (carrinhoItem == null)
            {
                return NotFound();
            }

            if (quantidade <= 0)
            {
                _context.CarrinhoItems.Remove(carrinhoItem);
            }
            else
            {
                // Verificar stock disponível
                if (carrinhoItem.StockItem != null && quantidade > carrinhoItem.StockItem.Quantidade)
                {
                    TempData["Error"] = "Quantidade solicitada excede o stock disponível.";
                    return RedirectToAction(nameof(Index));
                }
                carrinhoItem.Quantidade = quantidade;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Carrinho/Remover
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrinhoItem = await _context.CarrinhoItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (carrinhoItem == null)
            {
                return NotFound();
            }

            _context.CarrinhoItems.Remove(carrinhoItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Carrinho/Limpar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Limpar()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var carrinhoItens = await _context.CarrinhoItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.CarrinhoItems.RemoveRange(carrinhoItens);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Carrinho/Contador
        [HttpGet]
        public async Task<IActionResult> Contador()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var count = await _context.CarrinhoItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantidade);

            return Json(new { count });
        }
    }
}
