using DllImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
using IntPtr = System.IntPtr;

namespace Avfi {

static class Plugin
{
#if !UNITY_EDITOR && UNITY_IOS
    const string DllName = "__Internal";
#else
    const string DllName = "Avfi";
#endif

    [DllImport(DllName, EntryPoint = "Avfi_StartRecording")]
    public static extern
      void StartRecording(string filePath, int width, int height);

    [DllImport(DllName, EntryPoint = "Avfi_AppendFrame")]
    public static extern
      void AppendFrame(IntPtr pointer, uint size, double time);

    [DllImport(DllName, EntryPoint = "Avfi_EndRecording")]
    public static extern
      void EndRecording();
}

} // namespace Avfi
