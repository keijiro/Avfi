using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;
using DateTime = System.DateTime;

sealed class Recorder : MonoBehaviour
{
    [SerializeField] RenderTexture _source = null;

    NativeArray<Color32> _buffer;

    public bool IsPlaying { get; private set; }

    string Filename => $"Record_{DateTime.Now:MMdd_HHmm_ss}.mp4";

    public void StartRecording()
    {
        VideoWriter.Start(Filename, _source.width, _source.height);
        IsPlaying = true;
    }

    public void EndRecording()
    {
        VideoWriter.End();
        IsPlaying = false;
    }

    void Start()
      => _buffer = new NativeArray<Color32>
           (_source.width * _source.height,
            Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

    void OnDestroy()
    {
        if (IsPlaying) EndRecording();
        _buffer.Dispose();
    }

    unsafe void Update()
    {
        if (!IsPlaying) return;

        var (w, h) = (_source.width, _source.height);

        var p = (Color32)Color.HSVToRGB(Time.time % 1, 1, 1);
        for (var i = 0; i < w * h; i++) _buffer[i]  = p;

        var ptr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(_buffer);
        VideoWriter.Update((IntPtr)ptr, (uint)(w * h * 4));
    }
}
