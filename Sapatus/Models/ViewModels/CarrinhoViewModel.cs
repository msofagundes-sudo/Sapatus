namespace Sapatus.Models.ViewModels
{
    public class CarrinhoViewModel
    {
        public List<CarrinhoItemViewModel>? Itens { get; set; }
        public decimal Total { get; set; }
        public int TotalItens { get; set; }
    }

    public class CarrinhoItemViewModel
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public string? ProdutoImagem { get; set; }
        public decimal ProdutoPreco { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public int Quantidade { get; set; }
        public int StockDisponivel { get; set; }
        public decimal Subtotal { get; set; }
    }
}
