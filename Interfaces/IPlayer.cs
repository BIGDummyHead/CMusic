using CMusic.WASAPI;

namespace CMusic.Interfaces;

public interface IPlayer : IPlayList, IDisposable
{
    public Task<PlayerResult> Play(CancellationToken cancellationToken);
    public bool Pause();
    public bool Restart();

    public WAVEFORMATEX Format { get; set; }
    
}