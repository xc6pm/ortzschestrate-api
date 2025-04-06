using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ortzschestrate.Data.Models;

namespace Ortzschestrate.Data
{
    public class DbContext : IdentityDbContext<User>, IDataProtectionKeyContext
    {
        public DbContext(DbContextOptions options) : base(options)
        {
        }

        protected DbContext()
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}