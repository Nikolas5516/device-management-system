using Microsoft.AspNetCore.Mvc;
using DeviceManager.API.DTOs;
using DeviceManager.API.Services;

namespace DeviceManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
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
}