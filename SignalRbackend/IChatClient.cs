namespace SignalRbackend
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
