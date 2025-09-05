using System.IO.Pipes;
using System.Text;

public class ClientConnection
{
    public string ClientId { get; }
    public StreamWriter Writer { get; }
    public StreamReader Reader { get; }
    public NamedPipeServerStream Stream { get; }
    public Queue<string> ReceivedMessages = new Queue<string>();
    public ClientConnection(string clientId, NamedPipeServerStream stream)
    {
        ClientId = clientId;
        Stream = stream;
        Writer = new StreamWriter(stream) { AutoFlush = true };
        Reader = new StreamReader(stream, leaveOpen: true);
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
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
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
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
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
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
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
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
        }
    }

    public void SendLine(string message)
    {
        try
        {
            Writer.WriteLine(message);
        }
        catch (Exception ex)
        {
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
        }
    }
    public void Send(string message)
    {
        try
        {
            Writer.Write(message);
        }
        catch (Exception ex)
        {
            DebugWriter.WriteLine("Server", $"Failed to send to {ClientId}: {ex.Message}");
        }
    }
#nullable enable
    public string? Receive()
    {
        try
        {
            if (ReceivedMessages.Count > 0)
            {
                return ReceivedMessages.Dequeue();
            }
            return null;
        }
        catch (Exception ex)
        {
            DebugWriter.WriteLine("Server", $"Failed to receive from {ClientId}: {ex.Message}");
            return null;
        }
    }
    public string WaitForMessage(int timeout = 30000)
    {
        DateTime dateTime = DateTime.Now;
        while (true)
        {
            string? message = Receive();
            if (message != null)
            {
                return message;
            }
            if ((DateTime.Now - dateTime).TotalMilliseconds > timeout)
            {
                DebugWriter.WriteLine("Server", $"Timeout waiting for message from {ClientId}");
                return "error: timeout";
            }
            Thread.Sleep(50); // Avoid busy waiting
        }
    }
}