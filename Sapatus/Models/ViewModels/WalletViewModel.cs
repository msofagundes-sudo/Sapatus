namespace Sapatus.Models.ViewModels
{
    public class WalletViewModel
    {
        public decimal Saldo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public List<TransacaoViewModel>? Transacoes { get; set; }
    }

    public class TransacaoViewModel
    {
        public int Id { get; set; }
        public TipoTransacao Tipo { get; set; }
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataTransacao { get; set; }
    }

    public class AdicionarFundosViewModel
    {
        public decimal Valor { get; set; }
    }
}
