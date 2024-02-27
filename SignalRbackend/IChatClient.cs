namespace SignalRbackend
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
        Task ReceiveMessage2(string message);
        Task ReceiveMessage3(string message);
        Task ReceiveClientList(string message);
        Task ReceiveClientUpdate(string message);
        Task ReceivePM(string message);
    }
}
