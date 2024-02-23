using Microsoft.AspNetCore.SignalR;

namespace SignalRbackend;

public sealed class ChatHub : Hub<IChatClient>
{
    /*
    static Dictionary<string, User> users = DataBaseHelper.ReadAllUsers();
    static Dictionary<string, string> loggedIn = new Dictionary<string, string>(); // ID to username
    static Dictionary<string, string> IDtoIP = new Dictionary<string, string>(); // ID to remote IP
    */

    static int GuestCount = 0;

    public async Task Login(string username, string password)
    {
        bool isGuest = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);

        if (isGuest)
        {
            // Increment and use the guest count to generate a unique guest username
            username = $"Guest{++GuestCount}";
            
            GlobalData.loggedIn.TryAdd(Context.ConnectionId, username);
        }
        else
        {
            if (GlobalData.users.ContainsKey(username) && GlobalData.users[username].Correct(username, password))
            {
                GlobalData.loggedIn.TryAdd(Context.ConnectionId, username);
            }
            else
            {
                isGuest = true;
                username = $"Guest{++GuestCount}";
                GlobalData.loggedIn.TryAdd(Context.ConnectionId, username);
            }
        }

        // Announcement for a user joining
        string joinMessage = isGuest ? $"{username} has joined!" : $"{GlobalData.loggedIn[Context.ConnectionId]} has joined!";
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(joinMessage);
    }

    public override async Task OnConnectedAsync()
    {
        string ip = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
        string id = Context.ConnectionId;

        GlobalData.IDtoIP.TryAdd(id, ip);
    }

    public async Task SendMessage(string message)
    {
        string senderName = GlobalData.loggedIn[Context.ConnectionId];
         
        // Broadcast the message to all clients, excluding the sender, with the sender's name
        await Clients.Others.ReceiveMessage($"{senderName}: {message}");
    }

    public async Task SendMessage2(string message)
    {
        string senderName = GlobalData.loggedIn[Context.ConnectionId];

        // Broadcast the message to all clients, excluding the sender, with the sender's name
        await Clients.Others.ReceiveMessage2($"{senderName}: {message}");
    }

    public async Task SendMessage3(string message)
    {
        string senderName = GlobalData.loggedIn[Context.ConnectionId];

        // Broadcast the message to all clients, excluding the sender, with the sender's name
        await Clients.Others.ReceiveMessage3($"{senderName}: {message}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string ip = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
        string id = Context.ConnectionId;
        string name = GlobalData.loggedIn[id];

        Console.WriteLine($"{id} - {name} - {ip} disconnected");

        //GlobalData.loggedIn.Remove(id);
        //GlobalData.IDtoIP.Remove(id);        
    }
}
