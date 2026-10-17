namespace RuCHAT.Server.Models;

public class Message
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ConnectionId { get; set; }
}