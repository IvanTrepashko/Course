using course.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course.ViewModels
{
    public class OrderViewModel
    {
        public string Category { get; set; }
        public string Material { get; set; }
        public double Cost { get; set; }
        public DateTimeOffset OrderingTime { get; set; }
        public DateTimeOffset SewingTime { get; set; }
        public string MasterName{get;set;}
        public string ClientName { get; set; }
        public int OrderId { get; set; }
        public byte[] Image { get; set; }
        public int isCompleted { get; set; }
        public string PhoneNumber { get; set; }
    }
}
