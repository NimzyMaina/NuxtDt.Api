using Microsoft.EntityFrameworkCore;
using NuxtDt.Api.Models;

namespace NuxtDt.Api.Persistence
{
    public class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public DbSet<Employees> Employees { get; set; }
    }
}