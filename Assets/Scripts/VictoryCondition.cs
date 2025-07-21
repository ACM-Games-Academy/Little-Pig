using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryCondition : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private GameObject videoCanvas;          // UI Canvas with a RawImage
    [SerializeField] private RawImage videoDisplay;           // UI RawImage to display the video
    [SerializeField] private VideoPlayer videoPlayer;         // VideoPlayer component (disabled by default)
    [SerializeField] private VideoClip victoryVideo;          // Assigned .mp4 or .mov file

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private bool hasWon = false;

    private void Start()
    {
        if (videoCanvas != null)
            videoCanvas.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 0);

            if (videoDisplay != null)
                videoDisplay.texture = videoPlayer.targetTexture;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;

        if (other.CompareTag(playerTag))
        {
            hasWon = true;
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
        Time.timeScale = 0f; // Pause gameplay

        if (videoCanvas != null)
            videoCanvas.SetActive(true);

        if (videoPlayer != null && victoryVideo != null)
        {
            videoPlayer.gameObject.SetActive(true); // Enable the VideoPlayer if it was disabled
            videoPlayer.clip = victoryVideo;
            videoPlayer.Play();
        }

        Debug.Log("Victory: Video started.");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Victory: Video finished.");
        Time.timeScale = 1f; // Unpause just in case
        SceneManager.LoadScene("LittlePigMainMenu");
    }
}
