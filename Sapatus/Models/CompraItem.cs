using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public class CompraItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CompraId { get; set; }
        
        [ForeignKey("CompraId")]
        public virtual Compra? Compra { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        
        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }
        
        [Required]
        public int StockItemId { get; set; }
        
        [ForeignKey("StockItemId")]
        public virtual StockItem? StockItem { get; set; }
        
        [Required]
        public int Quantidade { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
        
        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
