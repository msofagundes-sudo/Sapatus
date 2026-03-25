using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using Sapatus.Models.ViewModels;

namespace Sapatus.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index(string? categoria = null, string? search = null)
        {
            var query = _context.Produtos
                .Include(p => p.Stocks)
                .Where(p => !p.Privado)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria) && Enum.TryParse<CategoriaProduto>(categoria, out var cat))
            {
                query = query.Where(p => p.Categoria == cat);
                ViewBag.CategoriaAtual = categoria;
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => 
                    p.Nome!.Contains(search) || 
                    (p.Descricao != null && p.Descricao.Contains(search)) ||
                    (p.Marca != null && p.Marca.Contains(search)));
                ViewBag.SearchTerm = search;
            }

            var produtos = await query.OrderByDescending(p => p.DataCriacao).ToListAsync();
            
            ViewBag.Categorias = Enum.GetNames(typeof(CategoriaProduto));
            
            return View(produtos);
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Privado);

            if (produto == null)
            {
                return NotFound();
            }

            // Preparar dados para a view
            var tamanhos = produto.Stocks?.Select(s => s.Tamanho).Distinct().ToList() ?? new List<string?>();
            var cores = produto.Stocks?.Select(s => s.Cor).Distinct().ToList() ?? new List<string?>();
            
            var tamanhoCorMap = new Dictionary<string, List<string>>();
            foreach (var stock in produto.Stocks ?? new List<StockItem>())
            {
                if (stock.Tamanho != null)
                {
                    if (!tamanhoCorMap.ContainsKey(stock.Tamanho))
                    {
                        tamanhoCorMap[stock.Tamanho] = new List<string>();
                    }
                    if (stock.Cor != null && !tamanhoCorMap[stock.Tamanho].Contains(stock.Cor))
                    {
                        tamanhoCorMap[stock.Tamanho].Add(stock.Cor);
                    }
                }
            }

            var viewModel = new ProdutoDetalhesViewModel
            {
                Produto = produto,
                TamanhosDisponiveis = tamanhos,
                CoresDisponiveis = cores,
                TamanhoCorMap = tamanhoCorMap,
                StockTotal = produto.Stocks?.Sum(s => s.Quantidade) ?? 0
            };

            return View(viewModel);
        }

        // POST: Produtos/AdicionarCarrinho
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarCarrinho(AdicionarCarrinhoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = model.ProdutoId });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Encontrar o stock item correspondente
            var stockItem = await _context.StockItems
                .FirstOrDefaultAsync(s => 
                    s.ProdutoId == model.ProdutoId && 
                    s.Tamanho == model.Tamanho && 
                    s.Cor == model.Cor);

            if (stockItem == null || stockItem.Quantidade < model.Quantidade)
            {
                TempData["Error"] = "Stock insuficiente para esta combinação de tamanho e cor.";
                return RedirectToAction(nameof(Details), new { id = model.ProdutoId });
            }

            // Verificar se já existe no carrinho
            var carrinhoItemExistente = await _context.CarrinhoItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.StockItemId == stockItem.Id);

            if (carrinhoItemExistente != null)
            {
                // Atualizar quantidade
                var novaQuantidade = carrinhoItemExistente.Quantidade + model.Quantidade;
                if (novaQuantidade > stockItem.Quantidade)
                {
                    TempData["Error"] = "Quantidade total excede o stock disponível.";
                    return RedirectToAction(nameof(Details), new { id = model.ProdutoId });
                }
                carrinhoItemExistente.Quantidade = novaQuantidade;
            }
            else
            {
                // Criar novo item no carrinho
                var carrinhoItem = new CarrinhoItem
                {
                    UserId = userId,
                    ProdutoId = model.ProdutoId,
                    StockItemId = stockItem.Id,
                    Quantidade = model.Quantidade,
                    DataAdicionado = DateTime.Now
                };
                _context.CarrinhoItems.Add(carrinhoItem);
            }

            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Produto adicionado ao carrinho com sucesso!";
            return RedirectToAction(nameof(Details), new { id = model.ProdutoId });
        }
    }
}
