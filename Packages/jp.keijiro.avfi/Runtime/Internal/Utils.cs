using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;

namespace Avfi {

static class PathUtil
{
    public static string TemporaryDirectoryPath
      => Application.platform == RuntimePlatform.IPhonePlayer
           ? Application.temporaryCachePath : ".";

    public static string GetTimestampedFilename()
      => $"Record_{DateTime.Now:MMdd_HHmm_ss}.mp4";

    public static string GetTemporaryFilePath()
      => TemporaryDirectoryPath + "/" + GetTimestampedFilename();
}

sealed class TimeQueue
{
    Queue<double> _queue = new Queue<double>();
    double _start;
    double _last;

    public void Clear()
    {
        _queue.Clear();
        _start = 0;
    }

    public double Dequeue()
      => _queue.Dequeue();

    public bool TryEnqueueNow()
    {
        if (_start == 0)
        {
            _queue.Enqueue(0);
            _start = Time.timeAsDouble;
            _last = 0;
            return true;
        }
        else
        {
            var time = Time.timeAsDouble - _start;

            // Reject it if it falls into the same frame.
            if ((int)(time * 60) == (int)(_last * 60)) return false;

            _queue.Enqueue(time);
            _last = time;
            return true;
        }
    }
}

} // namespace Avfi
