using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sapatus.Data;
using Sapatus.Models;
using Sapatus.Models.ViewModels;

namespace Sapatus.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Wallet
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = await _context.Wallets
                .Include(w => w.Transacoes)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                // Criar carteira se não existir
                wallet = new Wallet
                {
                    UserId = userId,
                    Saldo = 0,
                    DataCriacao = DateTime.Now
                };
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            var viewModel = new WalletViewModel
            {
                Saldo = wallet.Saldo,
                DataCriacao = wallet.DataCriacao,
                DataUltimaAtualizacao = wallet.DataUltimaAtualizacao,
                Transacoes = wallet.Transacoes?
                    .OrderByDescending(t => t.DataTransacao)
                    .Select(t => new TransacaoViewModel
                    {
                        Id = t.Id,
                        Tipo = t.Tipo,
                        Valor = t.Valor,
                        Descricao = t.Descricao,
                        DataTransacao = t.DataTransacao
                    }).ToList()
            };

            return View(viewModel);
        }

        // GET: Wallet/AdicionarFundos
        public IActionResult AdicionarFundos()
        {
            return View();
        }

        // POST: Wallet/AdicionarFundos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarFundos(AdicionarFundosViewModel model)
        {
            if (model.Valor <= 0)
            {
                ModelState.AddModelError("Valor", "O valor deve ser maior que zero.");
                return View(model);
            }

            if (model.Valor > 10000)
            {
                ModelState.AddModelError("Valor", "O valor máximo é 10.000€.");
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Saldo = 0,
                    DataCriacao = DateTime.Now
                };
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            // Adicionar fundos (sem verificação - como solicitado)
            wallet.Saldo += model.Valor;
            wallet.DataUltimaAtualizacao = DateTime.Now;

            // Registrar transação
            var transacao = new TransacaoWallet
            {
                WalletId = wallet.Id,
                Tipo = TipoTransacao.Credito,
                Valor = model.Valor,
                Descricao = "Adição de fundos",
                DataTransacao = DateTime.Now
            };
            _context.TransacaoWallets.Add(transacao);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Foram adicionados {model.Valor:C} à sua carteira.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Wallet/Saldo
        [HttpGet]
        public async Task<IActionResult> Saldo()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new { saldo = 0 });
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            return Json(new { saldo = wallet?.Saldo ?? 0 });
        }
    }
}
