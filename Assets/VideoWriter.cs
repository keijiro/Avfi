using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

static class VideoWriter
{
#if !UNITY_EDITOR && UNITY_IOS
    const string DllName = "__Internal";
#else
    const string DllName = "VideoWriter";
#endif

    [DllImport(DllName, EntryPoint = "VideoWriter_Start")]
    public static extern void Start(string filePath, int width, int height);

    [DllImport(DllName, EntryPoint = "VideoWriter_Update")]
    public static extern void Update(IntPtr pointer, uint size, double time);

    [DllImport(DllName, EntryPoint = "VideoWriter_End")]
    public static extern void End();

#if !UNITY_EDITOR && UNITY_IOS

    [DllImport(DllName, EntryPoint = "VideoWriter_StoreToAlbum")]
    public static extern void StoreToAlbum(string filePath);

#endif
}
