using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using NuxtDt.Api.Models;

namespace NuxtDt.Api.Persistence
{
    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApiContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApiContext>>()))
            {
                // Look for any board games.
                if (context.Employees.Any())
                {
                    return;   // Data was already seeded
                }

                context.Employees.AddRange(SampleData.GetSampleData());

                context.SaveChanges();
            }
        }
        
    }
}