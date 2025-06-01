using SimpleProxy.Pac;

namespace SimpleProxy;

internal sealed class ProxyWorker(HttpListener listener, IProxyAutoConfigurator pac) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        listener.Start();
        while (!stoppingToken.IsCancellationRequested)
        {
            var tunnel = await listener.AcceptAsync(stoppingToken);
            await tunnel.ConnectToExternal(pac, stoppingToken);
        }
    }
}