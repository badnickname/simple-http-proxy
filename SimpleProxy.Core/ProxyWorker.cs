using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleProxy.Core.Pac;

namespace SimpleProxy.Core;

/// <summary>
///     Воркер для создания новых туннелей
/// </summary>
internal sealed class ProxyWorker(HttpListener listener, IOptions<ProxyServerConfiguration> options, ILogger<ProxyWorker> logger, IProxyAutoConfiguration pac) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Now listening proxy on: {Url}", $"http://localhost:{options.Value.Port}");
        listener.Start();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tunnel = await listener.AcceptAsync(stoppingToken);
                tunnel.Timeout = options.Value.Timeout;
                await tunnel.ConnectToExternalAsync(pac, stoppingToken);
            }
        }
        finally
        {
            logger.LogInformation("Proxy is stopping...");
            listener.Dispose();
        }
    }
}