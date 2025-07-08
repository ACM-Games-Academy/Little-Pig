using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using JetBrains.Annotations;

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
    //public LayerMask FadeLayer;

    public void Start()
    {
        if (fadeUI != null)
        {
            Color c = fadeUI.color;
            c.a = 1f; // Fully transparent
            fadeUI.color = c;
        }

        CtrlButton.GetComponent<Button>();
        CtrlButton.onClick.AddListener(TaskOnClick);

        XButton.GetComponent<Button>();
        XButton.onClick.AddListener(ControlsExit);

        PlayButton.GetComponent<Button>();
        PlayButton.onClick.AddListener(Play);

        QuitButton.GetComponent<Button>();
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
        Color fadeColor = fadeUI.color;
        fadeColor.a = 0f; // Set alpha to 0
        fadeUI.color = fadeColor; // Apply the modified color

        float t = 0f;

        // Fade out (alpha 1 -> 0)
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float NormalizedTime = Mathf.Clamp01(t / fadeDuration);
            fadeColor.a = 1f - NormalizedTime; // Decrease alpha
            fadeUI.color = fadeColor;
            yield return null; // Wait for the next frame
        }

        // Delay for a moment to show the fade effect
        yield return new WaitForSeconds(1f);

        // Load the next scene (replace "NAME" with the actual scene name)
        SceneManager.LoadScene("LittlePig");
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
