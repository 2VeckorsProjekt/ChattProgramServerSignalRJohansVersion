using Microsoft.AspNetCore.SignalR;
using System.Collections;

namespace SignalRbackend;

public sealed class ChatHub : Hub <IChatClient>
{
    List<User> users = new List<User> { new User("Hampus", "1234"), new User("Marcus", "456"), new User("Farbror Nils", "789") };
    static Dictionary<string, User> loggedIn = new Dictionary<string, User>();

    /*
    public ChatHub()
    {
        loggedIn = new ConcurrentDictionary<string, User>();
    }
    */
    public static int GuestCount = 0;

    public async Task Login(string username, string password)
    {
        bool isGuest = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);

        if (isGuest)
        {
            // Increment and use the guest count to generate a unique guest username
            username = $"Guest{++GuestCount}";
            var guestUser = new User(username, ""); // Assuming an empty password for guests
            loggedIn.TryAdd(Context.ConnectionId, guestUser);
        }
        else
        {
            // Existing login logic for registered users
            foreach (var item in users)
            {
                if (item.Correct(username, password))
                {
                    loggedIn.TryAdd(Context.ConnectionId, item);
                    break; // Exit loop once the user is found and added
                }
            }
        }

        // Announcement for a user joining
        string joinMessage = isGuest ? $"{username} has joined!" : $"{loggedIn[Context.ConnectionId].name} has joined!";
        await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(joinMessage);

        // Debugging: Log current logged-in users
        foreach (var item in loggedIn)
        {
            Console.WriteLine($"{item.Key} : {item.Value.name}");
        }
    }


    public override async Task OnConnectedAsync()
    {
        // TODO: inloggningsfunktion
        // Clients.Client => anropar funktion Login på klienten => man fyller i namn och lösenord => skickas tillbaka och bekräftas              
    }

    public async Task SendMessage(string message)
    {
        string senderName;

        // Check if the sender is logged in and has a name; otherwise, label them as "Guest"
        if (loggedIn.ContainsKey(Context.ConnectionId))
        {
            senderName = loggedIn[Context.ConnectionId].name; // Use the user's name if logged in
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
