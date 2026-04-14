using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Models;

namespace DeviceManager.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasOne(d => d.AssignedUser)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Device>()
            .Property(d => d.OperatingSystem)
            .HasColumnName("OperatingSystem");

        modelBuilder.Entity<AppUser>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}