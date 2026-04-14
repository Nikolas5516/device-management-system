namespace DeviceManager.API.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int? UserId { get; set; }

    // Navigation property
    public User? User { get; set; }
}