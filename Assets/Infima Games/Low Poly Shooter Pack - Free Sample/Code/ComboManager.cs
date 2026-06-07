using UnityEngine;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance { get; private set; }

    [Header("Combo Settings")]
    [Tooltip("Zombiyi vurduktan sonra komboyu korumak için izin verilen maksimum süre (saniye).")]
    public float comboExpiryTime = 2.0f;
    
    [Header("UI References")]
    [Tooltip("Kombo sayısını gösterecek TextMeshPro UGUI bileşeni.")]
    public TextMeshProUGUI comboText;

    private int currentCombo = 0;
    private float lastHitTime;
    private Coroutine scalePopCoroutine;
    private Vector3 originalScale = Vector3.one;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (comboText != null)
        {
            originalScale = comboText.transform.localScale;
            comboText.gameObject.SetActive(false); // Başlangıçta gizli tut
        }
    }

    private void Update()
    {
        // Kombo aktifse ve süre dolduysa sıfırla
        if (currentCombo > 0 && Time.time - lastHitTime > comboExpiryTime)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// Her başarılı mermi çarptığında çağrılır.
    /// </summary>
    public void RegisterHit()
    {
        currentCombo++;
        lastHitTime = Time.time;

        if (comboText != null)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = "x" + currentCombo;

            // Juice / Pop Animasyonunu Tetikle
            if (scalePopCoroutine != null)
            {
                StopCoroutine(scalePopCoroutine);
            }
            scalePopCoroutine = StartCoroutine(ScalePopRoutine());
        }
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ScalePopRoutine()
    {
        float duration = 0.12f;
        float elapsed = 0f;
        Vector3 targetScale = originalScale * 1.5f;

        // Büyüme Aşaması
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        // Küçülme Aşaması
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        comboText.transform.localScale = originalScale;
    }
}
