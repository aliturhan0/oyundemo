using UnityEngine;
using InfimaGames.LowPolyShooterPack; // Infima silah sistemine erişim

/// <summary>
/// Yerdeki şarjör/mermi kutusu. Oyuncu üstüne gelince sahnedeki AK-12'nin
/// yedek mermisini gerçekten artırır, sonra kutuyu gizleyip siler.
/// NOT: Bu objede TRIGGER bir Collider olmalı (Is Trigger açık) ve oyuncuda "Player" tag'ı olmalı.
/// </summary>
public class SarjorAlma : MonoBehaviour
{
    [Header("Ne Kadar Mermi Versin (yedeğe eklenir)")]
    public int mermiMiktari = 30;

    [Header("Ses Efekti")]
    public AudioClip mermiAlmaSesi;

    private bool _alindi = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_alindi) return;
        if (!other.CompareTag("Player")) return;

        bool eklendi = false;

        // 1) ASIL SİLAH: Infima'nın o an kuşanılan silahının yedeğini doldur.
        try
        {
            var gms = ServiceLocator.Current != null ? ServiceLocator.Current.Get<IGameModeService>() : null;
            var karakter = gms != null ? gms.GetPlayerCharacter() : null;
            var envanter = karakter != null ? karakter.GetInventory() : null;
            var infimaSilah = envanter != null ? envanter.GetEquipped() : null;
            if (infimaSilah != null)
            {
                infimaSilah.AddAmmunitionReserve(mermiMiktari);
                eklendi = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("SarjorAlma: Infima silahına ulaşılamadı -> " + e.Message);
        }

        // 2) (Varsa) custom silahlar da dolsun — zararsız.
        AK12Silah ak = FindObjectOfType<AK12Silah>();
        if (ak != null) { ak.MermiEkle(mermiMiktari); eklendi = true; }
        SilahSistemi ss = FindObjectOfType<SilahSistemi>();
        if (ss != null) { ss.MermiEkle(mermiMiktari); eklendi = true; }

        if (eklendi)
            Debug.Log("Şarjör alındı! +" + mermiMiktari + " yedek mermi.");
        else
            Debug.LogWarning("SarjorAlma: Silah bulunamadı, mermi eklenemedi.");

        _alindi = true;

        // Kutuyu gizle + çarpışmayı kapat (tekrar alınmasın).
        foreach (Renderer gorsel in GetComponentsInChildren<Renderer>()) gorsel.enabled = false;
        foreach (Collider carp in GetComponentsInChildren<Collider>()) carp.enabled = false;

        // Ses varsa çal ve ses bitince sil; yoksa hemen sil.
        if (mermiAlmaSesi != null)
        {
            AudioSource kasetCalar = gameObject.AddComponent<AudioSource>();
            kasetCalar.spatialBlend = 0f; // 2D, mesafeden bağımsız
            kasetCalar.volume = 1f;
            kasetCalar.PlayOneShot(mermiAlmaSesi);
            Destroy(gameObject, mermiAlmaSesi.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
