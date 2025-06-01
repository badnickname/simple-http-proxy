using System.Net.Sockets;

namespace SimpleProxy;

public sealed class HttpListener(TcpListener listener) : IDisposable
{
    public void Start() => listener.Start();

    public async Task<HttpTunnel> AcceptAsync(CancellationToken cancellationToken = default)
    {
        var socket = await listener.AcceptSocketAsync(cancellationToken);
        return new HttpTunnel(socket);
    }

    public void Dispose() => listener.Dispose();
}