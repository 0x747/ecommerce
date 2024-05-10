using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models {
    public class ApiKey {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } 

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int AmountOfRequests { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int Limit { get; set; }
    }
}