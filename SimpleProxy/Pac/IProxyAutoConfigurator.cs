namespace SimpleProxy.Pac;

public interface IProxyAutoConfigurator
{
    public IProxyConfiguration Get(string host, int port);
}