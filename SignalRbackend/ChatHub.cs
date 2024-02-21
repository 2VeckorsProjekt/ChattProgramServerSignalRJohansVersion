using Microsoft.AspNetCore.SignalR;

namespace SignalRbackend;

public sealed class ChatHub : Hub<IChatClient>
{
    static Dictionary<string, User> users = new Dictionary<string, User>
    {
        { "Hampus", new User(0, "Hampus", 20, "1234") },
        { "Marcus", new User(1, "Marcus", 50, "456") },
        { "Johan", new User(2, "Johan", 150, "789") }
    };
    static Dictionary<string, string> loggedIn = new Dictionary<string, string>();

    static int GuestCount = 0;

    public async Task Login(string username, string password)
    {
        bool isGuest = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);

        if (isGuest)
        {
            // Increment and use the guest count to generate a unique guest username
            username = $"Guest{++GuestCount}";
            
            loggedIn.TryAdd(Context.ConnectionId, username);
        }
        else
        {
            if (users.ContainsKey(username) && users[username].Correct(username, password))
            {
                loggedIn.TryAdd(Context.ConnectionId, username);
            }
            else
            {
                isGuest = true;
                username = $"Guest{++GuestCount}";
                loggedIn.TryAdd(Context.ConnectionId, username);
            }
        }

        // Announcement for a user joining
        string joinMessage = isGuest ? $"{username} has joined!" : $"{loggedIn[Context.ConnectionId]} has joined!";
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(joinMessage);

        // Debugging: Log current logged-in users
        foreach (var item in loggedIn)
        {
            Console.WriteLine($"{item.Key} : {item.Value}");
        }
    }


    public override async Task OnConnectedAsync()
    {
                      
    }

    public async Task SendMessage(string message)
    {
        string senderName;

        // Check if the sender is logged in and has a name; otherwise, label them as "Guest"
        if (loggedIn.ContainsKey(Context.ConnectionId))
        {
            senderName = loggedIn[Context.ConnectionId]; // Use the user's name if logged in
        }
        else
        {
            // For guests, you could also generate a unique guest name based on a stored counter or use the ConnectionId
            // This example simply uses "Guest" for simplicity, but consider enhancing this for unique identification if needed
            senderName = "Guest";
        }

        // Debugging: Log the sender and message to the console (optional, for server-side debugging)
        Console.WriteLine($"{senderName}: {message}");

        // Broadcast the message to all clients, including the sender, with the sender's name
        await Clients.Others.ReceiveMessage($"{senderName}: {message}");
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
