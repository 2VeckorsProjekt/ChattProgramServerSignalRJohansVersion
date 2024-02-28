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

        builder.Services.AddEndpointsApiExplorer(); // Aktivera API discovery f�r Swagger-dokumentation
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        
        var app = builder.Build();

        // Aktivera Swagger och SwaggerUI endast i utvecklingsmilj�n
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Definiera en POST-endpoint f�r att skicka meddelanden till alla klienter
        app.MapPost("broadcast", async (string message, IHubContext<ChatHub, IChatClient> context) =>
        {
            await context.Clients.All.ReceiveMessage(message);
        });

        app.UseHttpsRedirection();

        app.MapHub<ChatHub>("/chathub");
        app.MapHub<AdminHub>("/admin");

        app.Run();
    }
}
