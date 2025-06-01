namespace SimpleProxy.Core.Pac;

public interface IProxyAutoConfiguration
{
    public IProxyPolicy GetPolicy(PolicyContext context);
}