using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    public GameObject ControlsUI;
    public Image fadeUI;
    public float fadeDuration = 1f;

    [SerializeField]
    public Button CtrlButton;
    public Button XButton;
    public Button PlayButton;
    public Button QuitButton;
    public GameObject MenuButtons;

    private void Start()
    {
        if (fadeUI != null)
        {
            Color c = fadeUI.color;
            c.a = 1f; // Fully transparent
            fadeUI.color = c;
        }

        CtrlButton.onClick.AddListener(TaskOnClick);
        XButton.onClick.AddListener(ControlsExit);
        PlayButton.onClick.AddListener(Play);
        QuitButton.onClick.AddListener(Quit);
    }

    public void TaskOnClick()
    {
        Debug.Log("Button clicked!");
        ControlsUI.SetActive(true);
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }

    public void Fade()
    {
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        Debug.Log("Loading scene...");

        MenuButtons.SetActive(false);

        Color fadeColor = fadeUI.color;
        fadeColor.a = 0f;
        fadeUI.color = fadeColor;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / fadeDuration);
            fadeColor.a = 1f - normalizedTime;
            fadeUI.color = fadeColor;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("LittlePig"); // Change if needed
    }

    public void Controls()
    {
        ControlsUI.SetActive(true);
        Debug.Log("Controls UI opened");
    }

    public void ControlsExit()
    {
        ControlsUI.SetActive(false);
        Debug.Log("Controls UI closed");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }
}
