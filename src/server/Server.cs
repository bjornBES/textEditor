
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;

public class Server
{
    public static void StartServer()
    {

    }
}

public class PipeServer
{
    public delegate Task PackageReceivedHandler(string clientId, string packageId, byte[] data);
    public string PipeName { get; }
    public event PackageReceivedHandler? OnPackageReceived;
    private readonly ConcurrentDictionary<string, ClientConnection> _clients = new();

    public PipeServer(string pipeName)
    {
        PipeName = pipeName;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[Server] Listening on pipe '{PipeName}'...");

        while (!cancellationToken.IsCancellationRequested)
        {
            var pipeStream = new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous
            );

            try
            {
                // Wait for a client to connect before spawning handler
                await pipeStream.WaitForConnectionAsync(cancellationToken);
                Console.WriteLine("[Server] Client connected.");

                // Handle the connected client in a background task
                _ = Task.Run(() => HandleClientAsync(pipeStream, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                pipeStream.Dispose(); // Clean up
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error accepting connection: {ex.Message}");
                pipeStream.Dispose();
            }
        }
    }


    private async Task HandleClientAsync(NamedPipeServerStream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            string? clientId = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(clientId))
            {
                Console.WriteLine("[Server] Client did not send a clientId. Disconnecting.");
                return;
            }

            Console.WriteLine($"[Server] Client connected with ID: {clientId}");

            var clientConnection = new ClientConnection(clientId, stream);
            _clients[clientId] = clientConnection;

            Console.WriteLine($"[Server] Client connected with ID: {clientId}");

            var binaryStream = stream;

            while (!cancellationToken.IsCancellationRequested && stream.IsConnected)
            {
                string? header = await reader.ReadLineAsync();
                if (header == null)
                    break;

                Console.WriteLine($"[Server] [{clientId}] Header: {header}");

                if (header.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    _clients.TryRemove(clientId, out _);
                    break;
                }

                var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3 || !parts[0].Equals("Package", StringComparison.OrdinalIgnoreCase))
                {
                    await writer.WriteLineAsync("error: invalid header");
                    continue;
                }

                string packageId = parts[1];
                if (!int.TryParse(parts[2], out int size) || size <= 0)
                {
                    await writer.WriteLineAsync("error: invalid size");
                    continue;
                }

                await writer.WriteLineAsync("ok");

                byte[] buffer = new byte[size];
                int totalRead = 0;

                while (totalRead < size)
                {
                    int bytesRead = await binaryStream.ReadAsync(buffer, totalRead, size - totalRead, cancellationToken);
                    if (bytesRead == 0)
                        return;

                    totalRead += bytesRead;
                }

                if (OnPackageReceived != null)
                {
                    await OnPackageReceived.Invoke(clientId, packageId, buffer);
                }

                await writer.WriteLineAsync("Received package");
            }

            Console.WriteLine($"[Server] [{clientId}] Connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Error: {ex.Message}");
        }
        finally
        {
            stream.Dispose();
        }
    }

    public async Task<bool> SendToClientAsync(string clientId, string message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            await client.SendAsync(message);
            return true;
        }
        else
        {
            Console.WriteLine($"[Server] Client '{clientId}' not found.");
            return false;
        }
    }

    public async Task<bool> SendToClientAsync(string clientId, byte[] message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            await client.SendAsync(message);
            return true;
        }
        else
        {
            Console.WriteLine($"[Server] Client '{clientId}' not found.");
            return false;
        }
    }

    public bool SendToClient(string clientId, byte[] message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            client.Send(message);
            return true;
        }
        else
        {
            Console.WriteLine($"[Server] Client '{clientId}' not found.");
            return false;
        }
    }

}
