using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SixNations.Server.Models;

namespace SixNations.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RequirementEstimation>().HasData(
                new { RequirementEstimationID = 1, RequirementEstimationName = "XS" },
                new { RequirementEstimationID = 2, RequirementEstimationName = "Small" },
                new { RequirementEstimationID = 3, RequirementEstimationName = "Medium" },
                new { RequirementEstimationID = 5, RequirementEstimationName = "Large" },
                new { RequirementEstimationID = 8, RequirementEstimationName = "XL" },
                new { RequirementEstimationID = 13, RequirementEstimationName = "XXL" });

            builder.Entity<RequirementPriority>().HasData(
                new { RequirementPriorityID = 1, RequirementPriorityName = "Must" },
                new { RequirementPriorityID = 2, RequirementPriorityName = "Should" },
                new { RequirementPriorityID = 3, RequirementPriorityName = "Could" },
                new { RequirementPriorityID = 4, RequirementPriorityName = "Wont" });

            builder.Entity<RequirementStatus>().HasData(
                new { RequirementStatusID = 1, RequirementStatusName = "Prioritised" },
                new { RequirementStatusID = 2, RequirementStatusName = "WIP" },
                new { RequirementStatusID = 3, RequirementStatusName = "Test" },
                new { RequirementStatusID = 4, RequirementStatusName = "Done" });

            builder.Entity<Requirement>().Property(r => r.Story).IsRequired();
        }

        public DbSet<Requirement> Requirement { get; set; }

        public DbSet<RequirementEstimation> RequirementEstimation { get; set; }

        public DbSet<RequirementPriority> RequirementPriority { get; set; }

        public DbSet<RequirementStatus> RequirementStatus { get; set; }
    }
}
