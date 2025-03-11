using System.Diagnostics;
using System.Runtime.InteropServices;
using CMusic.WASAPI;
using CMusic.Interfaces;

namespace CMusic;

public class Player : IPlayer
{
    public Player(WAVEFORMATEX format, IMMDevice? chosenDevice = null, bool forceFormat = true)
    {
        this.format = format;

        HRESULT hr;

        if (chosenDevice == null)
        {
            hr = Tools.GetDefaultDevice(out device);

            if (hr != HRESULT.S_OK)
                throw new Exception($"Error getting default device: 0x{hr:X}");

        }

        hr = Tools.ActivateDevice(device, out audioClient);

        if (hr != HRESULT.S_OK)
            throw new Exception($"Error activating device: 0x{hr:X}");

        //player.AudioStream = await player.Load(path) ?? throw new Exception("Audio Stream could not be read.");

        hr = audioClient.GetDevicePeriod(out long defaultDevicePeriod, out _);

        if (hr != HRESULT.S_OK)
            throw new Exception($"Error getting device period: 0x{hr:X}");

        if (forceFormat)
        {
            hr = Tools.EnsureFormat(audioClient, ref format);

            if (hr != HRESULT.S_OK)
                throw new Exception($"Error ensuring format: 0x{hr:X}");
        }

        hr = audioClient.Initialize(
            AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED,
            0, // No flags
            (uint)defaultDevicePeriod,
            0,
            ref format,
            IntPtr.Zero);

        if (hr != HRESULT.S_OK)
            throw new Exception($"Error initializing client: 0x{hr:X}");


        hr = Tools.GetRenderClient(audioClient, out audioRenderClient);

        if (hr != HRESULT.S_OK)
            throw new Exception($"Error getting service render client: 0x{hr:X}");

    }

    private IMMDevice device;
    private IAudioClient audioClient;

    private IAudioRenderClient audioRenderClient;

    private WAVEFORMATEX format;
    public WAVEFORMATEX Format { get => format; set => format = value; }

    public Stream CurrentAudioStream { get; set; }

    public Queue<Stream> QueuedAudioStreams { get; set; } = new Queue<Stream>();

    public Stack<Stream> PreviouslyPlayedStream { get; set; } = new Stack<Stream>();


    public bool Pause()
    {
        if (this.audioClient == null)
            throw new Exception("Invalid audio client");

        return audioClient.Stop() == HRESULT.S_OK;
    }

    public bool Next()
    {

        if (CurrentAudioStream != null)
        {
            PreviouslyPlayedStream.Push(CurrentAudioStream);
        }

        if (QueuedAudioStreams.Count == 0)
            return false;

        CurrentAudioStream = QueuedAudioStreams.Dequeue();
        Restart();
        return true;
    }

    public bool Previous()
    {
        throw new NotImplementedException("This feature is currently not implemented, due to non-seekable streaming");
        if (PreviouslyPlayedStream.Count == 0)
            return false;

        if (CurrentAudioStream != null)
            QueuedAudioStreams = new Queue<Stream>(QueuedAudioStreams.Prepend(CurrentAudioStream));

        CurrentAudioStream = PreviouslyPlayedStream.Pop();
        Restart();
        return true;
    }

    const uint REFTIMES_PER_SEC = 10000000;
    const uint REFTIMES_PER_MILLISEC = 10000;
    public async Task<PlayerResult> Play(CancellationToken cancellationToken = default)
    {
        if (CurrentAudioStream == null && !Next())
            throw new Exception("Nothing left to play in Queue");

        HRESULT hr = audioClient.GetBufferSize(out uint numBufferFrames);
        if (hr != HRESULT.S_OK)
            throw new Exception($"Could not get player's buffer: 0x{hr:X}");

        uint actualHNSDuration = REFTIMES_PER_SEC * numBufferFrames / Format.nSamplesPerSec;

        if (audioClient.Start() != HRESULT.S_OK)
            throw new Exception("Could not start the audio player...");

        while (!cancellationToken.IsCancellationRequested)
        {
            Thread.Sleep((int)(actualHNSDuration / REFTIMES_PER_MILLISEC / 2));

            audioClient.GetCurrentPadding(out uint numPaddingFrames);

            uint numFramesRequested = numBufferFrames - numPaddingFrames;
            if (numFramesRequested == 0)
                continue;

            if (audioRenderClient.GetBuffer(numFramesRequested, out IntPtr ptrBuffer) != HRESULT.S_OK || ptrBuffer == IntPtr.Zero)
                break;

            int copyingBytes = (int)numFramesRequested * format.nBlockAlign;
            byte[] audioBuffer = new byte[copyingBytes];

            int totalRead = 0;
            while (totalRead < copyingBytes)
            {
                int audioRead = await CurrentAudioStream.ReadAsync(audioBuffer, totalRead, copyingBytes - totalRead);

                if (audioRead == 0)
                    break;

                totalRead += audioRead;
            }

            Marshal.Copy(audioBuffer, 0, ptrBuffer, totalRead);

            uint framesWritten = (uint)(totalRead / format.nBlockAlign);
            audioRenderClient.ReleaseBuffer(framesWritten, 0);

            if(totalRead <= 0)
                break;
        }

        Thread.Sleep((int)(actualHNSDuration / REFTIMES_PER_MILLISEC / 2));
        audioClient.Stop();
        return cancellationToken.IsCancellationRequested ? PlayerResult.Cancelled : PlayerResult.Finished;
    }

    public bool Restart()
    {
        if(CurrentAudioStream.CanSeek)
            CurrentAudioStream.Position = 0;
            
        return true;
    }

    //Free up objects here.
    public void Dispose()
    {
        Marshal.ReleaseComObject(this.audioRenderClient);
        Marshal.ReleaseComObject(this.audioClient);
        CurrentAudioStream.Dispose();
    }
}