using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SimpleProxy.Core;

public static class SimpleProxyExtensions
{
    /// <summary>
    ///     Добавить HTTP-прокси
    /// </summary>
    /// <remarks>Требуется реализация IProxyAutoConfiguration</remarks>
    /// <seealso cref="SimpleProxy.Core.Pac.IProxyAutoConfiguration"/>
    public static IServiceCollection AddSimpleProxy(this IServiceCollection services, Action<ProxyServerConfiguration>? callback = null)
    {
        services.Configure(callback ?? (p =>
        {
            p.Port = 1234;
            p.Timeout = 5;
        }));
        services.AddHostedService<ProxyWorker>();
        services.AddTransient(provider => new TcpListener(IPAddress.Any, provider.GetService<IOptions<ProxyServerConfiguration>>()!.Value.Port));
        services.AddTransient<HttpListener>();
        return services;
    }
}
