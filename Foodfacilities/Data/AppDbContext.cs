using Foodfacilities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Foodfacilities.SeedData
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<FoodFacility> FoodFacilities { get; set; }
    }
}
