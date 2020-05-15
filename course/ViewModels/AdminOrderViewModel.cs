using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course.ViewModels
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string Category { get; set; }
        public string Material { get; set; }
        public double Cost { get; set; }
        public DateTimeOffset OrderingTime { get; set; }
        public DateTimeOffset SewingTime { get; set; }
        public string ClientName { get; set; }
    }
}
