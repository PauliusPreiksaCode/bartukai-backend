using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using portal_backend.Enums;

namespace portal_backend.Entities;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public Customer Customer { get; set; } = null!;
    public FullOrder FullOrder { get; set; } = null!;
}