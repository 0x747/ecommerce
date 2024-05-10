using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ApiLog
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public DateTime Timestamp { get; set; }
    }
}