using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public class StockItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        
        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }
        
        [Required(ErrorMessage = "O tamanho é obrigatório")]
        [StringLength(10)]
        public string? Tamanho { get; set; }
        
        [Required(ErrorMessage = "A cor é obrigatória")]
        [StringLength(50)]
        public string? Cor { get; set; }
        
        [Required(ErrorMessage = "A quantidade em stock é obrigatória")]
        [Range(0, 10000, ErrorMessage = "A quantidade deve estar entre 0 e 10000")]
        public int Quantidade { get; set; }
        
        // Navigation properties
        public virtual ICollection<CarrinhoItem>? CarrinhoItens { get; set; }
        public virtual ICollection<CompraItem>? CompraItens { get; set; }
    }
}
