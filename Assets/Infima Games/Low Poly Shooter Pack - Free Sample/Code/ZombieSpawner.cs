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

    private int currentZombiesAlive = 0;
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
            if (currentZombiesAlive < maxZombiesAlive)
            {
                SpawnZombie();
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
    }

    // Zombi öldüğünde EnemyAI içinden bu metot çağrılacak ki yeni zombilere yer açılsın
    public void ZombieDied()
    {
        currentZombiesAlive--;
    }
}
