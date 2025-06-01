using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SimpleProxy.Core.Pac;

namespace SimpleProxy.Core;

internal sealed class HttpListener(TcpListener listener, ILogger<HttpTunnel> logger) : IDisposable
{
    public void Start() => listener.Start();

    public async Task<HttpTunnel> AcceptAsync(CancellationToken cancellationToken = default)
    {
        var socket = await listener.AcceptSocketAsync(cancellationToken);
        return new HttpTunnel(socket, logger, new PolicyContext());
    }

    public void Dispose() => listener.Dispose();
}