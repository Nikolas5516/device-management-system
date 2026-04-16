namespace DeviceManager.API.Services;

public interface IAiDescriptionService
{
    Task<string> GenerateDescriptionAsync(string name, string manufacturer,
        string type, string os, string processor, string ram);
}