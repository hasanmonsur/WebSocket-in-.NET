using Microsoft.AspNetCore.WebSockets;
using System.Net.WebSockets;
using System.Text;
using WebSocketServer.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});


var app = builder.Build();

app.UseWebSockets(); // Enable WebSocket support

app.UseMiddleware<WSMiddleware>();

app.MapGet("/", () => "Hello World!");

app.Run();
