﻿namespace Ab108Uniqlo.Models;

public class Brand : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product>? Products { get; set; }
}
