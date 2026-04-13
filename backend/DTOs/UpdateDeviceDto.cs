using System.ComponentModel.DataAnnotations;

namespace DeviceManager.API.DTOs;

public class UpdateDeviceDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string OperatingSystem { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string OsVersion { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Processor { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string RamAmount { get; set; } = string.Empty;

    public string? Description { get; set; }
}