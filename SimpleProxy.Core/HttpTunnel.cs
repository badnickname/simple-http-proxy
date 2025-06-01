using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SimpleProxy.Core.Pac;

namespace SimpleProxy.Core;

/// <summary>
///     Прокси тунель до удаленного хоста
/// </summary>
internal sealed class HttpTunnel(Socket socket, ILogger<HttpTunnel> logger, PolicyContext context) : IDisposable
{
    private readonly byte[] _buffer = new byte[65535];
    private readonly byte[] _secondBuffer = new byte[65535];

    private int _count;
    private Socket? _external;
    private string? _host;

    public int Timeout { get; set; }

    public void Dispose() => socket.Dispose();

    public async Task ConnectToExternalAsync(IProxyAutoConfiguration pac, CancellationToken cancellationToken = default)
    {
        var memory = new Memory<byte>(_buffer);
        var count = await socket.ReceiveAsync(memory, cancellationToken);
        var str = Encoding.UTF8.GetString(memory.Span[..count]);
        var matches = Regex.Matches(str, "Host: (.*)(\r|\n)");

        try
        {
            if (matches.Count == 0)
            {
                logger.LogInformation("Extracting host name is failed: {String}", str);
                throw new TunnelException("Extracting host name is failed");
            }

            var match = matches[0];
            if (!match.Success)
            {
                logger.LogInformation("Extracting host name is failed: {String}", str);
                throw new TunnelException("Extracting host name is failed");
            }

            var key = match.Groups[1].Value;
            _host = key.Trim();

            var strs = key.Trim().Split(':');
            context.Host = strs[0];
            context.Port = strs.Length > 1 ? int.Parse(strs[1]) : 80;
            context.Url = $"http://{context.Host}:{context.Port}";

            logger.LogInformation("Socket: start receiving for host {Host}", context.Host);

            var policy = pac.GetPolicy(context);

            var cs = new CancellationTokenSource();
            var css = CancellationTokenSource.CreateLinkedTokenSource(cs.Token, cancellationToken);
            await using var timer = new Timer(_ => cs.Cancel(), 0, TimeSpan.FromSeconds(Timeout), TimeSpan.FromSeconds(Timeout));
            _external = await policy.ConnectAsync(socket, context, memory[..count], css.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Failed connecting to host: {Host}", _host ?? "undefined");
        }
        catch (TunnelException e)
        {
            logger.LogWarning("Exception on creating tunnel: {Exception}", e.Message);
        }
        catch (Exception e)
        {
            logger.LogInformation("Failed receiving for host: {Host}", _host ?? "undefined");
            logger.LogWarning("Tunnel threw exception: {Exception}", e.Message);
            _external?.Dispose();
            socket.Dispose();
        }

        var secondMemory = new Memory<byte>(_secondBuffer);
        Task.Run(async () =>
        {
            try
            {
                var source = new CancellationTokenSource();
                var s = CancellationTokenSource.CreateLinkedTokenSource(source.Token);
                while (!CheckIfDisconnected(socket))
                {
                    await using var timer = new Timer(_ => source.Cancel(), 0, TimeSpan.FromSeconds(Timeout), TimeSpan.FromSeconds(Timeout));
                    var to = await _external!.ReceiveAsync(secondMemory, s.Token);
                    await socket.SendAsync(secondMemory[..to], s.Token);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Socket: stop receiving for host {Host} by timeout", _host ?? "undefined");
            }
            catch (Exception e)
            {
                logger.LogError("Socket: stop receiving for host {Host} because exception {Exception}", _host ?? "undefined", e.Message);
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
                var source = new CancellationTokenSource();
                var s = CancellationTokenSource.CreateLinkedTokenSource(source.Token);
                while (!CheckIfDisconnected(socket))
                {
                    await using var timer = new Timer(_ => source.Cancel(), 0, TimeSpan.FromSeconds(Timeout), TimeSpan.FromSeconds(Timeout));
                    var to = await socket.ReceiveAsync(memory, s.Token);
                    await _external!.SendAsync(memory[..to], s.Token);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Socket: stop receiving for host {Host} by timeout", _host ?? "undefined");
            }
            catch (Exception e)
            {
                logger.LogError("Socket: stop receiving for host {Host} because exception {Exception}", _host ?? "undefined", e.Message);
            }
            finally
            {
                Close();
            }
        }, cancellationToken);
    }

    private void Close()
    {
        if (++_count > 1)
        {
            _external?.Dispose();
            socket.Dispose();
            logger.LogInformation("Socket: stop receiving for host {Host}", _host ?? "undefined");
        }
    }

    private bool CheckIfDisconnected(Socket tcp)
    {
        if (tcp.Poll(100, SelectMode.SelectRead))
        {
            var buff = new byte[1];

            if (tcp.Receive(buff, SocketFlags.Peek) == 0) return true;
        }

        return false;
    }
}