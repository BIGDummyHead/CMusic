public enum AUDCLNT_BUFFERFLAGS : uint
{
    /// <summary>
    /// The data in the packet does not correlate with the device position of the previous packet; this could be due to a stream state transition or a timing glitch.
    /// </summary>
    AUDCLNT_BUFFERFLAGS_DATA_DISCONTINUITY,
    /// <summary>
    /// Treats all data in the packet as silence and ignores the actual data values. See Rendering a Stream and Capturing a Stream for more information on using this flag .
    /// </summary>
    AUDCLNT_BUFFERFLAGS_SILENT,
    /// <summary>
    /// The device stream position is recorded at an indeterminate time. Therefore, the client may not be able to accurately set the timestamp of the current packet.
    /// </summary>
    AUDCLNT_BUFFERFLAGS_TIMESTAMP_ERROR
}