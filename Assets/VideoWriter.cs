using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

static class VideoWriter
{
    [DllImport("VideoWriter", EntryPoint = "VideoWriter_Start")]
    public static extern void Start(string filePath, int width, int height);

    [DllImport("VideoWriter", EntryPoint = "VideoWriter_Update")]
    public static extern void Update(IntPtr pointer, uint size, double time);

    [DllImport("VideoWriter", EntryPoint = "VideoWriter_End")]
    public static extern void End();
}
