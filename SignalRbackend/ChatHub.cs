using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace SignalRbackend;

public sealed class ChatHub : Hub<IChatClient>
{
    static int GuestCount = 0;

    public async Task Login(string username, string password)
    {
        bool isGuest = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);

        if (isGuest)
        {
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
        // Sänd meddelande till alla andra klienter (utom avsändaren) med avsändarnamnet inkluderat
        await Clients.Others.ReceiveMessage($"{senderName}: {message}");
    }

    public async Task SendMessage2(string message)
    {
        string senderName = GlobalData.loggedIn[Context.ConnectionId];
        await Clients.Others.ReceiveMessage2($"{senderName}: {message}");
    }

    public async Task SendMessage3(string message) 
    {
        string senderName = GlobalData.loggedIn[Context.ConnectionId];
        await Clients.Others.ReceiveMessage3($"{senderName}: {message}");
    }

    public async Task RelayPM(string message)
    {
        // Dela upp meddelandet i mottagare och innehåll
        string[] mess = message.Split(';');
        string sender = GlobalData.loggedIn[Context.ConnectionId];
        string receiver = mess[0];
        string content = mess[1];

        Console.WriteLine($"SEND PM - {sender}; {content} to {receiver}");

        string receiverID = GlobalData.nameToId[receiver];
        await Clients.Client(receiverID).ReceivePM($"{sender}; {content}");
    }

    // Skicka en lista med namn på inloggade användare
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
    }
}
