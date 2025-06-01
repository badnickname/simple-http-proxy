using Microsoft.ClearScript.V8;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleProxy.Core.Pac;

namespace SimpleProxy.Js;

/// <summary>
///     Реализация IProxyAutoConfiguration с JS функцией
/// </summary>
internal sealed class ProxyAutoConfiguration(IOptions<SimpleProxyJsSettings> options, ILogger<IProxyAutoConfiguration> logger) : IProxyAutoConfiguration, IDisposable
{
    private readonly V8ScriptEngine _engine = new();
    private string? _code;

    public void Dispose() => _engine.Dispose();

    public IProxyPolicy GetPolicy(PolicyContext context)
    {
        dynamic result = _engine.Evaluate(GetCode() + $"\nFindProxyForURL('{context.Url}', '{context.Host}')");
        if (result is string code && code.StartsWith("PROXY"))
        {
            var strs = code.Split(' ').LastOrDefault()?.Trim().Split(':');
            if (strs is null || strs.Length == 0)
            {
                logger.LogInformation("Pac: direct for {Host}", context.Host);
                return IProxyAutoConfiguration.CreateDirect();
            }

            if (strs.Length > 1)
            {
                logger.LogInformation("Pac: proxy to {ProxyHost}:{ProxyPort} for {Host}", strs[0], strs[1], context.Host);
                context.ProxyHost = strs[0];
                context.ProxyPort = int.Parse(strs[1]);
                return IProxyAutoConfiguration.CreateHttpProxy();
            }

            logger.LogInformation("Pac: proxy to {ProxyHost}:{ProxyPort} for {Host}", strs[0], 80, context.Host);
            context.ProxyHost = strs[0];
            context.ProxyPort = 80;
            return IProxyAutoConfiguration.CreateHttpProxy();
        }

        logger.LogInformation("Pac: direct for {Host}", context.Host);
        return IProxyAutoConfiguration.CreateDirect();
    }

    private string GetCode()
    {
        if (_code is not null) return _code;

        _code = File.ReadAllText(options.Value.FilePath);
        return _code;
    }
}