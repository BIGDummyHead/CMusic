using System.Runtime.InteropServices;

namespace CMusic.WASAPI;

public class Ole32
{

    /// <summary>
    /// Creates and default-initializes a single object of the class associated with a specified CLSID. Call CoCreateInstance when you want to create only one object on the local system. To create a single object on a remote system, call the CoCreateInstanceEx function. To create multiple objects based on a single CLSID, call the CoGetClassObject function.
    /// </summary>
    /// <param name="clsid">The CLSID associated with the data and code that will be used to create the object.</param>
    /// <param name="inner">If NULL, indicates that the object is not being created as part of an aggregate. If non-NULL, pointer to the aggregate object's IUnknown interface (the controlling IUnknown).</param>
    /// <param name="context">Context in which the code that manages the newly created object will run. The values are taken from the enumeration CLSCTX.</param>
    /// <param name="iid">A reference to the identifier of the interface to be used to communicate with the object.</param>
    /// <param name="instance">Address of pointer variable that receives the interface pointer requested in riid. Upon successful return, *ppv contains the requested interface pointer. Upon failure, *ppv contains NULL.</param>
    /// <returns></returns>
    [DllImport("ole32.dll")]
    public static extern HRESULT CoCreateInstance( [In] Guid rclsid, [In] IntPtr pUnkOuter, [In] uint dwClsContext, [In] Guid riid, [Out] out IntPtr ppv);
}