using Microsoft.Extensions.DependencyInjection;
using SimpleProxy.Core.Pac;

namespace SimpleProxy.Js;

public static class SimpleProxyJsExtensions
{
    /// <summary>
    ///     Добавить реализацию JS конфигуратора политик 
    /// </summary>
    /// <seealso cref="IProxyAutoConfiguration"/>
    public static IServiceCollection AddSimpleProxyJs(this IServiceCollection services, Action<SimpleProxyJsSettings>? setupAction = null)
    {
        services.Configure(setupAction ?? (_ => { }));
        services.AddTransient<IProxyAutoConfiguration, ProxyAutoConfiguration>();
        return services;
    }
}