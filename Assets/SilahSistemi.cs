using UnityEngine;
using System.Collections; // Zamanlayýcý (Coroutine) kullanmak için þart

public class SilahSistemi : MonoBehaviour
{
    [Header("Silah Ayarlarý")]
    public float menzil = 100f;
    public int maksimumMermi = 30;
    private int mevcutMermi;
    public float sarjorDegismeSuresi = 2f; // Animasyonun uzunluðuna göre ayarlarsýn
    private bool sarjorDegisiyor = false;

    [Header("Baðlantýlar (Inspector'dan Sürüklenecekler)")]
    public ParticleSystem atesEfekti;
    public AudioSource silahSesi;
    public Animator silahAnimator; // Kollarýn animasyon yöneticisi

    void Start()
    {
        // Oyun baþlarken þarjörü fullüyoruz
        mevcutMermi = maksimumMermi;
    }

    void Update()
    {
        if (sarjorDegisiyor) return; // Þarjör deðiþirken baþka iþlem yapma

        // R Tuþu ile Þarjör Deðiþtirme
        if (Input.GetKeyDown(KeyCode.R) && mevcutMermi < maksimumMermi)
        {
            StartCoroutine(SarjorDegistir());
            return;
        }

        // Sol týk (0) ile Ateþ Etme
        if (Input.GetMouseButtonDown(0) && mevcutMermi > 0)
        {
            AtesEt();
        }
        else if (Input.GetMouseButtonDown(0) && mevcutMermi <= 0)
        {
            Debug.Log("Mermi Bitti! R'ye bas!");
        }
    }

    void AtesEt()
    {
        mevcutMermi--; // Mermiyi eksilt

        // 1. Namlu Ateþini (Muzzle Flash) Patlat
        if (atesEfekti != null) atesEfekti.Play();

        // 2. Silah Sesini Çal
        if (silahSesi != null) silahSesi.Play();

        // 3. Ateþ Animasyonunu Oynat (Paketteki animasyon adý genelde "Fire"dýr)
        if (silahAnimator != null) silahAnimator.Play("Fire", 0, 0f);

        // 4. Görünmez Lazer (Raycast) Fýrlat
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, menzil))
        {
            Debug.Log("Vurulan: " + hit.transform.name);
        }
    }

    // Zaman ayarlý þarjör deðiþtirme fonksiyonu
    IEnumerator SarjorDegistir()
    {
        sarjorDegisiyor = true;
        Debug.Log("Þarjör Deðiþtiriliyor...");

        // Reload animasyonunu tetikle (Paketteki animasyon adý "Reload")
        if (silahAnimator != null) silahAnimator.Play("Reload");

        // Animasyon bitene kadar bekle
        yield return new WaitForSeconds(sarjorDegismeSuresi);

        mevcutMermi = maksimumMermi; // Mermiyi fulle
        sarjorDegisiyor = false;
        Debug.Log("Þarjör Doldu! Devam et!");
    }
}