using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace API.Models {

    public enum ProductCategory {
        Electronics,
        Furniture,
        Books,
        Movies,
        VideoGames,
    }
    public class Product {
        [Key]
        public int Id { get; set; }

        public int VendorId { get; set; }

        [Required(ErrorMessage = "Name is required."), Range(1, 100, ErrorMessage = "Name cannot be more than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required."), Range(25, 250, ErrorMessage = "Description must be between 25 to 250 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product category is required.")]
        public ProductCategory Category { get; set; }

        [Required, Range(0.10, 100000, ErrorMessage = "Price must be between $0.10 and $100,000")]
        public int Price { get; set; }
    }
}