using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Data
{
    public class DbContext : IdentityDbContext<User>
    {
        public DbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected DbContext()
        {
        }
    }
}
