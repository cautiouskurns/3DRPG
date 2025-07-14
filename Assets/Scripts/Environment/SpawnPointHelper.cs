using UnityEngine;

public class SpawnPointHelper : MonoBehaviour
{
    [Header("Spawn Point Creation")]
    public bool createVillageSpawnPoints = false;
    
    [Header("Debug")]
    public bool showSpawnPointGizmos = true;
    public Color gizmoColor = Color.green;
    
    void Start()
    {
        // Automatically create spawn points when scene loads (always ensure they exist)
        CreateVillageSpawnPointsIfNeeded();
        
        // Also handle the manual checkbox if checked
        if (createVillageSpawnPoints)
        {
            createVillageSpawnPoints = false; // Reset checkbox to prevent Update loop
        }
    }
    
    void Update()
    {
        // Create spawn points when checkbox is checked (for manual creation)
        if (createVillageSpawnPoints)
        {
            createVillageSpawnPoints = false;
            CreateVillageSpawnPoints();
        }
    }
    
    void CreateVillageSpawnPointsIfNeeded()
    {
        // Check if spawn points already exist before creating them
        string[] spawnPointNames = {
            "TownHall_ExitSpawn", "Shop_ExitSpawn", "Inn_ExitSpawn",
            "Blacksmith_ExitSpawn", "Chapel_ExitSpawn", "House_ExitSpawn"
        };
        
        bool anyMissing = false;
        foreach (string name in spawnPointNames)
        {
            if (GameObject.Find(name) == null)
            {
                anyMissing = true;
                break;
            }
        }
        
        if (anyMissing)
        {
            Debug.Log("Some spawn points missing, creating all village spawn points...");
            CreateVillageSpawnPoints();
        }
        else
        {
            Debug.Log("All village spawn points already exist, skipping creation.");
        }
    }
    
    void CreateVillageSpawnPoints()
    {
        Debug.Log("Creating village spawn points...");
        
        // Define spawn point data: name, position
        (string name, Vector3 position)[] spawnPoints = {
            ("TownHall_ExitSpawn", new Vector3(0, 1, 16)),
            ("Shop_ExitSpawn", new Vector3(15, 1, 1)),
            ("Inn_ExitSpawn", new Vector3(-15, 1, 1)),
            ("Blacksmith_ExitSpawn", new Vector3(15, 1, -11)),
            ("Chapel_ExitSpawn", new Vector3(-15, 1, 11)),
            ("House_ExitSpawn", new Vector3(0, 1, -16))
        };
        
        foreach (var (name, position) in spawnPoints)
        {
            // Check if spawn point already exists
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                Debug.LogWarning($"Spawn point '{name}' already exists at {existing.transform.position}");
                continue;
            }
            
            // Create new spawn point
            GameObject spawnPoint = new GameObject(name);
            spawnPoint.transform.position = position;
            spawnPoint.tag = "SpawnPoint";
            
            // Add visual indicator (small cube)
            GameObject visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visualIndicator.name = "Visual Indicator";
            visualIndicator.transform.SetParent(spawnPoint.transform);
            visualIndicator.transform.localPosition = Vector3.zero;
            visualIndicator.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
            
            // Make it semi-transparent
            Renderer renderer = visualIndicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0, 1, 0, 0.5f); // Semi-transparent green
                mat.SetFloat("_Mode", 3); // Set to Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                renderer.material = mat;
            }
            
            // Remove collider from visual indicator
            Collider indicatorCollider = visualIndicator.GetComponent<Collider>();
            if (indicatorCollider != null)
            {
                DestroyImmediate(indicatorCollider);
            }
            
            Debug.Log($"Created spawn point: {name} at {position}");
        }
        
        Debug.Log("Village spawn points creation complete!");
    }
    
    [ContextMenu("List All Spawn Points")]
    public void ListAllSpawnPoints()
    {
        Debug.Log("=== SPAWN POINT LISTING ===");
        
        // Find by name
        string[] expectedNames = {
            "TownHall_ExitSpawn", "Shop_ExitSpawn", "Inn_ExitSpawn",
            "Blacksmith_ExitSpawn", "Chapel_ExitSpawn", "House_ExitSpawn",
            "PlayerSpawn", "ExitSpawn"
        };
        
        foreach (string name in expectedNames)
        {
            GameObject spawn = GameObject.Find(name);
            if (spawn != null)
            {
                Debug.Log($"✅ Found: {name} at {spawn.transform.position}, Tag: {spawn.tag}");
            }
            else
            {
                Debug.Log($"❌ Missing: {name}");
            }
        }
        
        // Find by tag
        GameObject[] taggedSpawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log($"\nFound {taggedSpawns.Length} objects with 'SpawnPoint' tag:");
        foreach (GameObject spawn in taggedSpawns)
        {
            Debug.Log($"  - {spawn.name} at {spawn.transform.position}");
        }
        
        Debug.Log("=== SPAWN POINT LISTING COMPLETE ===");
    }
    
    [ContextMenu("Test Spawn Point Finding")]
    public void TestSpawnPointFinding()
    {
        Debug.Log("=== TESTING SPAWN POINT FINDING ===");
        
        string[] testNames = { "TownHall_ExitSpawn", "Shop_ExitSpawn", "PlayerSpawn" };
        
        foreach (string testName in testNames)
        {
            // Test the same logic as SimpleSceneTransition
            GameObject spawnPoint = GameObject.Find(testName);
            if (spawnPoint != null)
            {
                Debug.Log($"✅ Direct find: {testName} -> {spawnPoint.transform.position}");
                continue;
            }
            
            // Look for spawn point by tag
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            bool found = false;
            
            foreach (GameObject point in spawnPoints)
            {
                if (point.name == testName)
                {
                    Debug.Log($"✅ Tag search: {testName} -> {point.transform.position}");
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                Debug.LogError($"❌ Not found: {testName}");
            }
        }
        
        Debug.Log("=== SPAWN POINT TESTING COMPLETE ===");
    }
    
    void OnDrawGizmos()
    {
        if (!showSpawnPointGizmos) return;
        
        // Draw gizmos for all spawn points in scene
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        
        Gizmos.color = gizmoColor;
        foreach (GameObject spawn in spawnPoints)
        {
            Gizmos.DrawWireCube(spawn.transform.position + Vector3.up * 0.5f, new Vector3(1, 1, 1));
            Gizmos.DrawRay(spawn.transform.position, Vector3.up * 2);
        }
    }
}