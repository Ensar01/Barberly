using Barberly.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Barberly.Database
{
    public class BarberlyDbContext : IdentityDbContext<User>
    {
        public BarberlyDbContext(DbContextOptions<BarberlyDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
