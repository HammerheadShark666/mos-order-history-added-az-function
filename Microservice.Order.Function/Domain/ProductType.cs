using Microservice.Order.Function.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservice.Order.Function.Domain;

[Table("MSOS_ProductType")]
public class ProductType
{ 
     [Key]
    public Enums.ProductType Id { get; set; }

    [MaxLength(75)]
    [Required]
    public string Name { get; set; }
}