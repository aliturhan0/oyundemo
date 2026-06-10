using UnityEngine;

public class AyakSesleri : MonoBehaviour
{
    [Header("Ses Kaynakları")]
    public AudioSource ayakSesKaynagi;
    public AudioClip yurumeSesi;
    public AudioClip kosmaSesi;

    [Header("Adım Aralıkları (Saniye)")]
    public float yurumeAraligi = 0.5f; // Yürürken kaç saniyede bir ses çıksın
    public float kosmaAraligi = 0.3f;  // Koşarken kaç saniyede bir ses çıksın

    private float zamanlayici = 0f;

    void Update()
    {
        // Klavyeden W,A,S,D tuşlarına basılıyor mu kontrol et
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");
        bool hareketEdiyor = (yatay != 0 || dikey != 0);

        if (hareketEdiyor)
        {
            // Sol Shift tuşuna basılıyorsa koşuyor demektir
            bool kosuyor = Input.GetKey(KeyCode.LeftShift);

            // Zamanlayıcıyı geriye saydır
            zamanlayici -= Time.deltaTime;

            if (zamanlayici <= 0f)
            {
                // Koşuyorsa koşma sesini, yürüyorsa yürüme sesini kasede tak
                ayakSesKaynagi.clip = kosuyor ? kosmaSesi : yurumeSesi;
                
                // Ses ayarlarını yap ve çal (Sesi biraz değiştirerek robotik olmasını engelliyoruz)
                ayakSesKaynagi.volume = kosuyor ? 1f : 0.6f; 
                ayakSesKaynagi.pitch = Random.Range(0.9f, 1.1f); // Her adımda hafif farklı ton
                ayakSesKaynagi.Play();

                // Zamanlayıcıyı tekrar kur
                zamanlayici = kosuyor ? kosmaAraligi : yurumeAraligi;
            }
        }
        else
        {
            // Oyuncu durduğunda zamanlayıcıyı sıfırla ki anında ses kesilsin
            zamanlayici = 0f;
        }
    }
}