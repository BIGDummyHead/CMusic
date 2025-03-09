namespace CMusic.WASAPI;

public abstract class AUDCLNT_SESSIONFLAGS
{
    public const int Default = 0;

    /// <summary>
    /// The session expires when there are no associated streams and owning session control objects holding references.
    /// </summary>
    public const uint AUDCLNT_SESSIONFLAGS_EXPIREWHENUNOWNED = 0x10000000;
    /// <summary>
    /// The volume control is hidden in the volume mixer user interface when the audio session is created. If the session associated with the stream already exists before IAudioClient::Initialize opens the stream, the volume control is displayed in the volume mixer.
    /// </summary>
    public const uint AUDCLNT_SESSIONFLAGS_DISPLAY_HIDE = 0x20000000;
    /// <summary>
    /// The volume control is hidden in the volume mixer user interface after the session expires.
    /// </summary>
    public const uint AUDCLNT_SESSIONFLAGS_DISPLAY_HIDEWHENEXPIRED = 0x40000000;
}