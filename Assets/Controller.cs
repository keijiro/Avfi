using UnityEngine;
using UnityEngine.UI;

sealed class Controller : MonoBehaviour
{
    [SerializeField] Text _buttonLabel = null;

    public void OnPressRecordButton()
    {
        var rec = GetComponent<Recorder>();

        if (rec.IsPlaying)
            rec.EndRecording();
        else
            rec.StartRecording();

        _buttonLabel.text = rec.IsPlaying ? "Stop" : "Record";
    }
}
