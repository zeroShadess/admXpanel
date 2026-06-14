using AdminPanel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<ProjectTechnology> ProjectTechnologies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectTechnology>()
                .HasKey(pt => new { pt.ProjectId, pt.TechnologyId });

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTechnologies)
                .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Technology)
                .WithMany(t => t.ProjectTechnologies)
                .HasForeignKey(pt => pt.TechnologyId);

            // Seed Technologies
            modelBuilder.Entity<Technology>().HasData(
                new Technology { Id = 1, Name = "ASP.NET Core MVC" },
                new Technology { Id = 2, Name = "Entity Framework Core" },
                new Technology { Id = 3, Name = "MSSQL" },
                new Technology { Id = 4, Name = "Bootstrap 5" },
                new Technology { Id = 5, Name = "C#" },
                new Technology { Id = 6, Name = "HTML5 & CSS3" },
                new Technology { Id = 7, Name = "JavaScript" },
                new Technology { Id = 8, Name = "React" },
                new Technology { Id = 9, Name = "Angular" },
                new Technology { Id = 10, Name = "Vue.js" }
            );
        }
    }
}
