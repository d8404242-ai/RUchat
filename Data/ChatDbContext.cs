using Microsoft.EntityFrameworkCore;
using RuCHAT.Server.Models;

namespace RuCHAT.Server.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }
    public DbSet<Message> Messages => Set<Message>();
}