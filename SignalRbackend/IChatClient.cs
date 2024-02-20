namespace SignalRbackend
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
        Task EnterUsername(string username);
        Task EnterPassword(string password);
    }
}
