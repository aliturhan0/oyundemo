using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Damage Settings")]
    public float invincibilityTime = 1f;
    private float lastDamageTime;
    private bool isDead = false;

    [Header("UI & Effects")]
    public Slider healthBarSlider;
    public Image bloodScreenImage;
    public float bloodFadeSpeed = 2f;
    private float targetBloodAlpha = 0f;

    [Header("Death Menu")]
    public GameObject deathMenuPanel;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip[] hitSounds;
    public AudioSource heartbeatAudioSource;

    [Header("Camera Death Effect")]
    public Transform playerCamera;
    public float fallSpeed = 2f;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarSlider != null)
        {
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        if (bloodScreenImage != null)
        {
            Color c = bloodScreenImage.color;
            c.a = 0;
            bloodScreenImage.color = c;
            
            // Kan ekranının tıklamaları engellememesi için Raycast'i kapatıyoruz
            bloodScreenImage.raycastTarget = false;
        }

        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
        }

        // Oyun başladığında zaman normal aksın
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Kan ekranının yavaşça kaybolmasını sağla
        if (bloodScreenImage != null && !isDead)
        {
            Color c = bloodScreenImage.color;
            c.a = Mathf.Lerp(c.a, targetBloodAlpha, Time.deltaTime * bloodFadeSpeed);
            bloodScreenImage.color = c;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || Time.time < lastDamageTime + invincibilityTime) return;

        lastDamageTime = Time.time;
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0; // Canı 0'da tut, eksiye düşmesin
        
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }

        // Kan ekranını parlat
        targetBloodAlpha = 0.8f;
        StartCoroutine(ResetBloodAlpha());

        // Hasar sesi çal
        if (playerAudioSource != null && hitSounds.Length > 0)
        {
            playerAudioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
        }

        // Can %30'un altına düştüyse kalp atışı başlasın
        if (currentHealth <= (maxHealth * 0.3f) && currentHealth > 0)
        {
            if (heartbeatAudioSource != null && !heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Play();
            }
        }

        if (currentHealth <= 0)
        {
            if (heartbeatAudioSource != null) heartbeatAudioSource.Stop();
            Die();
        }
    }

    private IEnumerator ResetBloodAlpha()
    {
        yield return new WaitForSeconds(0.1f);
        targetBloodAlpha = 0f;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Karakterin diğer tüm scriptlerini (Hareket, Kamera, Silah vs.) kapat ki fareyi kilitlemesinler
        var components = GetComponents<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp != this)
            {
                comp.enabled = false;
            }
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Kan ekranını tamamen kızart
        if (bloodScreenImage != null)
        {
            Color c = bloodScreenImage.color;
            c.a = 1f;
            bloodScreenImage.color = c;
        }

        // Kamerayı yavaşça arkaya doğru (yukarı bakacak şekilde) düşür
        if (playerCamera != null)
        {
            Quaternion startRot = playerCamera.localRotation;
            Quaternion targetRot = Quaternion.Euler(-90f, startRot.eulerAngles.y, startRot.eulerAngles.z);
            float t = 0;

            while (t < 1f)
            {
                t += Time.deltaTime * fallSpeed;
                playerCamera.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                
                // Ayrıca kamerayı biraz yere doğru indir
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, new Vector3(playerCamera.localPosition.x, -0.5f, playerCamera.localPosition.z), t * 0.5f);
                
                yield return null;
            }
        }

        // Ölüm menüsünü aç
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(true);
        }

        // Oyunu durdur (Input sisteminin bozulmaması için tam 0 değil, sıfıra çok yakın yapıyoruz)
        Time.timeScale = 0.0001f;
        AudioListener.pause = true; // Tüm sesleri durdur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Butonlar için Public Metodlar
    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
