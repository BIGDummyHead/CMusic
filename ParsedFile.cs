using CMusic.WASAPI;

namespace CMusic;

public struct ParsedFile
{
    public byte[] audioData;
    public WAVEFORMATEX format;
    public ParsedFile(WAVEFORMATEX format, byte[] audio){
        this.format = format;
        this.audioData = audio;
    }

    public MemoryStream AsMemoryStream => new MemoryStream(audioData);
}