using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

[ComImport]
[Guid(GUIDs.IID_IMMDeviceEnumerator)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IMMDeviceEnumerator
{
    /// <summary>
    /// The EnumAudioEndpoints method generates a collection of audio endpoint devices that meet the specified criteria.
    /// </summary>
    /// <param name="dataFlow">The data-flow direction for the endpoint devices in the collection. The caller should set this parameter to one of the following EDataFlow enumeration values:</param>
    /// <param name="dwStateMask">The state or states of the endpoints that are to be included in the collection. The caller should set this parameter to the bitwise OR of one or more of the following DEVICE_STATE_XXX constants:</param>
    /// <param name="ppDevices">Pointer to a pointer variable into which the method writes the address of the IMMDeviceCollection interface of the device-collection object. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the EnumAudioEndpoints call fails, *ppDevices is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.</returns>
    HRESULT EnumAudioEndpoints(EDataFlow dataFlow, int dwStateMask, out IntPtr ppDevices);

    /*
        Return code	Description
        E_POINTER
            Parameter ppDevice is NULL.
        E_INVALIDARG
            Parameter dataFlow or role is out of range.
        E_NOTFOUND
            No device is available.
        E_OUTOFMEMORY
            Out of memory.
    */

    /// <summary>
    /// The GetDefaultAudioEndpoint method retrieves the default audio endpoint for the specified data-flow direction and role.
    /// </summary>
    /// <param name="dataFlow">The data-flow direction for the endpoint device. The caller should set this parameter to one of the following two EDataFlow enumeration values</param>
    /// <param name="role">The role of the endpoint device. The caller should set this parameter to one of the following ERole enumeration values:</param>
    /// <param name="ppDevice">Pointer to a pointer variable into which the method writes the address of the IMMDevice interface of the endpoint object for the default audio endpoint device. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the GetDefaultAudioEndpoint call fails, *ppDevice is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.</returns>
    HRESULT GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IntPtr ppDevice);

}