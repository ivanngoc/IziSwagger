namespace MockServer.Services;

public class DebugService : BackgroundService
{
    private readonly InitSeeder test;

    public DebugService(InitSeeder test)
    {
        this.test = test;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            Console.WriteLine("test runner");
        });
    }
}