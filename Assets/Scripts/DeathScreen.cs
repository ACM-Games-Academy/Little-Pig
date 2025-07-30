using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text deathPhraseText;

    [Header("Death Phrases")]
    [TextArea]
    [SerializeField] private string[] deathPhrases;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
        else
            Debug.LogError("DeathScreen: Death Panel is not assigned.");

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        else
            Debug.LogError("DeathScreen: Restart button is not assigned.");

        if (quitButton != null)
            quitButton.onClick.AddListener(GoToMainMenu);
        else
            Debug.LogError("DeathScreen: Quit button is not assigned.");
    }

    /// <summary>
    /// Shows the death screen panel and displays a random death phrase.
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);

            if (deathPhraseText != null && deathPhrases != null && deathPhrases.Length > 0)
            {
                string randomPhrase = deathPhrases[Random.Range(0, deathPhrases.Length)];
                deathPhraseText.text = randomPhrase;
            }

            Debug.Log("DeathScreen: deathPanel set to active.");
        }
        else
        {
            Debug.LogError("DeathScreen: deathPanel is null! Cannot show death screen.");
        }
    }

    public void RestartLevel()
    {
        Debug.Log("DeathScreen: Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Debug.Log("DeathScreen: Loading main menu...");
        SceneManager.LoadScene("LittlePigMainMenu");
    }
}
