namespace Sapatus.Models.ViewModels
{
    public class CompraViewModel
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal Total { get; set; }
        public EstadoCompra Estado { get; set; }
        public string? MoradaEntrega { get; set; }
        public List<CompraItemViewModel>? Itens { get; set; }
    }

    public class CompraItemViewModel
    {
        public int Id { get; set; }
        public string? ProdutoNome { get; set; }
        public string? ProdutoImagem { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CheckoutViewModel
    {
        public CarrinhoViewModel? Carrinho { get; set; }
        public decimal SaldoDisponivel { get; set; }
        public bool TemSaldoSuficiente { get; set; }
        public string? MoradaEntrega { get; set; }
    }

    public class HistoricoComprasViewModel
    {
        public List<CompraViewModel>? Compras { get; set; }
    }

    public class RegistoComprasAdminViewModel
    {
        public List<CompraAdminViewModel>? Compras { get; set; }
        public decimal TotalVendas { get; set; }
        public int TotalCompras { get; set; }
    }

    public class CompraAdminViewModel
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }
        public string? NomeUtilizador { get; set; }
        public string? EmailUtilizador { get; set; }
        public decimal Total { get; set; }
        public EstadoCompra Estado { get; set; }
        public int QuantidadeItens { get; set; }
    }
}
