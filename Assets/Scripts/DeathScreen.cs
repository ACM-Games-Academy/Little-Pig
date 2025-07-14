using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DeathScreen: Death Panel is not assigned in the Inspector.");
        }

        // Attach button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
        else
        {
            Debug.LogError("DeathScreen: Restart button is not assigned.");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogError("DeathScreen: Quit button is not assigned.");
        }
    }

    /// <summary>
    /// Shows the death screen panel.
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Debug.Log("DeathScreen: deathPanel set to active.");
        }
        else
        {
            Debug.LogError("DeathScreen: deathPanel is null! Cannot show death screen.");
        }
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void RestartLevel()
    {
        Debug.Log("DeathScreen: Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the main menu scene by name.
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("DeathScreen: Loading main menu...");
        SceneManager.LoadScene("LittlePigMainMenu");
    }
}
