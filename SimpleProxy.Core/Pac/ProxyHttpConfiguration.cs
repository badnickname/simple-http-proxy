using System.Net.Sockets;

namespace SimpleProxy.Core.Pac;

public sealed class ProxyHttpConfiguration : IProxyPolicy
{
    public async Task<Socket> ConnectAsync(Socket socket, PolicyContext context, Memory<byte> memory, CancellationToken token = default)
    {
        var external = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await external.ConnectAsync(context.ProxyHost, context.ProxyPort, token);
        await external.SendAsync(memory, token);
        return external;
    }
}
