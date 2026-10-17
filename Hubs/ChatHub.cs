using Microsoft.AspNetCore.SignalR;
using RuCHAT.Server.Data;
using RuCHAT.Server.Models;

namespace RuCHAT.Server.Hubs;

public class ChatHub : Hub
{
    private readonly ChatDbContext _db;
    public static readonly List<ChatUser> OnlineUsers = new();

    public ChatHub(ChatDbContext db) => _db = db;

    public async Task JoinChat(string userName)
    {
        var user = new ChatUser { ConnectionId = Context.ConnectionId, UserName = userName };
        lock (OnlineUsers)
        {
            var existing = OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (existing != null) OnlineUsers.Remove(existing);
            OnlineUsers.Add(user);
        }

        var history = _db.Messages
            .OrderByDescending(m => m.Timestamp)
            .Take(50).OrderBy(m => m.Timestamp).ToList();

        await Clients.Caller.SendAsync("LoadHistory", history);
        await Clients.All.SendAsync("UserList", OnlineUsers.Select(u => u.UserName).ToList());
        await Clients.Others.SendAsync("UserJoined", userName);
    }

    public async Task SendMessage(string userName, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var msg = new Message
        {
            UserName = userName,
            Text = text,
            Timestamp = DateTime.UtcNow,
            ConnectionId = Context.ConnectionId
        };
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? userName = null;
        lock (OnlineUsers)
        {
            var user = OnlineUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null) { userName = user.UserName; OnlineUsers.Remove(user); }
        }
        if (userName != null)
        {
            await Clients.All.SendAsync("UserLeft", userName);
            await Clients.All.SendAsync("UserList", OnlineUsers.Select(u => u.UserName).ToList());
        }
        await base.OnDisconnectedAsync(exception);
    }
}