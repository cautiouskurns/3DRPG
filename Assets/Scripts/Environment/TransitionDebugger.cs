using UnityEngine;

public class TransitionDebugger : MonoBehaviour
{
    [Header("Debug Controls")]
    public bool enableContinuousLogging = true;
    public float loggingInterval = 1f;
    
    private PlayerController player;
    private float lastLogTime;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        Debug.Log("TransitionDebugger: Press I to check current player state");
        Debug.Log("TransitionDebugger: Press O to test spawn point access");
        Debug.Log("TransitionDebugger: Press P to check PlayerPrefs");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            CheckPlayerState();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            TestSpawnPointAccess();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            CheckPlayerPrefs();
        }
        
        if (enableContinuousLogging && Time.time - lastLogTime > loggingInterval)
        {
            lastLogTime = Time.time;
            LogPlayerPosition();
        }
    }
    
    void CheckPlayerState()
    {
        Debug.Log("=== PLAYER STATE CHECK ===");
        
        if (player != null)
        {
            Debug.Log($"Player Position: {player.transform.position}");
            Debug.Log($"Player Rotation: {player.transform.rotation.eulerAngles}");
            
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"Rigidbody Position: {rb.position}");
                Debug.Log($"Rigidbody Velocity: {rb.linearVelocity}");
            }
        }
        else
        {
            Debug.LogError("No PlayerController found!");
        }
        
        Debug.Log("=== END PLAYER STATE ===");
    }
    
    void TestSpawnPointAccess()
    {
        Debug.Log("=== SPAWN POINT ACCESS TEST ===");
        
        // Test the exact logic from SimpleSceneTransition
        string testSpawnName = "TownHall_ExitSpawn";
        
        // Method 1: Direct find
        GameObject spawnPoint = GameObject.Find(testSpawnName);
        if (spawnPoint != null)
        {
            Debug.Log($"✅ Direct find success: {testSpawnName} at {spawnPoint.transform.position}");
        }
        else
        {
            Debug.Log($"❌ Direct find failed: {testSpawnName}");
        }
        
        // Method 2: Tag search
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log($"Found {spawnPoints.Length} objects with SpawnPoint tag");
        
        bool foundByTag = false;
        foreach (GameObject point in spawnPoints)
        {
            if (point.name == testSpawnName)
            {
                Debug.Log($"✅ Tag search success: {testSpawnName} at {point.transform.position}");
                foundByTag = true;
                break;
            }
        }
        
        if (!foundByTag)
        {
            Debug.Log($"❌ Tag search failed: {testSpawnName}");
        }
        
        Debug.Log("=== END SPAWN POINT TEST ===");
    }
    
    void CheckPlayerPrefs()
    {
        Debug.Log("=== PLAYERPREFS CHECK ===");
        
        string nextSpawnPoint = PlayerPrefs.GetString("NextSpawnPoint", "DEFAULT");
        Debug.Log($"NextSpawnPoint in PlayerPrefs: '{nextSpawnPoint}'");
        
        // List all spawn point names that should work
        string[] expectedSpawns = {
            "TownHall_ExitSpawn", "Shop_ExitSpawn", "Inn_ExitSpawn",
            "Blacksmith_ExitSpawn", "Chapel_ExitSpawn", "House_ExitSpawn"
        };
        
        Debug.Log("Expected spawn point names:");
        foreach (string name in expectedSpawns)
        {
            GameObject spawn = GameObject.Find(name);
            Debug.Log($"  {name}: {(spawn != null ? "EXISTS" : "MISSING")}");
        }
        
        Debug.Log("=== END PLAYERPREFS CHECK ===");
    }
    
    [ContextMenu("Clear PlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("NextSpawnPoint");
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared! NextSpawnPoint deleted.");
    }
    
    void LogPlayerPosition()
    {
        if (player != null)
        {
            Debug.Log($"[{Time.time:F1}s] Player at: {player.transform.position}");
        }
    }
    
    [ContextMenu("Force Set Player to Spawn Point")]
    public void ForceSetPlayerToSpawnPoint()
    {
        Debug.Log("=== FORCE SET PLAYER TO SPAWN POINT ===");
        
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
            Debug.Log($"Player reference: {(player != null ? "FOUND" : "NULL")}");
        }
        
        if (player != null)
        {
            Vector3 originalPosition = player.transform.position;
            Debug.Log($"Player original position: {originalPosition}");
            
            GameObject spawn = GameObject.Find("TownHall_ExitSpawn");
            if (spawn != null)
            {
                Vector3 targetPosition = spawn.transform.position;
                Debug.Log($"Target spawn position: {targetPosition}");
                
                player.transform.position = targetPosition;
                Debug.Log($"Set transform.position to: {targetPosition}");
                Debug.Log($"Player transform.position after set: {player.transform.position}");
                
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.position = targetPosition;
                    rb.linearVelocity = Vector3.zero;
                    Debug.Log($"Set rigidbody.position to: {targetPosition}");
                    Debug.Log($"Rigidbody position after set: {rb.position}");
                }
                else
                {
                    Debug.LogWarning("No Rigidbody found on player");
                }
                
                Debug.Log($"✅ Force positioning complete! Player should be at {targetPosition}");
            }
            else
            {
                Debug.LogError("❌ TownHall_ExitSpawn not found!");
            }
        }
        else
        {
            Debug.LogError("❌ PlayerController not found!");
        }
        
        Debug.Log("=== END FORCE SET ===");
    }
}