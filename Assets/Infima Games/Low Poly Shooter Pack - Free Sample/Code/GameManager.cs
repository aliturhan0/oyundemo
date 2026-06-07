using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Wave Settings")]
    public int totalZombiesToKill = 15;
    private int zombiesKilled = 0;

    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public GameObject winMenuPanel;

    [Header("Audio")]
    public AudioSource gameAudioSource;
    public AudioClip winSound;

    [Header("Next Level")]
    public string nextSceneName = "Level2";

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateObjectiveUI();
        if (winMenuPanel != null) winMenuPanel.SetActive(false);
    }

    public void ZombieKilled()
    {
        if (gameEnded) return;

        zombiesKilled++;
        UpdateObjectiveUI();

        if (zombiesKilled >= totalZombiesToKill)
        {
            WinGame();
        }
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Öldürülen Zombi: " + zombiesKilled + " / " + totalZombiesToKill;
        }
    }

    private void WinGame()
    {
        gameEnded = true;

        if (objectiveText != null)
        {
            objectiveText.text = "BÖLÜM GEÇİLDİ! ZOMBİLER TEMİZLENDİ!";
            objectiveText.color = Color.green;
        }

        if (gameAudioSource != null && winSound != null)
        {
            gameAudioSource.PlayOneShot(winSound);
        }

        if (winMenuPanel != null)
        {
            winMenuPanel.SetActive(true);
        }

        // Oyuncuyu durdur
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            var components = player.GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp != player) comp.enabled = false;
            }
        }

        // Menü tıklanabilsin diye oyunu dondur
        Time.timeScale = 0.0001f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // WinPanel içindeki "Sonraki Bölüm" butonuna atanacak metod
    public void LoadNextLevel()
    {
        Debug.Log("LoadNextLevel çağrıldı! Hedef sahne: " + nextSceneName);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
