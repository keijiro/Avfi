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

        if (rec.IsRecording)
            rec.EndRecording();
        else
            rec.StartRecording();

        _buttonLabel.text = rec.IsRecording ? "Stop" : "Record";
        _buttonLabel.color = rec.IsRecording ? Color.red : Color.black;
    }
}
