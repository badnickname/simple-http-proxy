using System.Net.Sockets;

namespace SimpleProxy.Pac;

public sealed class ProxyHttpConfiguration(string host, int port) : IProxyConfiguration
{
    public async Task<Socket> ConnectAsync(Socket socket, string host1, int port1, Memory<byte> memory, CancellationToken token = default)
    {
        var external = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await external.ConnectAsync("127.0.0.1", 1080, token);
        await external.SendAsync(memory, token);
        return external;
    }
}
