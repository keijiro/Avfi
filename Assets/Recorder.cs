using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;
using DateTime = System.DateTime;

sealed class Recorder : MonoBehaviour
{
    [SerializeField] RenderTexture _source = null;

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

    void OnDestroy()
    {
        if (IsPlaying) EndRecording();
    }

    void Start()
      => Application.targetFrameRate = 60;

    void Update()
    {
        if (!IsPlaying) return;
        AsyncGPUReadback.Request(_source, 0, OnSourceReadback);
    }

    unsafe void OnSourceReadback(AsyncGPUReadbackRequest request)
    {
        if (!IsPlaying) return;
        var data = request.GetData<byte>(0);
        var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
        VideoWriter.Update(ptr, (uint)data.Length);
    }
}
