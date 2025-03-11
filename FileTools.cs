using System.Text;
using CMusic.WASAPI;

namespace CMusic;

public static class FileTools
{
    public static async Task<ParsedWav> ReadWaveFileAsync(string path)
    {
        if (!File.Exists(path) || !Path.GetExtension(path).Equals(".wav", StringComparison.OrdinalIgnoreCase))
            throw new Exception($"Reading {path} returned invalid results");

        byte[] data = await File.ReadAllBytesAsync(path);

        if (data.Length < 44
        || !Encoding.ASCII.GetString(data, 0, 4).Equals("RIFF")
        || !Encoding.ASCII.GetString(data, 8, 4).Equals("WAVE"))
        {
            throw new Exception("Invalid WAV file");
        }

        WAVEFORMATEX format = GetWavFormat(data);

        byte[] audioBytes = new byte[data.Length - 44];
        Array.Copy(data, 44, audioBytes, 0, audioBytes.Length);

        return new ParsedWav(format, new MemoryStream(audioBytes));
    }

    public static async Task<ParsedWav> ReadWaveFileURLAsync(string path)
    {
        if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            throw new FormatException($"{path} was an invalid uri");

        using HttpClient cli = new HttpClient();

        Stream fullData = await cli.GetStreamAsync(path);

        byte[] buffer = new byte[44];

        //this reads the first 4
        int read = await fullData.ReadAsync(buffer, 0, 44, default);
        
        if(read < 44)
            throw new Exception("Invalid read amount on httpclient... Could not get Wav header information");

        //fullData.Seek(44, SeekOrigin.Begin);

        WAVEFORMATEX format = GetWavFormat(buffer);
        return new ParsedWav (format, fullData);
    }

    private static WAVEFORMATEX GetWavFormat(byte[] wavHeader){
        int offset = 20;
        return new WAVEFORMATEX
        {
            wFormatTag = GetUShort(wavHeader, ref offset),
            nChannels = GetUShort(wavHeader, ref offset),
            nSamplesPerSec = GetUInt(wavHeader, ref offset),
            nAvgBytesPerSec = GetUInt(wavHeader, ref offset),
            nBlockAlign = GetUShort(wavHeader, ref offset),
            wBitsPerSample = GetUShort(wavHeader, ref offset),
            cbSize = 0
        };
    }


    private static ushort GetUShort(byte[] data, ref int offset)
    {
        ushort ret = BitConverter.ToUInt16(data, offset);
        offset += 2;
        return ret;
    }

    private static uint GetUInt(byte[] data, ref int offset)
    {
        uint ret = BitConverter.ToUInt32(data, offset);
        offset += 4;
        return ret;
    }
}