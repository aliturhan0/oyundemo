using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;

    public float spawnInterval = 3f; // Kaç saniyede bir zombi çıksın?
    public int maxZombiesAlive = 5;  // Aynı anda haritada en fazla kaç zombi olabilir?

    [Header("Bolum Ayarlari")]
    public int bolumMaxZombiSayisi = 15; // BÖLÜMDE TOPLAM ÇIKACAK MAKSİMUM ZOMBİ SAYISI

    private int currentZombiesAlive = 0;
    private int totalSpawnedCount = 0; // Şu ana kadar kaç tane ürettik?
    private bool isSpawning = true;

    private void Start()
    {
        if (spawnPoints.Length == 0 || zombiePrefab == null)
        {
            Debug.LogError("Spawner hatası: Zombi Prefab'i veya Spawn noktaları atanmamış!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            // Hem sahnedeki kalabalık limiti dolmamışsa HEM DE bölüm kotasını aşmadıysak zombi bas
            if (currentZombiesAlive < maxZombiesAlive && totalSpawnedCount < bolumMaxZombiSayisi)
            {
                SpawnZombie();
            }
            // Eğer toplam ürettiğimiz zombi sayısı, bölüm sınırına ulaştıysa şalteri kapat!
            else if (totalSpawnedCount >= bolumMaxZombiSayisi)
            {
                isSpawning = false; // Spawner sistemini sonsuza dek durdur
                Debug.Log("<color=green>Bölüm kotası doldu! 15 Zombi basıldı, Spawner uyku moduna geçti.</color>");
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnZombie()
    {
        // Rastgele bir spawn noktası seç
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Zombiyi yarat
        GameObject newZombie = Instantiate(zombiePrefab, randomPoint.position, randomPoint.rotation);

        currentZombiesAlive++;
        totalSpawnedCount++; // Toplam üretilen zombi sayacı 1 arttı!
    }

    // Zombi öldüğünde EnemyAI içinden bu metot çağrılacak ki yeni zombilere yer açılsın
    public void ZombieDied()
    {
        currentZombiesAlive--;
    }
}