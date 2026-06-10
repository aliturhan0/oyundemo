using UnityEngine;
using System.Collections; // Coroutine için

public class SilahSistemi : MonoBehaviour
{
    [Header("Silah Ayarları")]
    public float menzil = 100f;
    public int maksimumMermi = 30;
    [Tooltip("Toplam yedek mermi (reload için). -1 = sınırsız yedek.")]
    public int yedekMermi = 90;            // YENİ: artık reload sınırsız değil
    public float sarjorDegismeSuresi = 2f; // Animasyon uzunluğuna göre ayarla

    [Header("Ateş Modu")]
    [Tooltip("Açık = basılı tutunca otomatik ateş (AK-12 ile aynı his). Kapalı = her tık 1 atış.")]
    public bool otomatik = true;           // YENİ
    [Tooltip("İki atış arası süre (saniye). 0.1 ≈ 600 RPM.")]
    public float atesHizi = 0.1f;          // YENİ

    [Header("Bağlantılar (Inspector'dan Sürüklenecekler)")]
    public ParticleSystem atesEfekti;
    public AudioSource silahSesi;
    public Animator silahAnimator;         // Kolların animasyon yöneticisi
    [Tooltip("Şarjör boşken tetiğe basınca çalacak 'tık' sesi (opsiyonel).")]
    public AudioSource bosSarjorSesi;      // YENİ opsiyonel

    private int mevcutMermi;
    private bool sarjorDegisiyor = false;
    private float sonrakiAtesZamani;

    // HUD için dışarıdan okunabilir.
    public int MevcutMermi => mevcutMermi;
    public int YedekMermi => yedekMermi;

    void Start()
    {
        // Oyun başlarken şarjörü doldur.
        mevcutMermi = maksimumMermi;
    }

    void Update()
    {
        if (sarjorDegisiyor) return; // şarjör değişirken başka işlem yok

        // R ile reload — sadece şarjör dolu değilse VE yedek varsa.
        if (Input.GetKeyDown(KeyCode.R) && mevcutMermi < maksimumMermi && yedekMermi != 0)
        {
            StartCoroutine(SarjorDegistir());
            return;
        }

        // Ateş girdisi: otomatik = basılı tut, yarı-otomatik = tek tık.
        bool atesGirdisi = otomatik ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (atesGirdisi && Time.time >= sonrakiAtesZamani)
        {
            if (mevcutMermi > 0)
            {
                sonrakiAtesZamani = Time.time + atesHizi;
                AtesEt();
            }
            else
            {
                // Şarjör boş: ateş etme, "boş" sesi çal.
                if (Input.GetMouseButtonDown(0) && bosSarjorSesi != null) bosSarjorSesi.Play();
                sonrakiAtesZamani = Time.time + atesHizi;
            }
        }
    }

    void AtesEt()
    {
        mevcutMermi--; // Mermiyi eksilt

        // 1. Namlu ateşi
        if (atesEfekti != null) atesEfekti.Play();
        // 2. Silah sesi
        if (silahSesi != null) silahSesi.Play();
        // 3. Ateş animasyonu
        if (silahAnimator != null) silahAnimator.Play("Fire", 0, 0f);

        // 4. Raycast (hasar / iz buradan gidiyor)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, menzil))
        {
            Debug.Log("Vurulan: " + hit.transform.name);
        }
    }

    /// <summary>Yerdeki şarjör kutusu (SarjorAlma) bunu çağırır: yedek mermiye ekler.</summary>
    public void MermiEkle(int adet)
    {
        if (yedekMermi < 0) return; // zaten sınırsız
        yedekMermi += adet;
        Debug.Log("Mermi alındı. Yeni yedek: " + yedekMermi);
    }

    // Zaman ayarlı şarjör değiştirme.
    IEnumerator SarjorDegistir()
    {
        sarjorDegisiyor = true;

        if (silahAnimator != null) silahAnimator.Play("Reload");

        yield return new WaitForSeconds(sarjorDegismeSuresi);

        // Şarjörü yedekten doldur (yedek azalır).
        int ihtiyac = maksimumMermi - mevcutMermi;
        if (yedekMermi < 0)
        {
            mevcutMermi = maksimumMermi; // sınırsız yedek
        }
        else
        {
            int alinan = Mathf.Min(ihtiyac, yedekMermi);
            mevcutMermi += alinan;
            yedekMermi  -= alinan;
        }

        sarjorDegisiyor = false;
    }
}
