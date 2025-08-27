using System.IO.Pipes;
using System.Text;

public class ClientConnection
{
    public string ClientId { get; }
    public StreamWriter Writer { get; }
    public NamedPipeServerStream Stream { get; }

    public ClientConnection(string clientId, NamedPipeServerStream stream)
    {
        ClientId = clientId;
        Stream = stream;
        Writer = new StreamWriter(stream) { AutoFlush = true };
    }

    public async Task SendAsync(string message)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message, 0, message.Length);
            await SendAsync(bytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Failed to send to {ClientId}: {ex.Message}");
        }
    }
    public async Task SendAsync(byte[] message)
    {
        try
        {
            await Stream.WriteAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Failed to send to {ClientId}: {ex.Message}");
        }
    }

    public void SendLine(byte[] message)
    {
        string msg = Encoding.UTF8.GetString(message);
        try
        {
            Writer.WriteLine(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Failed to send to {ClientId}: {ex.Message}");
        }
    }
        public void Send(byte[] message)
    {
        string msg = Encoding.UTF8.GetString(message);
        try
        {
            Writer.Write(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Failed to send to {ClientId}: {ex.Message}");
        }
    }
}