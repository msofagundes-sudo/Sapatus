using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models.ViewModels;

namespace Sapatus.Controllers
{
    [Authorize]
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var compras = await _context.Compras
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            var viewModel = new HistoricoComprasViewModel
            {
                Compras = compras.Select(c => new CompraViewModel
                {
                    Id = c.Id,
                    DataCompra = c.DataCompra,
                    Total = c.Total,
                    Estado = c.Estado,
                    MoradaEntrega = c.MoradaEntrega,
                    Itens = c.Itens?.Select(i => new CompraItemViewModel
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
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Compras/Detalhes/5
        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var compra = await _context.Compras
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .Include(c => c.Itens)
                .ThenInclude(i => i.StockItem)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (compra == null)
            {
                // Verificar se é admin
                if (User.IsInRole("Admin"))
                {
                    compra = await _context.Compras
                        .Include(c => c.Itens)
                        .ThenInclude(i => i.Produto)
                        .Include(c => c.Itens)
                        .ThenInclude(i => i.StockItem)
                        .Include(c => c.Utilizador)
                        .FirstOrDefaultAsync(c => c.Id == id);
                }
            }

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
    }
}
