﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastShop.Data.Entities;

public class CartItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int SizeId { get; set; }
    public Size? Size { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}