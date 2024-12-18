﻿using Microsoft.AspNetCore.Identity;

namespace Ab108Uniqlo.Models;

public class AppUser : IdentityUser
{
    public string? Fullname { get; set; }
    public string? Address { get; set; }
    public string? ProfileImageUrl { get; set; }
    public ICollection<ProductRating>? ProductRatings { get; set; }
    public ICollection<ProductComment>? ProductComments { get; set; }
}
