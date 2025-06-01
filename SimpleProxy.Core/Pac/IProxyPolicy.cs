using System.Net.Sockets;

namespace SimpleProxy.Core.Pac;

/// <summary>
///     Политика для прокси (отвечает за подключение к удаленному хосту)
/// </summary>
public interface IProxyPolicy
{
    /// <summary>
    ///     Подключиться к хосту
    /// </summary>
    /// <param name="socket">Сокет клиента</param>
    /// <param name="context">Контекст</param>
    /// <param name="memory">Первое сообщение</param>
    /// <param name="token">Токен отмены</param>
    Task<Socket> ConnectAsync(Socket socket, PolicyContext context, Memory<byte> memory, CancellationToken token = default); 
}