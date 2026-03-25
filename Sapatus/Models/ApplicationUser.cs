using Microsoft.AspNetCore.Identity;

namespace Sapatus.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? PrimeiroNome { get; set; }
        public string? UltimoNome { get; set; }
        public DateTime DataRegisto { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual Wallet? Carteira { get; set; }
        public virtual ICollection<CarrinhoItem>? CarrinhoItens { get; set; }
        public virtual ICollection<Compra>? Compras { get; set; }
    }
}
