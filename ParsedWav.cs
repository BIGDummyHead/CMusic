using CMusic.WASAPI;

namespace CMusic;

public struct ParsedWav
{
    public Stream audioData;
    public WAVEFORMATEX format;
    public ParsedWav(WAVEFORMATEX format, Stream audio){
        this.format = format;
        this.audioData = audio;
    }

}