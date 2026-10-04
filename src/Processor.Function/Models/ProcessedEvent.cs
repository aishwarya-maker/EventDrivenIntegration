namespace Processor.Function.Models;

public class ProcessedEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OriginalPayload { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}