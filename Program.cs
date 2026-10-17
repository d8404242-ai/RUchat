using Microsoft.EntityFrameworkCore;
using RuCHAT.Server.Data;
using RuCHAT.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSignalR();
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlite("Data Source=chat.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    db.Database.EnsureCreated();
}

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();