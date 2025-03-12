using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFileName;
    void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log("Playing video: " + videoPlayer.url);
            videoPlayer.Play();
        }
    }
}
