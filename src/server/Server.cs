
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using ExtensionLibGlobal;

public class PipeServer
{
    public delegate Task PackageReceivedHandler(string clientId, string packageId, byte[] data);
    public string PipeName { get; }
    #nullable enable
    public event PackageReceivedHandler? OnPackageReceived;
    private readonly ConcurrentDictionary<string, ClientConnection> _clients = new();

    public PipeServer(string pipeName)
    {
        PipeName = pipeName;

        Directory.GetFiles("logs", "debug_*.log").ToList().ForEach(File.Delete);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        DebugWriter.WriteLine("Server", $"Listening on pipe '{PipeName}'...");

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
                DebugWriter.WriteLine("Server", "Client connected.");

                // Handle the connected client in a background task
                _ = Task.Run(() => HandleClientAsync(pipeStream, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                pipeStream.Dispose(); // Clean up
            }
            catch (Exception ex)
            {
                DebugWriter.WriteLine("Server", $"Error accepting connection: {ex.Message}");
                pipeStream.Dispose();
            }
        }
    }


    private async Task HandleClientAsync(NamedPipeServerStream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new DebugStreamReader(stream, leaveOpen: true);
            using var writer = new DebugStreamWriter(stream) { AutoFlush = true };

            string? clientId = await reader.ReadLineAsync();
            reader.SetFile(clientId ?? "unknown");
            writer.SetFile(clientId ?? "unknown");
            if (string.IsNullOrWhiteSpace(clientId))
            {
                DebugWriter.WriteLine("Server", "Client did not send a clientId. Disconnecting.");
                return;
            }

            DebugWriter.WriteLine("Server", $"Client connected with ID: {clientId}");

            var clientConnection = new ClientConnection(clientId, stream);
            _clients[clientId] = clientConnection;

            DebugWriter.WriteLine("Server", $"Client connected with ID: {clientId}");

            var binaryStream = stream;

            clientConnection.SendLine("Server:READY");

            while (!cancellationToken.IsCancellationRequested && stream.IsConnected)
            {
                string? header = await reader.ReadLineAsync();
                if (header == null)
                    break;

                DebugWriter.WriteLine($"Server {clientId}", $"Header: {header}");

                var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (header.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    _clients.TryRemove(clientId, out _);
                    break;
                }

                if (parts.Length != 3 || !parts[0].Equals("Package", StringComparison.OrdinalIgnoreCase))
                {
                    _clients[clientId].ReceivedMessages.Enqueue(header);
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

                reader.AddToLogFile($"{Encoding.UTF8.GetString(buffer)}");

                if (OnPackageReceived != null)
                {
                    await OnPackageReceived.Invoke(clientId, packageId, buffer);
                }

                await writer.WriteLineAsync("Received package");
            }

            DebugWriter.WriteLine($"Server {clientId}", "Connection closed.");
        }
        catch (Exception ex)
        {
            DebugWriter.WriteLine("Server", $"Error: {ex.Message}");
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
            DebugWriter.WriteLine("Server", $"Client '{clientId}' not found.");
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
            DebugWriter.WriteLine("Server", $"Client '{clientId}' not found.");
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
            DebugWriter.WriteLine("Server", $"Client '{clientId}' not found.");
            return false;
        }
    }

    public bool SendPackageToClient(string clientId, PackageTypes packageType, string message)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            client.SendLine($"package {PackageId.PackageTypeToId(packageType)} {message.Length}");

            DebugWriter.WriteLine("Server", $"waiting for ack");
            string? ack = client.WaitForMessage();
            if (ack != "ack")
            {
                DebugWriter.WriteLine("Server", $"did not receive ack, got '{ack}'");
                return false;
            }

            DebugWriter.WriteLine("Server", $"sending data");
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            client.Stream.Write(bytes, 0, bytes.Length);
            client.Stream.Flush();

            string? response = client.WaitForMessage();
            if (!response.Equals("Received package"))
            {
                DebugWriter.WriteLine("Server", $"did not receive proper response, got '{response}'");
                return false;
            }
            return true;
        }
        else
        {
            DebugWriter.WriteLine("Server", $"Client '{clientId}' not found.");
            return false;
        }
    }

}
