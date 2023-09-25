using System.Text;

namespace System.IO;

public static class StreamExtensions
{
    private static readonly int NOT_FOUND_INDEX = -1;   
    
    public static long IndexOf(this Stream stream, string value, long startIndex = 0)
    {
        stream.Seek(startIndex, SeekOrigin.Begin);
        var searchValue = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(value));
        var buffer = new Span<byte>(new byte[searchValue.Length - 1]);
        int _byte;        
        while ((_byte = stream.ReadByte()) >= 0)
        {
            if (searchValue[0] == _byte)
            {                
                long length = stream.Read(buffer);
                if (searchValue[^1] == buffer[^1] && buffer.SequenceEqual(searchValue[1..]))
                {
                    return stream.Seek(-(length + 1), SeekOrigin.Current);
                }
                else
                {
                    int p = buffer.IndexOf(searchValue[0]);
                    if (p >= 0)
                    {
                        stream.Seek(-(length - p), SeekOrigin.Current);
                    }
                }
            }
        }

        return NOT_FOUND_INDEX;
    }

    public static long LastIndexOf(this Stream stream, string value, long startIndex)
    {
        var searchValue = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(value));
        startIndex -= 1;
        stream.Seek(startIndex, SeekOrigin.Begin);        
        var buffer = new Span<byte>(new byte[searchValue.Length]);
        int _byte;
        while ((_byte = stream.ReadByte()) >= 0)
        {
            if (searchValue[^1] == _byte)
            {
                if (stream.Position < searchValue.Length) break;
                long length = stream.Seek(-searchValue.Length, SeekOrigin.Current);
                stream.Read(buffer);
                if (searchValue[0] == buffer[0] && buffer.SequenceEqual(searchValue))
                {
                    return length;
                }
                else
                {
                    int p = buffer.LastIndexOf(searchValue[0]);
                    if (p >= 0)
                    {
                        stream.Seek(-(length - p), SeekOrigin.Current);
                    }
                    else
                    {
                        stream.Seek(-length, SeekOrigin.Current);
                    }
                }
            }
            else
            {
                if (stream.Position < 2) break;
                stream.Seek(-2, SeekOrigin.Current);
            }
        }

        return NOT_FOUND_INDEX;
    }

    public static long LastIndexOf(this Stream stream, string value) => LastIndexOf(stream, value, stream.Length);

    public static int GetBufferSize(string filename)
    {
        return new FileInfo(filename).Length switch
        {
            > 818937856 => 1048576,
            > 104857600 => 524288,
            > 10485760 => 131072,
            > 1048576 => 81920,
            > 524288 => 65536,
            _ => 0
        };
    }
}
