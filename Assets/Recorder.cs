using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;
using DateTime = System.DateTime;

sealed class Recorder : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] RenderTexture _source = null;

    #endregion

    #region Private asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public members for UI

    public bool IsPlaying { get; private set; }

    public void StartRecording()
    {
        VideoWriter.Start
          (GetTimestampedFilePath(), _source.width, _source.height);

        _timeQueue.Clear();
        _startTime = 0;

        IsPlaying = true;
    }

    public void EndRecording()
    {
        AsyncGPUReadback.WaitAllRequests();
        VideoWriter.End();
        IsPlaying = false;
    }

    #endregion

    #region Timestamped filename generation

    string DirectoryPath
      => Application.platform == RuntimePlatform.IPhonePlayer
           ? Application.temporaryCachePath : ".";

    string GetTimestampedFilename()
      => $"Record_{DateTime.Now:MMdd_HHmm_ss}.mp4";

    string GetTimestampedFilePath()
      => DirectoryPath + "/" + GetTimestampedFilename();

    #endregion

    #region Private objects

    RenderTexture _buffer;
    Material _material;

    Queue<double> _timeQueue = new Queue<double>();
    double _startTime;

    #endregion

    #region Async GPU readback

    unsafe void OnSourceReadback(AsyncGPUReadbackRequest request)
    {
        if (!IsPlaying) return;
        var data = request.GetData<byte>(0);
        var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
        VideoWriter.Update(ptr, (uint)data.Length, _timeQueue.Dequeue());
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _buffer = new RenderTexture(_source.width, _source.height, 0);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        if (IsPlaying) EndRecording();
        Destroy(_buffer);
        Destroy(_material);
    }

    void Update()
    {
        if (!IsPlaying) return;

        Graphics.Blit(_source, _buffer, _material);

        if (_startTime == 0)
        {
            _timeQueue.Enqueue(0);
            _startTime = Time.timeAsDouble;
        }
        else
        {
            _timeQueue.Enqueue(Time.timeAsDouble - _startTime);
        }

        AsyncGPUReadback.Request(_buffer, 0, OnSourceReadback);
    }

    #endregion
}
