namespace SignalRbackend;

public static class AdminTools
{
    public static void CommandInput()
    {
        Console.WriteLine("admintools.cs");

        while (true)
        {
            string command = AdminInput("Enter command: ");
            string[] cs = command.Split(' ');

            if (command == "list banned ips")
            {
                var blacklist = DataBaseHelper.ReadAllBlackList();
            }


            else if (command == "list all users")
            {
                var userlist = DataBaseHelper.ReadAllUsers();
            }

            else if (command == "list all chatrooms")
            {
                var chatroomlist = DataBaseHelper.ReadAllChatRooms();
            }

        }
    }

    static string AdminInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim().ToLower();
    }
}
