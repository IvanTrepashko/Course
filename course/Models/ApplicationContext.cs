using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course.Models;

namespace course.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Client> Clients { get; set; } 
        public DbSet<Clothing> Clothes { get; set; } 
        public DbSet<Order> Orders { get; set; } 
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<IndividualOrder> IndividualOrders { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<course.Models.Supplier> Supplier { get; set; }
    }
}
