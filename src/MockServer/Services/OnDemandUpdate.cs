using MockServer.Databases;

namespace MockServer.Controllers;

/// <summary>
/// обнолвение и проверка объектов при обращении
/// </summary>
public class OnDemandUpdate : IDisposable
{
    private readonly IServiceProvider provider;
    private IServiceScope? scope;
    private EquironDbContext? context;
    private bool isDisposed;

    public OnDemandUpdate(IServiceProvider provider)
    {
        this.provider = provider;
        this.isDisposed = true;
        Init();
    }

    public void Init()
    {
        if (!this.isDisposed) throw new ObjectDisposedException("Must be disposed to be init");
        this.isDisposed = false;
        this.scope = provider.CreateScope();
        this.context = scope.ServiceProvider.GetRequiredService<EquironDbContext>();
    }

    public void Dispose()
    {
        if (isDisposed) throw new ObjectDisposedException("Already disposed");
        this.isDisposed = true;
        this.scope!.Dispose();
        this.scope = default;
        this.context = default;
    }

    public bool IsExpired()
    {
        throw new NotImplementedException();
    }
}