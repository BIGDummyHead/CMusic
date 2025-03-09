using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

public static class Tools
{
    public static HRESULT GetDefaultDevice(out IMMDevice? device)
    {
        device = null;

        Guid CLSIID_IMMDeviceEnum = new Guid(GUIDs.CLSID_MMDeviceEnumerator);
        Guid IID_IMMDeviceEnum = new Guid(GUIDs.IID_IMMDeviceEnumerator);
        HRESULT hr = Ole32.CoCreateInstance(CLSIID_IMMDeviceEnum, IntPtr.Zero, CLSCTX.CLSCTX_ALL, IID_IMMDeviceEnum, out IntPtr ptrIMMDeviceEnum);

        if (hr != HRESULT.S_OK || ptrIMMDeviceEnum == IntPtr.Zero)
            return hr;

        IMMDeviceEnumerator devEnum = (IMMDeviceEnumerator)Marshal.GetObjectForIUnknown(ptrIMMDeviceEnum) ?? throw new Exception("No Device Enumerator gathered.");
        Marshal.Release(ptrIMMDeviceEnum);


        hr = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IntPtr ptrDevice);

        if (hr != HRESULT.S_OK || ptrDevice == IntPtr.Zero)
            return hr;

        device = (IMMDevice)Marshal.GetObjectForIUnknown(ptrDevice);
        Marshal.Release(ptrDevice);

        if (device == null)
            return HRESULT.E_POINTER;

        return hr;
    }

    public static HRESULT ActivateDevice(IMMDevice device, out IAudioClient client)
    {

        Guid IID_IAudioClient = new Guid(GUIDs.IID_IAudioClient);

        //we could pass in a audioclient activation parameter as the third, but we can also pass a null ptr
        HRESULT activationResult = device.Activate(IID_IAudioClient, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out IntPtr ptrAudioClient);

        if (activationResult != HRESULT.S_OK)
            client = null;
        else
            client = (IAudioClient)Marshal.GetObjectForIUnknown(ptrAudioClient);

        return activationResult;
    }

    public static HRESULT GetPreferredFormat(IAudioClient client, out WAVEFORMATEX prefFormat)
    {
        HRESULT hr = client.GetMixFormat(out IntPtr ptrFormat);
        prefFormat = (hr == HRESULT.S_OK) ? Marshal.PtrToStructure<WAVEFORMATEX>(ptrFormat) : default;
        return hr;
    }

    public static HRESULT GetRenderClient(IAudioClient client, out IAudioRenderClient? renderClient)
    {

        Guid IID_RenderClient = new Guid(GUIDs.IID_IAudioRenderClient);
        HRESULT hr = client.GetService(IID_RenderClient, out IntPtr ptrRenderClient);

        renderClient = (hr == HRESULT.S_OK) ? ((IAudioRenderClient)Marshal.GetObjectForIUnknown(ptrRenderClient)) : null;

        return hr;
    }

    public static bool IsSupported(IAudioClient client, ref WAVEFORMATEX format)
    {
        return client.IsFormatSupported(AUDCLNT_SHAREMODE.AUDCLNT_SHAREMODE_SHARED, ref format, out _) == HRESULT.S_OK;
    }


    public static HRESULT EnsureFormat(IAudioClient client, ref WAVEFORMATEX modifiedFormat)
    {
        if (client == null)
            return HRESULT.E_NOINTERFACE;
        else if (IsSupported(client, ref modifiedFormat)) //default cases here
            return HRESULT.S_OK;

        HRESULT hr = GetPreferredFormat(client, out WAVEFORMATEX preferredFormat);

        if (hr != HRESULT.S_OK)
            return hr;

        preferredFormat.wFormatTag = modifiedFormat.wFormatTag;
        preferredFormat.cbSize = modifiedFormat.cbSize;

        // Ensure other fields are compatible if needed
        preferredFormat.nChannels = modifiedFormat.nChannels;
        preferredFormat.wBitsPerSample = modifiedFormat.wBitsPerSample;
        preferredFormat.nBlockAlign = (ushort)(preferredFormat.nChannels * (preferredFormat.wBitsPerSample / 8));
        preferredFormat.nAvgBytesPerSec = preferredFormat.nSamplesPerSec * preferredFormat.nBlockAlign;

        modifiedFormat = preferredFormat;

        return IsSupported(client, ref modifiedFormat) ? HRESULT.S_OK : HRESULT.E_INVALIDARG;
    }
}