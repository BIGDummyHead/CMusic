namespace CMusic.Interfaces;

public interface IPlayList
{
    public Stream CurrentAudioStream { get; protected set; }

    public Queue<Stream> QueuedAudioStreams { get; protected set; }

    public Stack<Stream> PreviouslyPlayedStream { get; protected set; }

    public bool Next();
    public bool Previous();
}