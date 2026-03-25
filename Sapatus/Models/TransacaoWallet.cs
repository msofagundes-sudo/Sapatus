using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public enum TipoTransacao
    {
        Credito,
        Debito
    }

    public class TransacaoWallet
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int WalletId { get; set; }
        
        [ForeignKey("WalletId")]
        public virtual Wallet? Carteira { get; set; }
        
        [Required]
        public TipoTransacao Tipo { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
        
        public string? Descricao { get; set; }
        
        public DateTime DataTransacao { get; set; } = DateTime.Now;
        
        // Referência à compra (se for uma compra)
        public int? CompraId { get; set; }
        
        [ForeignKey("CompraId")]
        public virtual Compra? Compra { get; set; }
    }
}
