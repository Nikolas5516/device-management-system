using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Models;

namespace DeviceManager.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices { get; set; }
    public DbSet<User> Users { get; set; }

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
    }
}