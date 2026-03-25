using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public enum CategoriaProduto
    {
        Running,
        Casual,
        Basquetebol,
        Futebol,
        Tenis,
        Treino,
        Lifestyle
    }

    public class Produto
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string? Nome { get; set; }
        
        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000, ErrorMessage = "O preço deve estar entre 0.01 e 10000")]
        public decimal Preco { get; set; }
        
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public CategoriaProduto Categoria { get; set; }
        
        [StringLength(50)]
        public string? Marca { get; set; }
        
        public string? ImagemUrl { get; set; }
        
        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        [Display(Name = "Em Destaque")]
        public bool EmDestaque { get; set; } = false;
        
        // Se true, o produto está privado (não visível na loja)
        // Um produto que já foi comprado não pode ser eliminado, apenas tornado privado
        [Display(Name = "Privado")]
        public bool Privado { get; set; } = false;
        
        // Indica se o produto já foi comprado pelo menos uma vez
        [Display(Name = "Já foi comprado")]
        public bool FoiComprado { get; set; } = false;
        
        // Navigation properties
        public virtual ICollection<StockItem>? Stocks { get; set; }
        public virtual ICollection<CarrinhoItem>? CarrinhoItens { get; set; }
        public virtual ICollection<CompraItem>? CompraItens { get; set; }
    }
}
