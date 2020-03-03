namespace Arcadia.Assistant.DeviceRegistry.Contracts.Models
{
    public class DeviceRegistryEntry
    {
        public DeviceType DeviceType { get; set; } = new DeviceType();

        public DeviceId DeviceId { get; set; } = new DeviceId();
    }
}