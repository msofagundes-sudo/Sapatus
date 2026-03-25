using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sapatus.Models
{
    public enum EstadoCompra
    {
        Pendente,
        Pago,
        Processando,
        Enviado,
        Entregue,
        Cancelado
    }

    public class Compra
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Utilizador { get; set; }
        
        [Required]
        public DateTime DataCompra { get; set; } = DateTime.Now;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        public EstadoCompra Estado { get; set; } = EstadoCompra.Pago;
        
        [StringLength(200)]
        public string? MoradaEntrega { get; set; }
        
        // Navigation properties
        public virtual ICollection<CompraItem>? Itens { get; set; }
        public virtual ICollection<TransacaoWallet>? Transacoes { get; set; }
    }
}
