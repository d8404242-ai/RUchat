namespace RuCHAT.Server.Models;

public class ChatUser
{
    public string ConnectionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
}