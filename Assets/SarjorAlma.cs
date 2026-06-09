using UnityEngine;

public class SarjorAlma : MonoBehaviour
{
    [Header("Ses Efekti")]
    public AudioClip mermiAlmaSesi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Şarjör Alındı! Mermiler Fullendi!");

            if (mermiAlmaSesi != null)
            {
                // 1. Kutuyu anında görünmez yap (Oyuncu mermiyi aldı sanacak)
                Renderer[] butunGorseller = GetComponentsInChildren<Renderer>();
                foreach (Renderer gorsel in butunGorseller) gorsel.enabled = false;
                
                // 2. Kutunun çarpışmasını kapat (İkinci kez alınmasın diye)
                Collider[] butunCarpismalar = GetComponentsInChildren<Collider>();
                foreach (Collider carp in butunCarpismalar) carp.enabled = false;

                // 3. Kutuya anında bir kaset çalar tak, 2D yapıp beynin içine ver!
                AudioSource kasetCalar = gameObject.AddComponent<AudioSource>();
                kasetCalar.spatialBlend = 0f; // 0 demek KESİNLİKLE 2D demek, mesafe tanımaz!
                kasetCalar.volume = 1f;
                kasetCalar.PlayOneShot(mermiAlmaSesi);

                // 4. Kutuyu ses bittikten TAM SONRA sil!
                Destroy(gameObject, mermiAlmaSesi.length);
            }
            else
            {
                // Kaset boşsa direkt sil
                Destroy(gameObject);
            }
        }
    }
}