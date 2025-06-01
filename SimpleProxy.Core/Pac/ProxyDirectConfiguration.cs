using System.Net.Sockets;
using System.Text;

namespace SimpleProxy.Core.Pac;

public sealed class ProxyDirectConfiguration : IProxyPolicy
{
    public async Task<Socket> ConnectAsync(Socket socket, PolicyContext context, Memory<byte> memory, CancellationToken token = default)
    {
        var str = Encoding.UTF8.GetString(memory.Span);
        var external = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await external.ConnectAsync(context.Host, context.Port, token);
        
        if (!str.StartsWith("CONNECT"))
        {
            await external.SendAsync(memory, token);
        }
        else
        {
            await socket.SendAsync("HTTP/1.1 200 Connection Established\r\n\r\n"u8.ToArray(), token);
        }
        
        return external;
    }
}