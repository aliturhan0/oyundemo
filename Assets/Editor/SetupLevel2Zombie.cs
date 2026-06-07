using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class SetupLevel2Zombie
{
    [InitializeOnLoadMethod]
    public static void Execute()
    {
        string scenePath = "Assets/Scenes/Level2.unity";
        string fbxPath = "Assets/zombi/mutant/character.fbx";
        string prefabPath = "Assets/zombi/mutant/MutantZombie.prefab";
        string controllerPath = "Assets/zombi/mutant/TankZombie_Anim.controller";

        Debug.Log("=== SETUP LEVEL 2 ZOMBIE PROCESS STARTED ===");

        // 1. Load scene
        var scene = EditorSceneManager.OpenScene(scenePath);
        if (!scene.IsValid())
        {
            Debug.LogError("Setup hatası: Level2 sahnesi açılamadı!");
            return;
        }

        // 2. Remove static 'character' instance if it exists in the scene
        GameObject staticChar = GameObject.Find("character");
        if (staticChar != null)
        {
            Debug.Log("Sahnede bulunan statik 'character' nesnesi temizleniyor (Spawner tarafından yaratılacak)...");
            GameObject.DestroyImmediate(staticChar);
        }

        // 3. Load Mutant FBX
        GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
        {
            Debug.LogError("FBX bulunamadı: " + fbxPath);
            return;
        }

        // 4. Create Mutant Prefab Asset
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        instance.name = "MutantZombie";

        // Add NavMeshAgent
        NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();
        if (agent == null) agent = instance.AddComponent<NavMeshAgent>();
        agent.speed = 2.5f;
        agent.stoppingDistance = 2f;

        // Add Rigidbody
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        if (rb == null) rb = instance.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Add CapsuleCollider
        CapsuleCollider col = instance.GetComponent<CapsuleCollider>();
        if (col == null) col = instance.AddComponent<CapsuleCollider>();
        col.radius = 0.45f;
        col.height = 2.2f;
        col.center = new Vector3(0, 1.1f, 0);
        col.isTrigger = false;

        // Add AudioSource
        AudioSource audio = instance.GetComponent<AudioSource>();
        if (audio == null) audio = instance.AddComponent<AudioSource>();
        audio.spatialBlend = 1.0f; // 3D Sound

        // Add Animator Controller
        Animator anim = instance.GetComponent<Animator>();
        if (anim != null)
        {
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
            if (controller != null)
            {
                anim.runtimeAnimatorController = controller;
            }
            else
            {
                Debug.LogError("Animator Controller bulunamadı: " + controllerPath);
            }
        }

        // Add EnemyAI script
        EnemyAI ai = instance.GetComponent<EnemyAI>();
        if (ai == null) ai = instance.AddComponent<EnemyAI>();
        ai.maxHealth = 150;
        ai.movementSpeed = 2.5f;
        ai.stoppingDistance = 2f;
        ai.attackDistance = 2f;
        ai.attackDamage = 20;
        ai.attackCooldown = 2.5f;
        ai.attackHitDelay = 0.7f;
        ai.audioSource = audio;

        // Assign sounds
        AudioClip ambient = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/zombi/ses/zombihirilti.wav");
        AudioClip hit = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/zombi/ses/zombiacı.wav");
        AudioClip attack = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/zombi/ses/zombivurma.wav");

        if (ambient != null) ai.ambientSounds = new AudioClip[] { ambient };
        if (hit != null) ai.hitSounds = new AudioClip[] { hit };
        if (attack != null) ai.attackSounds = new AudioClip[] { attack };

        // Save as prefab
        GameObject mutantPrefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Debug.Log("Mutant Zombie prefab başarıyla kaydedildi: " + prefabPath);

        // Destroy temporary scene instance
        GameObject.DestroyImmediate(instance);

        // 5. Find Spawner in scene and assign the new prefab
        ZombieSpawner spawner = GameObject.FindObjectOfType<ZombieSpawner>();
        if (spawner != null)
        {
            spawner.zombiePrefab = mutantPrefab;
            spawner.maxZombiesAlive = 6;
            spawner.spawnInterval = 4f;
            Debug.Log("ZombieSpawner'a yeni MutantZombie prefabi atandı!");
        }
        else
        {
            Debug.LogError("Sahnede ZombieSpawner bileşeni bulunamadı!");
        }

        // 6. Bake NavMesh in Level 2
        Debug.Log("Level 2 için NavMesh bake ediliyor...");
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        Debug.Log("NavMesh başarıyla bakelendi!");

        // 7. Save Scene and Assets
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Level 2 sahnesi başarıyla güncellendi ve kaydedildi!");
        Debug.Log("=== SETUP LEVEL 2 ZOMBIE PROCESS COMPLETED ===");

        // 8. Delete all setup editor scripts to clean up
        AssetDatabase.DeleteAsset("Assets/Editor/BakeNavMesh.cs");
        AssetDatabase.DeleteAsset("Assets/Editor/CreateMutantPrefab.cs");
        AssetDatabase.DeleteAsset("Assets/Editor/SetupLevel2Zombie.cs");
    }
}
