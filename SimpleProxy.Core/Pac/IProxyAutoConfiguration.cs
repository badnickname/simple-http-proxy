namespace SimpleProxy.Core.Pac;

/// <summary>
///     Авто-конфигурация политик для прокси
/// </summary>
public interface IProxyAutoConfiguration
{
    /// <summary>
    ///     Получить политику
    /// </summary>
    public IProxyPolicy GetPolicy(PolicyContext context);

    /// <summary>
    ///     Создать политику для прямого подключения
    /// </summary>
    public static IProxyPolicy CreateDirect() => new ProxyDirectConfiguration();

    /// <summary>
    ///     Создать политику для подключения через HTTP-прокси
    /// </summary>
    public static IProxyPolicy CreateHttpProxy() => new ProxyHttpConfiguration();
}