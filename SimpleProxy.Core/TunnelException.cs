namespace SimpleProxy.Core;

/// <summary>
///     Исключение, связанное с выполнением туннелирования запроса
/// </summary>
public sealed class TunnelException(string? message) : Exception(message);