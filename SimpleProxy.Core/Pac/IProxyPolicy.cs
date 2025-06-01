using System.Net.Sockets;

namespace SimpleProxy.Core.Pac;

public interface IProxyPolicy
{
    Task<Socket> ConnectAsync(Socket socket, PolicyContext context, Memory<byte> memory, CancellationToken token = default); 
}