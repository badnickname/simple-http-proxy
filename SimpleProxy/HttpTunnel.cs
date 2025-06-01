using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using SimpleProxy.Pac;

namespace SimpleProxy;

public sealed class HttpTunnel(Socket socket) : IDisposable
{
    private readonly byte[] _buffer = new byte[65535];
    private readonly byte[] _secondBuffer = new byte[65535];
    private Socket? _external;
    private string? _host;

    public void Dispose() => socket.Dispose();

    public async Task ConnectToExternal(IProxyAutoConfigurator pac, CancellationToken cancellationToken = default)
    {
        var memory = new Memory<byte>(_buffer);
        var count = await socket.ReceiveAsync(memory, cancellationToken);
        var str = Encoding.UTF8.GetString(memory.Span[..count]);
        var matches = Regex.Matches(str, "Host: (.*)(\r|\n)");

        try
        {
            if (matches.Count == 0)
            {
                throw new Exception($"Invalid host: {str}");
            }

            var match = matches[0];
            if (!match.Success) throw new Exception($"Invalid host: {str}");

            var key = match.Groups[1].Value;
            _host = key.Trim();

            var strs = key.Trim().Split(':');
            var host = strs[0];
            var port = strs.Length > 1 ? int.Parse(strs[1]) : 80;

            Console.WriteLine("START: SOCKET " + host);

            var policy = pac.Get(host, port);

            _external = await policy.ConnectAsync(socket, host, port, memory[..count], cancellationToken);

            
        }
        catch
        {
            Console.WriteLine("FAILED: SOCKET " + _host);
            _external?.Dispose();
            socket.Dispose();
        }

        var secondMemory = new Memory<byte>(_secondBuffer);
        Task.Run(async () =>
        {
            try
            {
                while (!CheckIfDisconnected(socket))
                {
                    var source = new CancellationTokenSource();
                    await using var timer = new Timer(_ => source.Cancel(), 0, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                    var to = await _external.ReceiveAsync(secondMemory, source.Token);
                    await socket.SendAsync(secondMemory[..to], source.Token);
                }
            }
            finally
            {
                Close();
            }
        }, cancellationToken);

        Task.Run(async () =>
        {
            try
            {
                while (!CheckIfDisconnected(socket))
                {
                    var source = new CancellationTokenSource();
                    await using var timer = new Timer(_ => source.Cancel(), 0, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                    var to = await socket.ReceiveAsync(memory, source.Token);
                    await _external.SendAsync(memory[..to], source.Token);
                }
            }
            finally
            {
                Close();
            }
        }, cancellationToken);
    }

    private int _count;
    private void Close()
    {
        if (++_count > 1)
        {
            _external?.Dispose();
            socket.Dispose();
            Console.WriteLine("DONE: SOCKET " + _host);
        }
    }
    
    private bool CheckIfDisconnected(Socket tcp)
    {
        if (tcp.Poll(100, SelectMode.SelectRead))
        {
            byte[] buff = new byte[1];

            if (tcp.Receive(buff, SocketFlags.Peek) == 0)
            {
                // Client disconnected
                return true;
            }
        }
        
        return false;
    }
}