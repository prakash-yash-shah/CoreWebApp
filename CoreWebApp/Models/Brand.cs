﻿using System.ComponentModel.DataAnnotations;

namespace CoreWebApp.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        public string? Name{ get; set; }
        public string? Category { get; set; }
        public int IsActive { get; set; }
    }
}
