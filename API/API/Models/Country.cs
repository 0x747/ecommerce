using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models {
    public class Country {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO3 { get; set; }
        public string Phone_Code { get; set; }
    }
}