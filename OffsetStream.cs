namespace CMusic;

public sealed class OffsetStream 
{
    public int Offset { get; set; }
    public Stream Stream { get; private set; }

    public OffsetStream(Stream stream, int offset)
    {
        this.Stream = stream;
        this.Offset = offset;
    }

    public async Task ReadAsync()
    {
        
    }
}