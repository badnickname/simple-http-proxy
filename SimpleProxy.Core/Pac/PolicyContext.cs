namespace SimpleProxy.Core.Pac;

public sealed class PolicyContext
{
    public string Host { get; set; }

    public int Port { get; set; }

    public string Url { get; set; }

    public string ProxyHost { get; set; }

    public int ProxyPort { get; set; }
}