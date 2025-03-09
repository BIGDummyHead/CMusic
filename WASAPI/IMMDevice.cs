using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

[ComImport]
[Guid(GUIDs.IID_IMMDevice)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IMMDevice
{

    /*
    Return code	Description
        E_NOINTERFACE
            The object does not support the requested interface type.
        E_POINTER
            Parameter ppInterface is NULL.
        E_INVALIDARG
            The pActivationParams parameter must be NULL for the specified interface; or pActivationParams points to invalid data.
        E_OUTOFMEMORY
            Out of memory.
        AUDCLNT_E_DEVICE_INVALIDATED
            The user has removed either the audio endpoint device or the adapter device that the endpoint device connects to.
    */

    /// <summary>
    /// The Activate method creates a COM object with the specified interface.
    /// </summary>
    /// <param name="iid">The interface identifier. This parameter is a reference to a GUID that identifies the interface that the caller requests be activated. The caller will use this interface to communicate with the COM object. Set this parameter to one of the following interface identifiers:</param>
    /// <param name="dwClsCtx">The execution context in which the code that manages the newly created object will run. The caller can restrict the context by setting this parameter to the bitwise OR of one or more CLSCTX enumeration values. Alternatively, the client can avoid imposing any context restrictions by specifying CLSCTX_ALL. For more information about CLSCTX, see the Windows SDK documentation.</param>
    /// <param name="pActivationParams">Set to NULL to activate an IAudioEndpointVolume, IAudioMeterInformation, IAudioSessionManager, or IDeviceTopology interface on an audio endpoint device. Starting in Windows 10 Build 20348, callers activating an IAudioClient can set pActivationParams to a pointer to a AUDIOCLIENT_ACTIVATION_PARAMS to configure an audio client in loopback mode with a process filter. When activating an IBaseFilter, IDirectSound, IDirectSound8, IDirectSoundCapture, or IDirectSoundCapture8 interface on the device, the caller can specify a pointer to a PROPVARIANT structure that contains stream-initialization information. For more information, see Remarks.</param>
    /// <param name="ppInterface">Pointer to a pointer variable into which the method writes the address of the interface specified by parameter iid. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the Activate call fails, *ppInterface is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.</returns>
    [PreserveSig]
    HRESULT Activate(
        [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
        uint dwClsCtx,
        IntPtr pActivationParams,
        out IntPtr ppInterface
    );

    [PreserveSig]
    HRESULT GetId([Out] out IntPtr ppstrId);

    [PreserveSig]
    HRESULT GetState([Out] out IntPtr pdwState);

    HRESULT OpenPropertyStore(
        [In] ulong stgmAccess,
        [Out] IntPtr ppProperties
    );

}