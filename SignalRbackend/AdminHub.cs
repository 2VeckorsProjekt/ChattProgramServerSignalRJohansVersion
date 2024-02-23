using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRbackend;

public class AdminHub : Hub <IChatClient>
{
    static Dictionary<string, User> users = new Dictionary<string, User> // Username to user object
    {
        { "admin", new User(0, "admin", 60, "password") },
    };
    static Dictionary<string, string> loggedIn = new Dictionary<string, string>();

    public async Task Login(string username, string password)
    {
        if (users.ContainsKey(username) && users[username].Correct(username, password))
        {
            loggedIn.Add(Context.ConnectionId, username);
            await Clients.Client(Context.ConnectionId).ReceiveMessage($"{help}");
        }
        else
        {
            await Clients.Client(Context.ConnectionId).ReceiveMessage("Access denied");
        }

        
    }
    static string url = "wss://localhost:5001/chathub";
    
    HubConnection connection = new HubConnectionBuilder().WithUrl(url).Build();

    static string help => @"-- Commands:
        -- list banned ips - lists banned ips
        -- list all users - lists user accounts
        -- list all chatrooms - lists all chatrooms
        -- create acc /name:age:password/ - adds user account
        -- ban /username/ - adds ip to blacklist via username
        -- create room /chatroom/ - creates chatroom after restart
        -- remove acc /user/ - remove account
        -- unban /ip/ - unbans an ip idress
        -- remove room /chatroom/ - removes chatroom after restart";

    public async Task SendMessage(string message)
    {
        string[] mes = message.Split(' ');
        string reply = string.Empty;
        if (!loggedIn.ContainsKey(Context.ConnectionId))
        {
            reply += "Access denied!";
        }
        else if (message == "list banned ips") // list banned ips
        {
            var blacklist = DataBaseHelper.ReadAllBlackList();
            reply += "Banned IP:s \n";
            foreach (var item in blacklist)
            {
                reply += $"{item}\n";
            }
        }
        else if (message == "list all users") // list all users
        {
            var userlist = DataBaseHelper.ReadAllUsers();
            reply += "Users: \n";
            foreach (var item in userlist)
            {
                reply += $"{item.Value}\n";
            }
        }
        else if (message == "list all chatrooms") // list all chatrooms
        {
            var chatroomlist = DataBaseHelper.ReadAllChatRooms();
            reply += "Chatrooms: \n";
            foreach (var item in chatroomlist)
            {
                reply += $"{item}\n";
            }
        }
        else if (mes.Length == 3 && mes[0] == "create" && mes[1] == "acc") // create acc /user/
        {
            string[] acc = mes[2].Split(":");

            var newUser = new User(0, acc[0], int.Parse(acc[1]), acc[2]);

            if (!GlobalData.users.ContainsKey(newUser.UserName))
            {
                GlobalData.users.Add(newUser.UserName, newUser);
                DataBaseHelper.PushUser(newUser);

                reply += $"{newUser} pushed to db and chathub";
            }
            else
            {
                reply += $"{newUser} not pushed, name already in use";
            }                                  
        }
        else if (false) // ban /username/
        {

        }
        else if (false) // create room /chatroom/ - creates chatroom after restart
        {

        }
        else if (mes.Length == 3 && mes[0] == "remove" && mes[1] == "acc") // remove acc /user:age:password/ - remove account
        {
            string[] acc = mes[2].Split(":");

            DataBaseHelper.DeleteUser(acc[0], acc[1], acc[2]);
            reply += $"{mes[2]} removed";
        }
        else if (false) // unban /ip/ - unbans an ip idress
        {

        }
        else if (false) // remove room /chatroom/ - removes chatroom after restart
        {

        }
        else reply = $"Unknown command: {message}";

        await Clients.Client(Context.ConnectionId).ReceiveMessage($"{reply}");
    }

    static string AdminInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim().ToLower();
    }
}
