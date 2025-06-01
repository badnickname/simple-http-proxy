using System.Net.Sockets;

namespace SimpleProxy.Pac;

public interface IProxyConfiguration
{
    Task<Socket> ConnectAsync(Socket socket, string host, int port, Memory<byte> memory, CancellationToken token = default); 
}