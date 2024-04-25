using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebCoder.Application.Identity;
using WebCoder.Domain.Models;

namespace WebCoder.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public virtual DbSet<ProjectRepository> ProjectRepositories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var projectRepository = modelBuilder.Entity<ProjectRepository>();

        projectRepository.HasKey(pr => pr.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}