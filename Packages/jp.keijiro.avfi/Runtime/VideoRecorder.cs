using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;

namespace Avfi {

public sealed class VideoRecorder : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] RenderTexture _source = null;

    public RenderTexture source
      { get => _source; set => ChangeSource(value); }

    #endregion

    #region Public properties and methods

    public bool IsPlaying
      { get; private set; }

    public void StartRecording()
    {
        var path = PathUtil.GetTemporaryFilePath();
        Plugin.StartRecording(path, _source.width, _source.height);

        _timeQueue.Clear();
        IsPlaying = true;
    }

    public void EndRecording()
    {
        AsyncGPUReadback.WaitAllRequests();
        Plugin.EndRecording();
        IsPlaying = false;
    }

    #endregion

    #region Private objects

    RenderTexture _buffer;
    TimeQueue _timeQueue = new TimeQueue();

    void ChangeSource(RenderTexture rt)
    {
        if (IsPlaying)
        {
            Debug.LogError("Can't change the source while recording.");
            return;
        }

        if (_buffer != null) Destroy(_buffer);

        _source = rt;
        _buffer = new RenderTexture(rt.width, rt.height, 0);
    }

    #endregion

    #region Async GPU readback

    unsafe void OnSourceReadback(AsyncGPUReadbackRequest request)
    {
        if (!IsPlaying) return;
        var data = request.GetData<byte>(0);
        var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
        Plugin.AppendFrame(ptr, (uint)data.Length, _timeQueue.Dequeue());
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => ChangeSource(_source);

    void OnDestroy()
    {
        if (IsPlaying) EndRecording();
        Destroy(_buffer);
    }

    void Update()
    {
        if (!IsPlaying) return;
        if (!_timeQueue.TryEnqueueNow()) return;
        Graphics.Blit(_source, _buffer, new Vector2(1, -1), new Vector2(0, 1));
        AsyncGPUReadback.Request(_buffer, 0, OnSourceReadback);
    }

    #endregion
}

} // namespace Avfi
