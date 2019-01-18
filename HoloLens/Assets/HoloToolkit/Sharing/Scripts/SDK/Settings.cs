//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.10
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace HoloToolkit.Sharing {

public class Settings : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal Settings(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(Settings obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~Settings() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SharingClientPINVOKE.delete_Settings(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public Settings() : this(SharingClientPINVOKE.new_Settings(), true) {
  }

  public XString GetServerAddress() {
    global::System.IntPtr cPtr = SharingClientPINVOKE.Settings_GetServerAddress(swigCPtr);
    XString ret = (cPtr == global::System.IntPtr.Zero) ? null : new XString(cPtr, true);
    return ret; 
  }

  public int GetServerPort() {
    int ret = SharingClientPINVOKE.Settings_GetServerPort(swigCPtr);
    return ret;
  }

  public XString GetViewerAddress() {
    global::System.IntPtr cPtr = SharingClientPINVOKE.Settings_GetViewerAddress(swigCPtr);
    XString ret = (cPtr == global::System.IntPtr.Zero) ? null : new XString(cPtr, true);
    return ret; 
  }

  public int GetViewerPort() {
    int ret = SharingClientPINVOKE.Settings_GetViewerPort(swigCPtr);
    return ret;
  }

  public XString GetLocalUserName() {
    global::System.IntPtr cPtr = SharingClientPINVOKE.Settings_GetLocalUserName(swigCPtr);
    XString ret = (cPtr == global::System.IntPtr.Zero) ? null : new XString(cPtr, true);
    return ret; 
  }

}

}
