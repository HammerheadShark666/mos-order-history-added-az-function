using Microservice.Order.Function.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservice.Order.Function.Domain;

[Table("MSOS_OrderStatus")]
public class OrderStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public Enums.OrderStatus Id { get; set; }

    [MaxLength(75)]
    [Required]
    public string Status { get; set; }
}