using System.Text;
using CMusic.WASAPI;

namespace CMusic;

public static class FileTools
{
    public static async Task<ParsedFile> ReadWaveFileAsync(string path)
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

        int offset = 20;

        WAVEFORMATEX format = new WAVEFORMATEX
        {
            wFormatTag = GetUShort(data, ref offset),
            nChannels = GetUShort(data, ref offset),
            nSamplesPerSec = GetUInt(data, ref offset),
            nAvgBytesPerSec = GetUInt(data, ref offset),
            nBlockAlign = GetUShort(data, ref offset),
            wBitsPerSample = GetUShort(data, ref offset),
            cbSize = 0
        };

        byte[] audioBytes = new byte[data.Length - 44];
        Array.Copy(data, 44, audioBytes, 0, audioBytes.Length);

        return new ParsedFile(format, audioBytes);
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