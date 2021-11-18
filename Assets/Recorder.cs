using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;
using DateTime = System.DateTime;

sealed class Recorder : MonoBehaviour
{
    [SerializeField] RenderTexture _source = null;

    public bool IsPlaying { get; private set; }

    string _lastPath;

    string Filename => $"Record_{DateTime.Now:MMdd_HHmm_ss}.mp4";

    Queue<double> _timeQueue = new Queue<double>();
    double _startTime;

    public void StartRecording()
    {
#if !UNITY_EDITOR && UNITY_IOS
        if (_lastPath != null) VideoWriter.StoreToAlbum(_lastPath);
#endif

        var path = Filename;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
            path = Application.temporaryCachePath + "/" + path;

        _lastPath = path;
        VideoWriter.Start(path, _source.width, _source.height);
        IsPlaying = true;

        _timeQueue.Clear();
        _startTime = 0;
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

        if (_startTime == 0)
        {
            _timeQueue.Enqueue(0);
            _startTime = Time.timeAsDouble;
        }
        else
        {
            _timeQueue.Enqueue(Time.timeAsDouble - _startTime);
        }

        AsyncGPUReadback.Request(_source, 0, OnSourceReadback);
    }

    unsafe void OnSourceReadback(AsyncGPUReadbackRequest request)
    {
        if (!IsPlaying) return;
        var data = request.GetData<byte>(0);
        var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
        VideoWriter.Update(ptr, (uint)data.Length, _timeQueue.Dequeue());
    }
}
