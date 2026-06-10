using UnityEngine;
using UnityEngine.SceneManagement;

public class AnaMenuYonetici : MonoBehaviour
{
    [Header("UI Panelleri")]
    [Tooltip("Kontroller ve Bilgiler panelini temsil eden GameObject.")]
    public GameObject kontrollerPaneli;

    [Tooltip("Ana Menü butonlarının bulunduğu panel (Açılışta aktif olan).")]
    public GameObject anaMenuPaneli;

    [Header("Müzik Ayarları")]
    [Tooltip("Ana menüde çalacak olan müzik kaynağı.")]
    public AudioSource menuAudioSource;
    [Tooltip("Ana menü müzik dosyası.")]
    public AudioClip menuMusic;

    private void Start()
    {
        // Oyun açıldığında sadece ana menü butonları görünsün, kontroller gizli olsun
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(true);
        if (kontrollerPaneli != null) kontrollerPaneli.SetActive(false);

        // Müzik çalma ayarları
        if (menuAudioSource == null)
        {
            menuAudioSource = GetComponent<AudioSource>();
            if (menuAudioSource == null)
            {
                menuAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (menuAudioSource != null)
        {
            menuAudioSource.spatialBlend = 0f; // 2D ses yap
            if (menuMusic != null)
            {
                menuAudioSource.clip = menuMusic;
                menuAudioSource.loop = true;
                menuAudioSource.volume = 0.5f; // Rahatsız etmeyecek orta seviye ses
                menuAudioSource.Play();
            }
        }

        // Menüde fareyi serbest bırak ve görünür yap
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Oyunu 1. Levelden (SampleScene) başlatır.
    /// </summary>
    public void OyunaBasla()
    {
        Debug.Log("Oyun başlatılıyor... Yüklenen sahne: SampleScene");
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// Kontroller panelini açar ve ana menüyü gizler.
    /// </summary>
    public void KontrolleriAc()
    {
        if (kontrollerPaneli != null) kontrollerPaneli.SetActive(true);
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(false);
    }

    /// <summary>
    /// Kontroller panelini kapatır ve ana menüye döner.
    /// </summary>
    public void KontrolleriKapat()
    {
        if (kontrollerPaneli != null) kontrollerPaneli.SetActive(false);
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(true);
    }

    /// <summary>
    /// Oyundan tamamen çıkış yapar.
    /// </summary>
    public void OyundanCik()
    {
        Debug.Log("Oyundan çıkış yapılıyor...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
