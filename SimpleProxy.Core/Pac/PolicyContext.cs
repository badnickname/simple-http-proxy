namespace SimpleProxy.Core.Pac;

/// <summary>
///     Контекст подключения по политике
/// </summary>
public sealed class PolicyContext
{
    /// <summary>
    ///     Удаленный хост
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    ///     Порт удаленного хоста
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    ///     Url, к которому производится подключение
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Хост прокси (если есть)
    /// </summary>
    public string ProxyHost { get; set; }

    /// <summary>
    ///     Порт прокси (если есть)
    /// </summary>
    public int ProxyPort { get; set; }
}