namespace Consumer.API.Models;

public class EventResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Payload { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
}