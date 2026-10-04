namespace Producer.API.Models;

public class OrderEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}