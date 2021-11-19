using UnityEngine;
using UnityEngine.UI;

sealed class Controller : MonoBehaviour
{
    [SerializeField] Text _buttonLabel = null;

    void Start()
      => Application.targetFrameRate = 60;

    public void OnPressRecordButton()
    {
        var rec = GetComponent<Avfi.VideoRecorder>();

        if (rec.IsPlaying)
            rec.EndRecording();
        else
            rec.StartRecording();

        _buttonLabel.text = rec.IsPlaying ? "Stop" : "Record";
        _buttonLabel.color = rec.IsPlaying ? Color.red : Color.black;
    }
}
