using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Utilizador { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        // Histórico de transações
        public virtual ICollection<TransacaoWallet>? Transacoes { get; set; }
    }
}
