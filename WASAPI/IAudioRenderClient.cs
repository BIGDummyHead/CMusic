using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

[ComImport]
[Guid(GUIDs.IID_IAudioRenderClient)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IAudioRenderClient
{
    /// <summary>
    /// Retrieves a pointer to the next available space in the rendering endpoint buffer into which the caller can write a data packet.
    /// </summary>
    /// <param name="NumFramesRequested">The number of audio frames in the data packet that the caller plans to write to the requested space in the buffer. If the call succeeds, the size of the buffer area pointed to by *ppData matches the size specified in NumFramesRequested.</param>
    /// <param name="ppData">Pointer to a pointer variable into which the method writes the starting address of the buffer area into which the caller will write the data packet.</param>
    /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.</returns>
    HRESULT GetBuffer([In] uint NumFramesRequested, [Out] out IntPtr ppData);


    /// <summary>
    /// The ReleaseBuffer method releases the buffer space acquired in the previous call to the IAudioRenderClient::GetBuffer method.
    /// </summary>
    /// <param name="NumFramesWritten">The number of audio frames written by the client to the data packet. The value of this parameter must be less than or equal to the size of the data packet, as specified in the NumFramesRequested parameter passed to the IAudioRenderClient::GetBuffer method.</param>
    /// <param name="dwFlags">The buffer-configuration flags. The caller can set this parameter either to 0 or to the following _AUDCLNT_BUFFERFLAGS enumeration value (a flag bit):</param>
    /// <returns></returns>
    HRESULT ReleaseBuffer([In] uint NumFramesWritten, uint dwFlags);
}