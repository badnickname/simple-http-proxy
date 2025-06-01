namespace SimpleProxy.Core;

/// <summary>
///     Конфигурация прокси сервера
/// </summary>
public sealed class ProxyServerConfiguration
{
    /// <summary>
    ///     Порт сервера
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    ///     Таймаут на соединения
    /// </summary>
    public int Timeout { get; set; }
}