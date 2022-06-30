using System.Collections.Concurrent;

namespace ColoryrServer.ASP;

public static class BytesPool
{
    private static readonly ConcurrentDictionary<int, ConcurrentBag<byte[]>> _BytesPool = new ConcurrentDictionary<int, ConcurrentBag<byte[]>>();

    public static byte[] Rent(int size) => _BytesPool.GetOrAdd(size, new ConcurrentBag<byte[]>()).TryTake(out var bytes) ? bytes : new byte[size];

    public static void Return(byte[] bytes) => _BytesPool.GetOrAdd(bytes.Length, new ConcurrentBag<byte[]>()).Add(bytes);
}
public static class HttpClientUtils
{
    public static async Task CopyToAsync(this Stream source, Stream dest, CancellationToken cancellationToken)
    {
        var buffer = BytesPool.Rent(256 * 256 * 256);
        var totalBytes = 0L;
        int bytesCopied;
        try
        {
            do
            {
                bytesCopied = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                await dest.WriteAsync(buffer, 0, bytesCopied, cancellationToken).ConfigureAwait(false);
                totalBytes += bytesCopied;
            } while (bytesCopied > 0);
        }
        finally
        {
            BytesPool.Return(buffer);
        }
    }
}
