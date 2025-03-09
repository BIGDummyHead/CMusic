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

    public Queue<Stream> QueuedAudioStreams { get;  set; } = new Queue<Stream>();

    public Stack<Stream> PreviouslyPlayedStream { get;  set; } = new Stack<Stream>();

    private int dataOffset = 0;


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
        if (PreviouslyPlayedStream.Count == 0)
            return false;

        if (CurrentAudioStream != null)
            QueuedAudioStreams = new Queue<Stream>(QueuedAudioStreams.Prepend(CurrentAudioStream));

        CurrentAudioStream = PreviouslyPlayedStream.Pop();
        Restart();
        return true;
    }

    public async Task<PlayerResult> Play(CancellationToken cancellationToken = default)
    {
        if (CurrentAudioStream == null && !Next())
            throw new Exception("Nothing left to play in Queue");

        HRESULT hr = audioClient.Start();
        if (hr != HRESULT.S_OK)
            throw new Exception($"Could not start the player: 0x{hr:X}");

        hr = audioClient.GetBufferSize(out uint bufferSize);
        if (hr != HRESULT.S_OK)
            throw new Exception($"Could not get player's buffer: 0x{hr:X}");

        while (dataOffset < CurrentAudioStream.Length && !cancellationToken.IsCancellationRequested)
        {
            //gets the amount of frames already in the buffer...
            hr = audioClient.GetCurrentPadding(out uint padding);

            if (hr != HRESULT.S_OK)
                throw new Exception($"Abort. Failed to get the padding: 0x{hr:X}");

            //take the actual buffer size and remove the frames already inside of the buffer
            uint actualFramesAvaiable = bufferSize - padding;
            if (actualFramesAvaiable == 0)
                continue; //just keep going until we are given some

            uint bytesPerFrame = format.nBlockAlign;
            uint copyAmnt = Math.Min(actualFramesAvaiable * bytesPerFrame, (uint)(CurrentAudioStream.Length - dataOffset));
            uint countWrite = copyAmnt / bytesPerFrame;

            hr = audioRenderClient.GetBuffer(countWrite, out IntPtr bufferPtr);
            if (hr != HRESULT.S_OK)
                throw new Exception($"Could not get buffer to write: 0x{hr:X} , point to {bufferPtr}");
            else if (bufferPtr == IntPtr.Zero)
                break; //do not throw an error for this. It happens sometimes

            //finally to the easier steps, copy to the buffer and release.

            //copy raw pcm wav data (+ offset) to the buffer pointer with the amnt 

            byte[] bufferData = new byte[copyAmnt];

            CurrentAudioStream.Position = dataOffset;

            _ = await CurrentAudioStream.ReadAsync(bufferData, 0, (int)copyAmnt, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                break;

            Marshal.Copy(bufferData, 0, bufferPtr, bufferData.Length);

            audioRenderClient.ReleaseBuffer(countWrite, 0);

            dataOffset += (int)copyAmnt;
        }

        audioClient.Stop();
        return cancellationToken.IsCancellationRequested ? PlayerResult.Cancelled : PlayerResult.Finished;
    }

    public bool Restart()
    {
        dataOffset = 0;
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