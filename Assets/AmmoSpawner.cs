using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    [Header("Ne Doğacak?")]
    public GameObject sarjorPrefab; 
    private GameObject sahadakiSarjor;

    [Header("Harita Sınırları (Koordinatlar)")]
    // Kendi haritana göre bu sayıları değiştireceksin
    public float minX = -100f;
    public float maxX = 100f;
    public float minZ = -100f;
    public float maxZ = 100f;

    [Header("Yeniden Doğma Süresi")]
    public float beklemeSuresi = 10f;

    void Start()
    {
        // Oyun başlar başlamaz ilk şarjörü haritaya at
        YeniSarjorSpawnla();
    }

    void Update()
    {
        // Eğer sahadaki şarjör yok olduysa (oyuncu aldıysa) ve geri sayım saymıyorsa
        if (sahadakiSarjor == null && !IsInvoking("YeniSarjorSpawnla"))
        {
            // Belirlenen süre kadar bekle ve yenisini at
            Invoke("YeniSarjorSpawnla", beklemeSuresi);
        }
    }

    void YeniSarjorSpawnla()
    {
        // Harita sınırları içinde rastgele X ve Z koordinatı seç
        float rastgeleX = Random.Range(minX, maxX);
        float rastgeleZ = Random.Range(minZ, maxZ);

        Vector3 spawnNoktasi = new Vector3(rastgeleX, 0, rastgeleZ);
        
        // Kutunun dağın veya toprağın içine girmemesi için Terrain yüksekliğini ölç
        if (Terrain.activeTerrain != null)
        {
            float yerYuzeyi = Terrain.activeTerrain.SampleHeight(spawnNoktasi);
            spawnNoktasi.y = yerYuzeyi + 1f; // Yerden 1 metre yukarı koy
        }

        // Şarjörü o noktaya mühürle
        sahadakiSarjor = Instantiate(sarjorPrefab, spawnNoktasi, Quaternion.identity);
    }
}