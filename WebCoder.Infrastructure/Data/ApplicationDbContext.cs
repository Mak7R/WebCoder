using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebCoder.Application.Identity;
using WebCoder.Infrastructure.Entities;

namespace WebCoder.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public virtual DbSet<ProjectRepositoryEntity> ProjectRepositories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var projectRepository = modelBuilder.Entity<ProjectRepositoryEntity>();

        projectRepository.HasKey(pr => pr.Id);
        projectRepository.HasIndex(pr => new { pr.OwnerId, pr.Title }).IsUnique();

        projectRepository
            .HasOne(pr => pr.Owner)
            .WithMany();
        
        base.OnModelCreating(modelBuilder);
    }
}