using Microsoft.EntityFrameworkCore;
using ErrorReporter.Entities;

namespace ErrorReporter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ErrorReport> ErrorReports => Set<ErrorReport>();
        public DbSet<ApiClient> ApiClients => Set<ApiClient>();
    }
}
