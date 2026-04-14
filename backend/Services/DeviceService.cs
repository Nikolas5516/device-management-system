using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Data;
using DeviceManager.API.DTOs;
using DeviceManager.API.Models;

namespace DeviceManager.API.Services;

public class DeviceService : IDeviceService
{
    private readonly AppDbContext _context;

    public DeviceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        return await _context.Devices
            .Include(d => d.AssignedUser)
            .Select(d => MapToDto(d))
            .ToListAsync();
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await _context.Devices
            .Include(d => d.AssignedUser)
            .FirstOrDefaultAsync(d => d.Id == id);

        return device == null ? null : MapToDto(device);
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
    {
        var device = new Device
        {
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            Type = dto.Type,
            OperatingSystem = dto.OperatingSystem,
            OsVersion = dto.OsVersion,
            Processor = dto.Processor,
            RamAmount = dto.RamAmount,
            Description = dto.Description
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return MapToDto(device);
    }

    public async Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return null;

        device.Name = dto.Name;
        device.Manufacturer = dto.Manufacturer;
        device.Type = dto.Type;
        device.OperatingSystem = dto.OperatingSystem;
        device.OsVersion = dto.OsVersion;
        device.Processor = dto.Processor;
        device.RamAmount = dto.RamAmount;
        device.Description = dto.Description;

        await _context.SaveChangesAsync();

        // Reload with user data
        await _context.Entry(device)
            .Reference(d => d.AssignedUser)
            .LoadAsync();

        return MapToDto(device);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return false;

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Devices
            .AnyAsync(d => d.Name.ToLower() == name.ToLower());
    }

    public async Task<DeviceDto?> AssignToUserAsync(int deviceId, int userId)
    {
        var device = await _context.Devices
            .Include(d => d.AssignedUser)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null) return null;

        // Device already assigned to someone
        if (device.AssignedUserId != null)
            return null;

        device.AssignedUserId = userId;
        await _context.SaveChangesAsync();

        await _context.Entry(device).Reference(d => d.AssignedUser).LoadAsync();
        return MapToDto(device);
    }

    public async Task<DeviceDto?> UnassignAsync(int deviceId, int userId)
    {
        var device = await _context.Devices
            .Include(d => d.AssignedUser)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null) return null;

        // Can only unassign if the device is assigned to this user
        if (device.AssignedUserId != userId)
            return null;

        device.AssignedUserId = null;
        await _context.SaveChangesAsync();

        return MapToDto(device);
    }

    private static DeviceDto MapToDto(Device device)
    {
        return new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Manufacturer = device.Manufacturer,
            Type = device.Type,
            OperatingSystem = device.OperatingSystem,
            OsVersion = device.OsVersion,
            Processor = device.Processor,
            RamAmount = device.RamAmount,
            Description = device.Description,
            AssignedUserId = device.AssignedUserId,
            AssignedUserName = device.AssignedUser?.Name
        };
    }
}