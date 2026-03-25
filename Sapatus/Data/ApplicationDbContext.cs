using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sapatus.Models;

namespace Sapatus.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItems { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraItem> CompraItems { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TransacaoWallet> TransacaoWallets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurar relacionamentos e constraints
            
            // Produto - StockItem (1:N)
            builder.Entity<StockItem>()
                .HasOne(s => s.Produto)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser - CarrinhoItem (1:N)
            builder.Entity<CarrinhoItem>()
                .HasOne(c => c.Utilizador)
                .WithMany(u => u.CarrinhoItens)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Produto - CarrinhoItem (1:N)
            builder.Entity<CarrinhoItem>()
                .HasOne(c => c.Produto)
                .WithMany(p => p.CarrinhoItens)
                .HasForeignKey(c => c.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockItem - CarrinhoItem (1:N)
            builder.Entity<CarrinhoItem>()
                .HasOne(c => c.StockItem)
                .WithMany(s => s.CarrinhoItens)
                .HasForeignKey(c => c.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser - Compra (1:N)
            builder.Entity<Compra>()
                .HasOne(c => c.Utilizador)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Compra - CompraItem (1:N)
            builder.Entity<CompraItem>()
                .HasOne(ci => ci.Compra)
                .WithMany(c => c.Itens)
                .HasForeignKey(ci => ci.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Produto - CompraItem (1:N)
            builder.Entity<CompraItem>()
                .HasOne(ci => ci.Produto)
                .WithMany(p => p.CompraItens)
                .HasForeignKey(ci => ci.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockItem - CompraItem (1:N)
            builder.Entity<CompraItem>()
                .HasOne(ci => ci.StockItem)
                .WithMany(s => s.CompraItens)
                .HasForeignKey(ci => ci.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser - Wallet (1:1)
            builder.Entity<Wallet>()
                .HasOne(w => w.Utilizador)
                .WithOne(u => u.Carteira)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wallet - TransacaoWallet (1:N)
            builder.Entity<TransacaoWallet>()
                .HasOne(t => t.Carteira)
                .WithMany(w => w.Transacoes)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único para evitar duplicados no carrinho
            builder.Entity<CarrinhoItem>()
                .HasIndex(c => new { c.UserId, c.StockItemId })
                .IsUnique();
        }
    }
}
