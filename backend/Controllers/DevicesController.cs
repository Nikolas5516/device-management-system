using Microsoft.AspNetCore.Mvc;
using DeviceManager.API.DTOs;
using DeviceManager.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DeviceManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IAiDescriptionService _aiService;

    public DevicesController(IDeviceService deviceService, IAiDescriptionService aiService)
    {
        _deviceService = deviceService;
        _aiService = aiService;
    }

    // GET: api/devices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    // GET: api/devices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetById(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null)
            return NotFound(new { message = $"Device with ID {id} not found." });

        return Ok(device);
    }

    // POST: api/devices
    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Create([FromBody] CreateDeviceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check for duplicate device name
        if (await _deviceService.ExistsByNameAsync(dto.Name))
            return Conflict(new { message = $"A device with the name '{dto.Name}' already exists." });

        var device = await _deviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
    }

    // PUT: api/devices/5
    [HttpPut("{id}")]
    public async Task<ActionResult<DeviceDto>> Update(int id, [FromBody] UpdateDeviceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var device = await _deviceService.UpdateAsync(id, dto);
        if (device == null)
            return NotFound(new { message = $"Device with ID {id} not found." });

        return Ok(device);
    }

    // DELETE: api/devices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _deviceService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Device with ID {id} not found." });

        return NoContent();
    }

    // POST: api/devices/5/assign
    [Authorize]
    [HttpPost("{id}/assign")]
    public async Task<ActionResult<DeviceDto>> Assign(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var device = await _deviceService.AssignToUserAsync(id, userId);

        if (device == null)
            return BadRequest(new { message = "Device not found or already assigned to another user." });

        return Ok(device);
    }

    // POST: api/devices/5/unassign
    [Authorize]
    [HttpPost("{id}/unassign")]
    public async Task<ActionResult<DeviceDto>> Unassign(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var device = await _deviceService.UnassignAsync(id, userId);

        if (device == null)
            return BadRequest(new { message = "Device not found or not assigned to you." });

        return Ok(device);
    }

    // POST: api/devices/5/generate-description
    [HttpPost("{id}/generate-description")]
    public async Task<ActionResult> GenerateDescription(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null)
            return NotFound(new { message = $"Device with ID {id} not found." });

        try
        {
            var description = await _aiService.GenerateDescriptionAsync(
                device.Name, device.Manufacturer, device.Type,
                device.OperatingSystem, device.Processor, device.RamAmount);

            // Save the description
            var updateDto = new DTOs.UpdateDeviceDto
            {
                Name = device.Name,
                Manufacturer = device.Manufacturer,
                Type = device.Type,
                OperatingSystem = device.OperatingSystem,
                OsVersion = device.OsVersion,
                Processor = device.Processor,
                RamAmount = device.RamAmount,
                Description = description
            };

            var updated = await _deviceService.UpdateAsync(id, updateDto);
            return Ok(new { description = description, device = updated });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to generate description.", error = ex.Message });
        }
    }

    // GET: api/devices/search?q=apple phone
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(await _deviceService.GetAllAsync());

        var results = await _deviceService.SearchAsync(q);
        return Ok(results);
    }
}