using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using InfimaGames.LowPolyShooterPack;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Movement")]
    public float movementSpeed = 3f;
    public float stoppingDistance = 2f;

    [Header("Hit Reaction")]
    public float hitStunDuration = 0.5f;

    [Header("Attack Settings")]
    public float attackDistance = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public float attackHitDelay = 0.5f; // Saldırı başladıktan kaç saniye sonra hasar verilecek
    private bool isAttacking = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] ambientSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] attackSounds;
    public float minAmbientTime = 3f;
    public float maxAmbientTime = 8f;
    private float nextAmbientTime;

    // Components
    private NavMeshAgent navAgent;
    private Animator animator;
    private CharacterBehaviour player;
    private PlayerHealth playerHealth;

    // States
    private bool isDead = false;
    private bool isHit = false;

    // Animation Hashes
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDeath = Animator.StringToHash("Death");
    private readonly int hashAttack = Animator.StringToHash("Attack");

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        navAgent.speed = movementSpeed;
        navAgent.stoppingDistance = stoppingDistance;
        
        currentHealth = maxHealth;
        SetNextAmbientTime();
    }

    private void Start()
    {
        // Bulunan oyuncuyu hedef olarak ayarla
        var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        if (gameModeService != null)
        {
            player = gameModeService.GetPlayerCharacter();
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        // Ortam seslerini çal
        if (Time.time >= nextAmbientTime)
        {
            PlayRandomSound(ambientSounds);
            SetNextAmbientTime();
        }

        if (isHit || isAttacking)
            return;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= attackDistance)
            {
                // Saldırı menzilinde
                StartCoroutine(AttackRoutine());
            }
            else
            {
                // Oyuncuya doğru hareket et
                navAgent.isStopped = false;
                navAgent.SetDestination(player.transform.position);
                animator.SetFloat(hashSpeed, navAgent.velocity.magnitude);
            }
        }
    }

    private void SetNextAmbientTime()
    {
        nextAmbientTime = Time.time + Random.Range(minAmbientTime, maxAmbientTime);
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (audioSource != null && clips != null && clips.Length > 0)
        {
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        navAgent.isStopped = true;
        animator.SetFloat(hashSpeed, 0); // Yürümeyi durdur
        animator.SetTrigger(hashAttack);

        PlayRandomSound(attackSounds); // Saldırı sesi

        // Vuruşun hedefe ulaşması için biraz bekle (animasyonun eli indirme anı)
        yield return new WaitForSeconds(attackHitDelay);

        if (!isDead && !isHit && playerHealth != null)
        {
            // Hala menzilde mi diye kontrol et
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackDistance + 0.5f) 
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // Saldırı soğuma süresi (animasyonun bitmesi + bekleme)
        yield return new WaitForSeconds(attackCooldown - attackHitDelay);
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("Zombi vuruldu! Alınan hasar: " + damageAmount + " | Kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayRandomSound(hitSounds); // Vurulma sesi
            StartCoroutine(HitRoutine());
        }
    }

    private IEnumerator HitRoutine()
    {
        isHit = true;
        navAgent.isStopped = true; // Hareketi durdur
        animator.SetTrigger(hashHit); // Vurulma animasyonunu oynat

        yield return new WaitForSeconds(hitStunDuration);

        if (!isDead)
        {
            isHit = false;
            navAgent.isStopped = false; // Harekete devam et
        }
    }

    private void Die()
    {
        isDead = true;
        navAgent.isStopped = true;
        animator.SetTrigger(hashDeath);
        
        // Zombi öldüğünde sistemi bilgilendir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ZombieKilled();
        }

        // Spawner'a haber ver ki haritada yeni zombiye yer açılsın
        var spawner = FindObjectOfType<ZombieSpawner>();
        if (spawner != null)
        {
            spawner.ZombieDied();
        }

        // Collider'ı kapat ki mermiler veya oyuncu cesede takılmasın
        var colliders = GetComponents<Collider>();
        foreach (var c in colliders) c.enabled = false;

        // Ölüm sesini çal (Hit seslerinden birini kullanabiliriz veya ayrı bir liste açabiliriz)
        PlayRandomSound(hitSounds);

        // Zombiyi 5 saniye sonra tamamen yok et ki oyun kasmasın
        Destroy(gameObject, 5f);
    }
}
