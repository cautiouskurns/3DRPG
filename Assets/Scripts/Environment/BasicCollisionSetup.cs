using UnityEngine;

[ExecuteInEditMode]
public class BasicCollisionSetup : MonoBehaviour
{
    [Header("Collision Settings")]
    public bool setupOnStart = true;
    public string buildingTag = "Building";
    public string propTag = "Prop";
    
    [Header("Boundary Settings")]
    public Vector2 villageSize = new Vector2(60, 60);
    public float boundaryHeight = 2f;
    
    [Header("Editor Tools")]
    [Space]
    public bool generateInEditor = false;
    
    private GameObject collisionParent;
    
    void Start()
    {
        if (setupOnStart && Application.isPlaying) SetupVillageCollision();
    }
    
    void Update()
    {
        // Editor-time generation
        if (!Application.isPlaying && generateInEditor)
        {
            generateInEditor = false; // Reset the toggle
            SetupVillageCollision();
        }
    }
    
    public void SetupVillageCollision()
    {
        Debug.Log("Setting up village collision...");
        
        // Create parent GameObject for organization
        collisionParent = new GameObject("Village Collision");
        collisionParent.transform.position = Vector3.zero;
        
        SetupBuildingCollision();
        SetupVillageBoundaries();
        
        Debug.Log("Village collision setup complete");
    }
    
    void SetupBuildingCollision()
    {
        // Find all GameObjects with building tag and add box colliders
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(buildingTag);
        int collisionCount = 0;
        
        foreach (GameObject building in buildings)
        {
            // Check if building already has a collider
            if (building.GetComponent<Collider>() == null)
            {
                // Add BoxCollider
                BoxCollider collider = building.AddComponent<BoxCollider>();
                
                // Adjust collider size based on building renderer bounds
                Renderer renderer = building.GetComponent<Renderer>();
                if (renderer != null)
                {
                    collider.size = renderer.bounds.size;
                    collider.center = Vector3.zero;
                }
                
                collisionCount++;
                Debug.Log($"Added collision to building: {building.name}");
            }
        }
        
        // Also add collision to props
        GameObject[] props = GameObject.FindGameObjectsWithTag(propTag);
        foreach (GameObject prop in props)
        {
            if (prop.GetComponent<Collider>() == null)
            {
                BoxCollider collider = prop.AddComponent<BoxCollider>();
                
                // Adjust collider size
                Renderer renderer = prop.GetComponent<Renderer>();
                if (renderer != null)
                {
                    collider.size = renderer.bounds.size;
                    collider.center = Vector3.zero;
                }
                
                collisionCount++;
                Debug.Log($"Added collision to prop: {prop.name}");
            }
        }
        
        Debug.Log($"Set up collision for {collisionCount} objects");
    }
    
    void SetupVillageBoundaries()
    {
        // Create invisible walls around village perimeter
        float halfWidth = villageSize.x * 0.5f;
        float halfHeight = villageSize.y * 0.5f;
        
        CreateInvisibleWall(new Vector3(0, boundaryHeight * 0.5f, halfHeight), new Vector3(villageSize.x, boundaryHeight, 1), "North Boundary");
        CreateInvisibleWall(new Vector3(0, boundaryHeight * 0.5f, -halfHeight), new Vector3(villageSize.x, boundaryHeight, 1), "South Boundary");
        CreateInvisibleWall(new Vector3(halfWidth, boundaryHeight * 0.5f, 0), new Vector3(1, boundaryHeight, villageSize.y), "East Boundary");
        CreateInvisibleWall(new Vector3(-halfWidth, boundaryHeight * 0.5f, 0), new Vector3(1, boundaryHeight, villageSize.y), "West Boundary");
        
        Debug.Log($"Created village boundaries for {villageSize.x}x{villageSize.y} area");
    }
    
    void CreateInvisibleWall(Vector3 position, Vector3 size, string name)
    {
        // Create invisible GameObject with BoxCollider
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        wall.transform.SetParent(collisionParent.transform);
        
        // Add BoxCollider
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = size;
        
        // Tag as boundary
        wall.tag = "Boundary";
        
        // Make it invisible by not adding any renderer
        Debug.Log($"Created invisible wall: {name} at {position} with size {size}");
    }
    
    // Public method to recreate collision
    public void RecreateCollision()
    {
        // Clear existing collision
        if (collisionParent != null)
        {
            DestroyImmediate(collisionParent);
        }
        
        // Recreate
        SetupVillageCollision();
    }
    
    // Public method to validate collision setup
    public bool ValidateCollisionSetup()
    {
        bool isValid = true;
        
        // Check buildings have collision
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(buildingTag);
        int buildingsWithCollision = 0;
        
        foreach (GameObject building in buildings)
        {
            if (building.GetComponent<Collider>() != null)
            {
                buildingsWithCollision++;
            }
        }
        
        if (buildingsWithCollision != buildings.Length)
        {
            Debug.LogWarning($"Collision validation failed: {buildingsWithCollision}/{buildings.Length} buildings have collision");
            isValid = false;
        }
        
        // Check boundaries exist
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        if (boundaries.Length < 4)
        {
            Debug.LogWarning($"Boundary validation failed: {boundaries.Length}/4 boundaries found");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("Collision validation passed");
        }
        
        return isValid;
    }
    
    // Public method to get collision parent for validation
    public GameObject GetCollisionParent()
    {
        return collisionParent;
    }
    
    // Public method to adjust village size
    public void SetVillageSize(Vector2 newSize)
    {
        villageSize = newSize;
        Debug.Log($"Village size set to {villageSize.x}x{villageSize.y}");
        
        // Recreate boundaries with new size
        if (collisionParent != null)
        {
            RecreateCollision();
        }
    }
}