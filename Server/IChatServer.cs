namespace Server
{
    public interface IChatServer
    {
        void BroadcastMessage(int id, string message);
    }
}
