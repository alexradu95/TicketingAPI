using Microsoft.EntityFrameworkCore;
using WatersTicketingAPI.Models;

namespace WatersTicketingAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            :base(options)
        {

            this.Database.EnsureCreated();
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
    }
}