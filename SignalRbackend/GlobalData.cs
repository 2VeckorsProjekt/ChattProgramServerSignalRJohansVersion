namespace SignalRbackend;

public static class GlobalData
{
    public static Dictionary<string, User> users = DataBaseHelper.ReadAllUsers();
    public static Dictionary<string, string> loggedIn = new Dictionary<string, string>(); // ID to username
    public static Dictionary<string, string> IDtoIP = new Dictionary<string, string>(); // ID to remote IP
    public static Dictionary<string, string> nameToId = new Dictionary<string, string>(); // username to ID
}