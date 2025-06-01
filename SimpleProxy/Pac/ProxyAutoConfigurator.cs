namespace SimpleProxy.Pac;

public sealed class ProxyAutoConfigurator : IProxyAutoConfigurator
{
    public IProxyConfiguration Get(string host, int port)
    {
        if (host.Contains("youtube") || host.Contains("googlevideo") || host.Contains("ggpht") || host.Contains("ytimg"))
        {
            return new ProxyHttpConfiguration("127.0.0.1", 1080);
        }

        return new ProxyDirectConfiguration();
    }
}