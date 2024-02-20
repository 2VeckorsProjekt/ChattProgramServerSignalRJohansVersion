using Microsoft.AspNetCore.SignalR;
using System.Collections;

namespace SignalRbackend;

public sealed class ChatHub : Hub <IChatClient>
{
    List<User> users = new List<User> { new User("Kjell", "123"), new User("Bosse", "456"), new User("Farbror Nils", "789") };
    static Dictionary<string, User> loggedIn = new Dictionary<string, User>();

    /*
    public ChatHub()
    {
        loggedIn = new ConcurrentDictionary<string, User>();
    }
    */
    public async Task Login(string username, string password)
    {        

        foreach (var item in users)
        {
            if (item.Correct(username, password))
            {
                loggedIn.TryAdd(Context.ConnectionId, item);

            }
        }

        if (loggedIn.ContainsKey(Context.ConnectionId))
        {

            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage($"{loggedIn[Context.ConnectionId].name} has joined!");
            

        }
        else
        {

            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage($"{Context.ConnectionId} has joined!");
        }

        foreach (var item in loggedIn) { Console.WriteLine(item.Key + " : " + item.Value.name); }
    }

    public override async Task OnConnectedAsync()
    {
        // TODO: inloggningsfunktion
        // Clients.Client => anropar funktion Login på klienten => man fyller i namn och lösenord => skickas tillbaka och bekräftas              
    }

    public async Task SendMessage(string message)
    {

        foreach (var item in loggedIn) { Console.WriteLine(item.Key + " : " + item.Value.name); }

        if (loggedIn.ContainsKey(Context.ConnectionId))
        {
            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage($"{loggedIn[Context.ConnectionId].name}: {message}");
        }
        else
        {
            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage($"{Context.ConnectionId}: {message}");
        }

        
        //await Clients.All.ReceiveMessage($"{Context.ConnectionId}: {message}");
    }

    public async Task EnterUsername(string username)
    {
        await Clients.Client(Context.ConnectionId).ReceiveMessage("Enter your username: ");
        
    }

    public async Task EnterPassword(string username)
    {
        await Clients.Client(Context.ConnectionId).ReceiveMessage("Enter your password: ");
    }

    /*public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
    }
    */
}
