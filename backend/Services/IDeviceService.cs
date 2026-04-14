using DeviceManager.API.DTOs;

namespace DeviceManager.API.Services;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(CreateDeviceDto dto);
    Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task<DeviceDto?> AssignToUserAsync(int deviceId, int userId);
    Task<DeviceDto?> UnassignAsync(int deviceId, int userId);
}