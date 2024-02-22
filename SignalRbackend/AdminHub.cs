using Microsoft.AspNetCore.SignalR;

namespace SignalRbackend;

public class AdminHub : Hub <IChatClient>
{
    static Dictionary<string, User> users = new Dictionary<string, User> // Username to user object
    {
        { "Bosse", new User(0, "Bosse", 60, "ctrlaltdelete") },
    };
    static Dictionary<string, string> loggedIn = new Dictionary<string, string>();

    public async Task Login(string username, string password)
    {
        if (users.ContainsKey(username) && users[username].Correct(username, password))
        {
            loggedIn.TryAdd(Context.ConnectionId, username);
        }
        else
        {
            
        }
    }

    public async Task SendMessage(string message)
    {
        string reply = string.Empty;

        if (message == "list banned ips")
        {
            reply += "LIST BANNED IPS";
            /*
            var blacklist = DataBaseHelper.ReadAllBlackList();
            reply += "Banned IP:s \n";
            foreach (var item in blacklist) 
            {
                reply += $"{item}\n";
            }
            */
        }
        else if (message == "list all users")
        {
            reply += "LIST ALL USERS";
            /*
            var userlist = DataBaseHelper.ReadAllUsers();
            reply += "Users: \n";
            foreach (var item in userlist)
            {
                reply += $"{item}\n";
            }
            */
        }
        else if (message == "list all chatrooms")
        {
            reply += "LIST CHATROOMS";
            /*
            var chatroomlist = DataBaseHelper.ReadAllChatRooms();
            reply += "Chatrooms: \n";
            foreach (var item in chatroomlist)
            {
                reply += $"{item}\n";
            }
            */
        }


        await Clients.Client(Context.ConnectionId).ReceiveMessage($"{reply}");
    }
}
