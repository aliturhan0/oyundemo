using UnityEngine;
using TMPro;
using System.Collections;

public class LevelSonuYonetici : MonoBehaviour
{
    [Header("Bileşen Bağlantıları")]
    public CanvasGroup containerGroup; // LevelSonuContainer objesindeki Canvas Group
    public TextMeshProUGUI levelYazisi; // Panel içindeki Text objesi

    void Awake()
    {
        // Oyun başında bu sistemi tamamen görünmez ve etkileşimsiz yap
        containerGroup.alpha = 0f;
        containerGroup.interactable = false;
        containerGroup.blocksRaycasts = false;
    }

    // Bu fonksiyonu çağırdığında içine ne yazarsan ekranda o patlayacak!
    public void BolumuBitir(string ekrandaYazacakMesaj)
    {
        levelYazisi.text = ekrandaYazacakMesaj;
        StartCoroutine(SinematikGiris());
    }

    IEnumerator SinematikGiris()
    {
        // 1. ADIM: Yazıyı sıfırla ve hazırla
        levelYazisi.transform.localScale = Vector3.zero;
        containerGroup.interactable = true;
        containerGroup.blocksRaycasts = true;

        float sure = 1.0f;
        float gecenZaman = 0f;

        // 2. ADIM: Kararma ve Yazının "Şekilli" Fırlaması
        while (gecenZaman < sure)
        {
            gecenZaman += Time.unscaledDeltaTime;
            float oran = gecenZaman / sure;

            // Arka plan yavaşça kararır
            containerGroup.alpha = Mathf.Lerp(0f, 1f, oran);

            // YAZININ ŞEKİLLİ ŞÜKÜLLÜ BÜYÜMESİ (Sıçrama efekti)
            // Önce 1.3 katına çıkar, sonra 1'e geri oturur
            float scale = oran < 0.6f ? Mathf.Lerp(0f, 1.3f, oran / 0.6f) : Mathf.Lerp(1.3f, 1f, (oran - 0.6f) / 0.4f);
            levelYazisi.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        // 3. ADIM: Zamanı durdur (Oyuncunun başarısını kutla)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; // Fareyi serbest bırak (Belki buton koyarsın)
        Cursor.visible = true;
    }
}