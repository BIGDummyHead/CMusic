namespace CMusic.WASAPI;

public abstract class AUDCLNT_STREAMFLAGS
{

    /// <summary>
    /// The audio stream will be a member of a cross-process audio session. For more information, see Remarks.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_CROSSPROCESS = 0x00010000;

    /// <summary>
    /// The audio stream will operate in loopback mode. For more information, see Remarks.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_LOOPBACK = 0x00020000;

    /// <summary>
    /// Processing of the audio buffer by the client will be event driven. For more information, see Remarks.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_EVENTCALLBACK = 0x00040000;

    /// <summary>
    /// The volume and mute settings for an audio session will not persist across application restarts. For more information, see Remarks.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_NOPERSIST = 0x00080000;

    /// <summary>
    /// This constant is new in Windows 7. The sample rate of the stream is adjusted to a rate specified by an application. For more information, see Remarks.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_RATEADJUST = 0x00100000;
    /// <summary>
    /// A channel matrixer and a sample rate converter are inserted as necessary to convert between the uncompressed format supplied to IAudioClient::Initialize and the audio engine mix format.
    /// </summary>

    public const uint AUDCLNT_STREAMFLAGS_AUTOCONVERTPCM = 0x80000000;

    /// <summary>
    /// When used with AUDCLNT_STREAMFLAGS_AUTOCONVERTPCM, a sample rate converter with better quality than the default conversion but with a higher performance cost is used. This should be used if the audio is ultimately intended to be heard by humans as opposed to other scenarios such as pumping silence or populating a meter.
    /// </summary>
    public const uint AUDCLNT_STREAMFLAGS_SRC_DEFAULT_QUALITY = 0x08000000;
}