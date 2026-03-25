using System.ComponentModel.DataAnnotations;

namespace Sapatus.Models.ViewModels
{
    public class ProdutoViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string? Nome { get; set; }
        
        [StringLength(500)]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, 10000)]
        public decimal Preco { get; set; }
        
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public CategoriaProduto Categoria { get; set; }
        
        [StringLength(50)]
        public string? Marca { get; set; }
        
        public string? ImagemUrl { get; set; }
        
        public bool EmDestaque { get; set; }
        
        public bool Privado { get; set; }
        
        public bool FoiComprado { get; set; }
        
        public List<StockItemViewModel>? Stocks { get; set; }
    }

    public class StockItemViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string? Tamanho { get; set; }
        
        [Required]
        public string? Cor { get; set; }
        
        [Required]
        [Range(0, 10000)]
        public int Quantidade { get; set; }
    }

    public class ProdutoDetalhesViewModel
    {
        public Produto? Produto { get; set; }
        public List<string>? TamanhosDisponiveis { get; set; }
        public List<string>? CoresDisponiveis { get; set; }
        public Dictionary<string, List<string>>? TamanhoCorMap { get; set; }
        public int StockTotal { get; set; }
    }

    public class AdicionarCarrinhoViewModel
    {
        public int ProdutoId { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public int Quantidade { get; set; } = 1;
    }
}
