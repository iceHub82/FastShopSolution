using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastShop.Data.Entities;

public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public ICollection<CartItem>? CartItems { get; set; }
}