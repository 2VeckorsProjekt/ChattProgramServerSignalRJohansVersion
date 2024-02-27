using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

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
            GlobalData.nameToId.TryAdd(username, Context.ConnectionId);
        }
        else
        {
            if (GlobalData.users.ContainsKey(username) && GlobalData.users[username].Correct(username, password))
            {
                GlobalData.loggedIn.TryAdd(Context.ConnectionId, username);
                GlobalData.nameToId.TryAdd(username, Context.ConnectionId);
            }
            else
            {
                isGuest = true;
                username = $"Guest{++GuestCount}";
                GlobalData.loggedIn.TryAdd(Context.ConnectionId, username);
                GlobalData.nameToId.TryAdd(username, Context.ConnectionId);
            }
        }

        // Announcement for a user joining
        string joinMessage = isGuest ? $"{username} has joined!" : $"{GlobalData.loggedIn[Context.ConnectionId]} has joined!";
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(joinMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage2(joinMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage3(joinMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveClientUpdate($"Connected;{username}");

        foreach (var item in GlobalData.loggedIn)
        {
            Console.WriteLine(item.Key + ":" + item.Value);
        }
        foreach (var item in GlobalData.nameToId)
        {
            Console.WriteLine(item.Key + ":" + item.Value);
        }
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

    public async Task RelayPM(string message)
    {
        string[] mess = message.Split(';');
        string sender = GlobalData.loggedIn[Context.ConnectionId];
        string receiver = mess[0];
        string content = mess[1];

        Console.WriteLine($"SEND PM - {sender}; {content} to {receiver}");

        string receiverID = GlobalData.nameToId[receiver];
        await Clients.Client(receiverID).ReceivePM($"{sender}; {content}");
    }

    public async Task SendLoggedInList()
    {
        string list = string.Empty;

        foreach (var item in GlobalData.loggedIn)
        {
            list += item.Value + ";";
        }

        await Clients.Caller.ReceiveClientList(list);

        Console.WriteLine(list);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string ip = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
        string id = Context.ConnectionId;
        string name = GlobalData.loggedIn[id];

        Console.WriteLine($"{id} - {name} - {ip} disconnected");

        string leaveMessage = $"{name} has disconnected!";
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(leaveMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage2(leaveMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage3(leaveMessage);
        await Clients.AllExcept(Context.ConnectionId).ReceiveClientUpdate($"Disconnected;{name}");

        GlobalData.nameToId.Remove(name);
        GlobalData.loggedIn.Remove(id);
        //GlobalData.IDtoIP.Remove(id);        
    }
}
