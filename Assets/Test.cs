using UnityEngine;

sealed class Test : MonoBehaviour
{
    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_Start();

    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_Update();

    [System.Runtime.InteropServices.DllImport("VideoWriter")]
    static extern void VideoWriter_End();

    System.Collections.IEnumerator Start()
    {
        Debug.Log("Start");
        VideoWriter_Start();

        for (var i = 0; i < 30; i++)
        {
            Debug.Log($"Update ({i})");
            yield return new WaitForSeconds(1.0f / 30);
            VideoWriter_Update();
        }

        Debug.Log("End");
        VideoWriter_End();
    }
}
