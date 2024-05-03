using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace API.Models {
    public class Vendor {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required."), MaxLength(100, ErrorMessage = "Name must be less than 100 characters."), RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only have letters A-Z.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email is invalid."), Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Country code is required."), MaxLength(5, ErrorMessage = "Country code is invalid.")]
        public string CountryCode { get; set; }

        [Phone(ErrorMessage = "Phone number is invalid."), Required(ErrorMessage = "Phone number is required"), RegularExpression(@"^[0-9]+$", ErrorMessage = "Name can only have numbers 0-9.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Password is required."), MinLength(6, ErrorMessage = "Password must be atleast 6 characters"), MaxLength(32, ErrorMessage = "Password is limited to 32 characters."),
         Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string Password { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? DateModified { get; set; }
    }
}
