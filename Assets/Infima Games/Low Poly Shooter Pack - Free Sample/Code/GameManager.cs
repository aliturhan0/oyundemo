using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using InfimaGames.LowPolyShooterPack;

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

    public bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // EventSystem'i bul ve DeathPanel'in altından kurtarıp Canvas'ın altına taşı
        var eventSys = FindObjectOfType<UnityEngine.EventSystems.EventSystem>(true);
        var canvas = FindObjectOfType<Canvas>();
        if (eventSys != null && canvas != null)
        {
            eventSys.transform.SetParent(canvas.transform, true);
            eventSys.gameObject.SetActive(true);
            Debug.Log("[GameManager] EventSystem başarıyla Canvas altına taşındı ve butonlar aktifleşti.");
        }
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
            objectiveText.gameObject.SetActive(false);
        }

        // Sahne adına göre müzik davranışını ayarla
        string sahneAdi = SceneManager.GetActiveScene().name;

        if (sahneAdi != "SampleScene")
        {
            // LEVEL 2 VE DİĞERLERİ: Sahnedeki tüm diğer sesleri kapat, kazanma sesini loopta çal
            if (gameAudioSource == null)
            {
                gameAudioSource = GetComponent<AudioSource>();
                if (gameAudioSource == null)
                {
                    gameAudioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            gameAudioSource.spatialBlend = 0f; // 2D Ses yap

            AudioSource[] tumSesKaynaklari = FindObjectsOfType<AudioSource>();
            foreach (AudioSource ses in tumSesKaynaklari)
            {
                if (ses != gameAudioSource)
                {
                    ses.Stop();
                }
            }

            if (winSound != null)
            {
                gameAudioSource.clip = winSound;
                gameAudioSource.loop = true; // Döngüye al
                gameAudioSource.volume = 1.0f; // Sesi tam aç
                gameAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("[GameManager] Win Sound (Kazanma Sesi) Inspector'da atanmamış!");
            }
        }
        else
        {
            // LEVEL 1 (SampleScene): Sahne müziklerini susturma, sadece kazanma sesini bir kez çal
            if (winSound != null)
            {
                if (gameAudioSource == null)
                {
                    gameAudioSource = GetComponent<AudioSource>();
                    if (gameAudioSource == null)
                    {
                        gameAudioSource = gameObject.AddComponent<AudioSource>();
                    }
                }
                gameAudioSource.spatialBlend = 0f;
                gameAudioSource.PlayOneShot(winSound);
            }
        }

        // Kombo sayacını temizle/gizle
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.ResetCombo();
        }

        if (winMenuPanel != null)
        {
            winMenuPanel.SetActive(true);
        }

        // Oyuncuyu ve girdilerini tamamen durdur
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            // Character scriptini bul ve fare kilidini kapat
            var character = player.GetComponent<Character>();
            if (character != null)
            {
                character.enabled = false;
            }

            var components = player.GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp != player) comp.enabled = false;
            }
        }

        // Menü tıklanabilsin diye oyunu dondur ve fareyi serbest bırak
        Time.timeScale = 0f;
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

    // Ana menüye dönmek için kullanılacak metod
    public void LoadMainMenu()
    {
        Debug.Log("Ana Menüye dönülüyor...");
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("AnaMenu");
    }

    // Oyundan tamamen çıkmak için kullanılacak metod
    public void QuitGame()
    {
        Debug.Log("QuitGame çağrıldı!");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
