using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using System.Diagnostics;

namespace Sapatus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var produtosDestaque = await _context.Produtos
                .Where(p => p.EmDestaque && !p.Privado)
                .Take(6)
                .ToListAsync();

            var todosProdutos = await _context.Produtos
                .Where(p => !p.Privado)
                .Take(8)
                .ToListAsync();

            ViewBag.ProdutosDestaque = produtosDestaque;
            ViewBag.TodosProdutos = todosProdutos;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
