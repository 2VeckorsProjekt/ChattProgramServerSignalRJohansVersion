using Microsoft.AspNetCore.SignalR;
using Swashbuckle;

namespace SignalRbackend;

public class Program
{
    public static async Task Main(string[] args)
    {
        DataBaseHelper.InitializeDatabase();
        var blacklist = DataBaseHelper.ReadAllUsers();
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSignalR();

        
        var app = builder.Build();

        if (app.Environment.IsDevelopment()) 
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapPost("broadcast", async (string message, IHubContext<ChatHub, IChatClient> context) =>
        {
            await context.Clients.All.ReceiveMessage(message);

            //return Results.NoContent();
        });

        app.UseHttpsRedirection();

        app.MapHub<ChatHub>("/chathub");
        app.MapHub<AdminHub>("/admin");

        app.Run();
    }
}
