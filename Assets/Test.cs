using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;
using IEnumerator = System.Collections.IEnumerator;

sealed class Test : MonoBehaviour
{
    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_Start(string filePath, int width, int height);

    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_Update(IntPtr pointer, uint size);

    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_End();

    NativeArray<Color32> _buffer;

    void Start()
    {
        _buffer = new NativeArray<Color32>
          (1920 * 1080, Allocator.Persistent,
           NativeArrayOptions.UninitializedMemory);

        Debug.Log("Start");
        VideoWriter_Start("Test.mp4", 1920, 1080);
    }

    unsafe void Update()
    {
        if (Time.frameCount % 4 != 0) return;

        var p = (Color32)Color.HSVToRGB(Time.time % 1, 1, 1);
        for (var i = 0; i < 1920*1080; i++)
            _buffer[i]  = p;

        Debug.Log("Update");
        var ptr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_buffer);
        VideoWriter_Update((IntPtr)ptr, 1920 * 1080 * 4);
    }

    void OnDestroy()
    {
        Debug.Log("End");
        VideoWriter_End();
        _buffer.Dispose();
    }
}
