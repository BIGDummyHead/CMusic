using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

[ComImport]
[Guid(GUIDs.IID_IAudioClient)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IAudioClient
{
    // The methods must be in this exact order
    
    [PreserveSig]
    HRESULT Initialize(
        AUDCLNT_SHAREMODE ShareMode,
        uint StreamFlags,
        long hnsBufferDuration,
        long hnsPeriodicity,
        [In] ref WAVEFORMATEX pFormat,
        [In] IntPtr audioSessionGuid);
        
    [PreserveSig]
    HRESULT GetBufferSize(
        [Out] out uint pNumBufferFrames);
        
    [PreserveSig]
    HRESULT GetStreamLatency(
        [Out] out long phnsLatency);
        
    [PreserveSig]
    HRESULT GetCurrentPadding(
        [Out] out uint pNumPaddingFrames);
        
    [PreserveSig]
    HRESULT IsFormatSupported(
        [In] AUDCLNT_SHAREMODE ShareMode,
        [In] ref WAVEFORMATEX pFormat,
        [Out] out IntPtr ppClosestMatch);
        
    [PreserveSig]
    HRESULT GetMixFormat(
        [Out] out IntPtr ppDeviceFormat);
        
    [PreserveSig]
    HRESULT GetDevicePeriod(
        [Out] out long phnsDefaultDevicePeriod,
        [Out] out long phnsMinimumDevicePeriod);
        
    [PreserveSig]
    HRESULT Start();
    
    [PreserveSig]
    HRESULT Stop();
    
    [PreserveSig]
    HRESULT Reset();
    
    [PreserveSig]
    HRESULT SetEventHandle(
        [In] IntPtr eventHandle);
        
    [PreserveSig]
    HRESULT GetService(
        [In] Guid riid,
        [Out] out IntPtr ppv);
}