
using Avalonia;

public class Program
{
    public static string ProjectName = "TestApp";
    public static PipeServer server;

    static int Main(string[] args)
    {
        Thread serverThread = new Thread(new ThreadStart(StartServer));
        serverThread.Name = "Server Thread";
        serverThread.Start();

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    static void StartServer()
    {
        server = new PipeServer("test_pipe");
        using CancellationTokenSource cts = new CancellationTokenSource();
        server.OnPackageReceived += PackageManeger.OnPackageReceived;

        _ = server.StartAsync(cts.Token);
    }

    static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
    .UsePlatformDetect().LogToTrace();
}