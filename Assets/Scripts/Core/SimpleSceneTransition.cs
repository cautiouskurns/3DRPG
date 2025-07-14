using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleSceneTransition : MonoBehaviour
{
    public static SimpleSceneTransition Instance { get; private set; }
    
    [Header("Transition Settings")]
    public float transitionDelay = 0.1f;
    public bool preservePlayerState = true;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private PlayerStateData savedPlayerState;
    private bool isTransitioning = false;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("SimpleSceneTransition: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("SimpleSceneTransition: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Subscribe to scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        if (enableDebugLogs)
        {
            Debug.Log("SimpleSceneTransition: Initialized and ready");
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void TransitionToScene(string sceneName, string spawnPointName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("SimpleSceneTransition: Already transitioning, ignoring request");
            return;
        }
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SimpleSceneTransition: Scene name is null or empty!");
            return;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Starting transition to {sceneName} with spawn point {spawnPointName}");
        }
        
        // Start transition coroutine
        StartCoroutine(TransitionCoroutine(sceneName, spawnPointName));
    }
    
    private IEnumerator TransitionCoroutine(string sceneName, string spawnPointName)
    {
        isTransitioning = true;
        
        // Save player state before transition
        if (preservePlayerState)
        {
            SavePlayerState();
        }
        
        // Publish transition start event
        EventBus.Publish(new SceneTransitionStartEvent(sceneName, spawnPointName));
        
        // Optional delay for any pre-transition effects
        if (transitionDelay > 0)
        {
            yield return new WaitForSeconds(transitionDelay);
        }
        
        // Store spawn point name for restoration
        PlayerPrefs.SetString("NextSpawnPoint", spawnPointName);
        PlayerPrefs.Save();
        
        // Load the target scene
        try
        {
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleSceneTransition: Loading scene '{sceneName}'");
            }
            
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SimpleSceneTransition: Failed to load scene '{sceneName}': {e.Message}");
            isTransitioning = false;
        }
    }
    
    private void SavePlayerState()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            savedPlayerState = new PlayerStateData(player);
            savedPlayerState.SetTransitionTimestamp(); // Set timestamp when transition starts
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleSceneTransition: Player state saved - {savedPlayerState.GetSummary()}");
            }
        }
        else
        {
            Debug.LogWarning("SimpleSceneTransition: No PlayerController found to save state");
            savedPlayerState = new PlayerStateData(); // Create default state
            savedPlayerState.SetTransitionTimestamp();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Scene loaded - {scene.name}");
        }
        
        // Restore player state after scene loads
        if (preservePlayerState && savedPlayerState != null)
        {
            string spawnPointName = PlayerPrefs.GetString("NextSpawnPoint", "PlayerSpawn");
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleSceneTransition: Retrieved spawn point name from PlayerPrefs: '{spawnPointName}'");
            }
            StartCoroutine(RestorePlayerStateDelayed(spawnPointName));
        }
        
        isTransitioning = false;
        
        // Publish transition complete event
        EventBus.Publish(new SceneTransitionCompleteEvent(scene.name));
    }
    
    private IEnumerator RestorePlayerStateDelayed(string spawnPointName)
    {
        // Wait a frame for all objects to initialize
        yield return null;
        
        // Wait for spawn points to be created (they might be created by SpawnPointHelper)
        float waitTime = 0f;
        const float maxWaitTime = 2f; // Maximum time to wait for spawn points
        
        while (waitTime < maxWaitTime)
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName);
            if (spawnPoint != null)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"SimpleSceneTransition: Spawn point '{spawnPointName}' found after {waitTime:F2}s");
                }
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }
        
        RestorePlayerState(spawnPointName);
        
        // Wait another frame and check if position was overridden
        yield return null;
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Player position one frame after restore: {player.transform.position}");
        }
    }
    
    private void RestorePlayerState(string spawnPointName)
    {
        // Find the player in the new scene
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("SimpleSceneTransition: No PlayerController found in new scene!");
            return;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Player found at initial position: {player.transform.position}");
        }
        
        // Find spawn point position
        Vector3 spawnPosition = FindSpawnPoint(spawnPointName);
        
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Target spawn position: {spawnPosition}");
        }
        
        // Apply saved state with new position
        savedPlayerState.ApplyToPlayer(player, spawnPosition);
        
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Player position after applying state: {player.transform.position}");
        }
        
        // Ensure camera follows player
        CameraController camera = FindFirstObjectByType<CameraController>();
        if (camera != null)
        {
            camera.SetTarget(player.transform);
            
            if (enableDebugLogs)
            {
                Debug.Log("SimpleSceneTransition: Camera target updated");
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Player state restored at {spawnPosition}");
        }
    }
    
    private Vector3 FindSpawnPoint(string spawnPointName)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Looking for spawn point '{spawnPointName}'");
        }
        
        // Look for spawn point GameObject by name
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleSceneTransition: Found spawn point '{spawnPointName}' at {spawnPoint.transform.position}");
            }
            return spawnPoint.transform.position;
        }
        
        // Look for spawn point by tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (enableDebugLogs)
        {
            Debug.Log($"SimpleSceneTransition: Found {spawnPoints.Length} GameObjects with 'SpawnPoint' tag");
        }
        
        foreach (GameObject point in spawnPoints)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleSceneTransition: Checking spawn point '{point.name}'");
            }
            
            if (point.name == spawnPointName)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"SimpleSceneTransition: Found tagged spawn point '{spawnPointName}' at {point.transform.position}");
                }
                return point.transform.position;
            }
        }
        
        // Fallback: look for any spawn point
        if (spawnPoints.Length > 0)
        {
            Debug.LogWarning($"SimpleSceneTransition: Spawn point '{spawnPointName}' not found, using first available spawn point '{spawnPoints[0].name}'");
            return spawnPoints[0].transform.position;
        }
        
        // Final fallback: use origin with slight offset
        Debug.LogError($"SimpleSceneTransition: No spawn points found for '{spawnPointName}', using origin - THIS IS THE PROBLEM!");
        return new Vector3(0, 1, 0); // Slightly above ground to avoid collision issues
    }
    
    // Public method to get current transition state
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    
    // Public method to get saved player state (for debugging)
    public PlayerStateData GetSavedPlayerState()
    {
        return savedPlayerState;
    }
    
    // Public method to force save current player state
    public void ForceSavePlayerState()
    {
        SavePlayerState();
    }
    
    // Public method to validate scene transition setup
    public bool ValidateTransitionSetup()
    {
        bool valid = true;
        
        // Check if we have required components
        GameManager gameManager = GameManager.Instance;
        InputManager inputManager = InputManager.Instance;
        PlayerController player = FindFirstObjectByType<PlayerController>();
        
        if (gameManager == null)
        {
            Debug.LogError("SceneTransition validation: GameManager not found!");
            valid = false;
        }
        
        if (inputManager == null)
        {
            Debug.LogError("SceneTransition validation: InputManager not found!");
            valid = false;
        }
        
        if (player == null)
        {
            Debug.LogError("SceneTransition validation: PlayerController not found!");
            valid = false;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SceneTransition validation: {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
    
    // Public method to get the main village scene name
    public static string GetVillageSceneName()
    {
        // Try to determine the village scene name
        // This could be the first scene in build settings or a specifically named scene
        string[] possibleNames = { "Village"};
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
#if UNITY_EDITOR
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
#else
            string sceneName = "Scene" + i; // Fallback for builds
#endif
            
            foreach (string possibleName in possibleNames)
            {
                if (sceneName.Contains(possibleName))
                {
                    return sceneName;
                }
            }
        }
        
        // Fallback to first scene in build settings
        if (SceneManager.sceneCountInBuildSettings > 0)
        {
#if UNITY_EDITOR
            string firstScenePath = SceneUtility.GetScenePathByBuildIndex(0);
            string firstSceneName = System.IO.Path.GetFileNameWithoutExtension(firstScenePath);
#else
            string firstSceneName = "Scene0"; // Fallback for builds
#endif
            Debug.LogWarning($"SimpleSceneTransition: Using first scene in build settings as village: {firstSceneName}");
            return firstSceneName;
        }
        
        Debug.LogError("SimpleSceneTransition: No scenes found in build settings!");
        return "Village"; // Final fallback
    }
}

// Event classes for scene transitions
public class SceneTransitionStartEvent : IGameEvent
{
    public string targetSceneName;
    public string spawnPointName;
    public float Timestamp { get; private set; }
    
    public SceneTransitionStartEvent(string sceneName, string spawnPoint)
    {
        targetSceneName = sceneName;
        spawnPointName = spawnPoint;
        Timestamp = Time.time;
    }
}

public class SceneTransitionCompleteEvent : IGameEvent
{
    public string loadedSceneName;
    public float Timestamp { get; private set; }
    
    public SceneTransitionCompleteEvent(string sceneName)
    {
        loadedSceneName = sceneName;
        Timestamp = Time.time;
    }
}