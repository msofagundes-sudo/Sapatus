using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public class CarrinhoItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Utilizador { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        
        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }
        
        [Required]
        public int StockItemId { get; set; }
        
        [ForeignKey("StockItemId")]
        public virtual StockItem? StockItem { get; set; }
        
        [Required]
        [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100")]
        public int Quantidade { get; set; }
        
        public DateTime DataAdicionado { get; set; } = DateTime.Now;
        
        [NotMapped]
        public decimal Subtotal => Quantidade * (Produto?.Preco ?? 0);
    }
}
