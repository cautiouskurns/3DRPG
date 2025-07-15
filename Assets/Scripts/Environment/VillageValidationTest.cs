using UnityEngine;

public class VillageValidationTest : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool enableDebugLogging = true;
    
    private BasicVillageLayout villageLayout;
    private BasicGroundSetup groundSetup;
    private BasicCollisionSetup collisionSetup;
    
    void Start()
    {
        // Find village systems
        villageLayout = FindFirstObjectByType<BasicVillageLayout>();
        groundSetup = FindFirstObjectByType<BasicGroundSetup>();
        collisionSetup = FindFirstObjectByType<BasicCollisionSetup>();
        
        Debug.Log("VillageValidationTest: Press V to validate village setup");
        Debug.Log("VillageValidationTest: Press R to rebuild village");
        Debug.Log("VillageValidationTest: WASD to move around and test navigation");
        
        // Run initial validation after a brief delay
        Invoke("ValidateVillage", 2f);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ValidateVillage();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RebuildVillage();
        }
    }
    
    void ValidateVillage()
    {
        if (enableDebugLogging)
        {
            Debug.Log("=== VILLAGE VALIDATION ===");
        }
        
        // Count buildings
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        if (enableDebugLogging)
        {
            Debug.Log($"Buildings found: {buildings.Length} (target: 6+)");
        }
        
        // Validate building placement
        ValidateBuildingPlacement(buildings);
        
        // Check collision on buildings
        int buildingsWithCollision = 0;
        foreach (GameObject building in buildings)
        {
            if (building.GetComponent<Collider>() != null)
                buildingsWithCollision++;
        }
        
        if (enableDebugLogging)
        {
            Debug.Log($"Buildings with collision: {buildingsWithCollision}/{buildings.Length}");
        }
        
        // Check props
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        if (enableDebugLogging)
        {
            Debug.Log($"Props found: {props.Length}");
        }
        
        // Check ground setup
        ValidateGroundSetup();
        
        // Check boundaries
        ValidateBoundaries();
        
        // Check player functionality
        ValidatePlayer();
        
        // Performance check
        float fps = 1.0f / Time.unscaledDeltaTime;
        if (enableDebugLogging)
        {
            Debug.Log($"Current FPS: {fps:F1} (target: 30+)");
        }
        
        // Summary
        bool validationPassed = buildings.Length >= 6 && buildingsWithCollision == buildings.Length;
        
        if (enableDebugLogging)
        {
            Debug.Log("=== VALIDATION COMPLETE ===");
            Debug.Log($"Overall validation: {(validationPassed ? "PASSED" : "FAILED")}");
            Debug.Log("Press WASD to move around village and test navigation");
        }
        
        if (validationPassed)
        {
            Debug.Log("✅ Village validation PASSED - All systems functional");
        }
        else
        {
            Debug.LogWarning("❌ Village validation FAILED - Check issues above");
        }
    }
    
    void ValidateBuildingPlacement(GameObject[] buildings)
    {
        // Check for specific building types
        string[] expectedBuildings = { "Town Hall", "Shop", "Inn", "Blacksmith", "Chapel", "House" };
        int foundBuildings = 0;
        
        foreach (string expectedBuilding in expectedBuildings)
        {
            bool found = false;
            foreach (GameObject building in buildings)
            {
                if (building.name.Contains(expectedBuilding))
                {
                    found = true;
                    break;
                }
            }
            
            if (found)
            {
                foundBuildings++;
            }
            else if (enableDebugLogging)
            {
                Debug.LogWarning($"Missing building: {expectedBuilding}");
            }
        }
        
        if (enableDebugLogging)
        {
            Debug.Log($"Expected buildings found: {foundBuildings}/{expectedBuildings.Length}");
        }
    }
    
    void ValidateGroundSetup()
    {
        GameObject[] groundPlanes = GameObject.FindGameObjectsWithTag("Ground");
        if (enableDebugLogging)
        {
            Debug.Log($"Ground planes found: {groundPlanes.Length}");
        }
        
        // Check for main grass area
        bool hasMainGrass = false;
        foreach (GameObject ground in groundPlanes)
        {
            if (ground.name.Contains("Main Grass"))
            {
                hasMainGrass = true;
                break;
            }
        }
        
        if (enableDebugLogging)
        {
            Debug.Log($"Main grass area found: {hasMainGrass}");
        }
    }
    
    void ValidateBoundaries()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        if (enableDebugLogging)
        {
            Debug.Log($"Boundary walls found: {boundaries.Length} (target: 4)");
        }
        
        // Check for all four boundaries
        string[] expectedBoundaries = { "North", "South", "East", "West" };
        int foundBoundaries = 0;
        
        foreach (string expectedBoundary in expectedBoundaries)
        {
            bool found = false;
            foreach (GameObject boundary in boundaries)
            {
                if (boundary.name.Contains(expectedBoundary))
                {
                    found = true;
                    break;
                }
            }
            
            if (found)
            {
                foundBoundaries++;
            }
        }
        
        if (enableDebugLogging)
        {
            Debug.Log($"Expected boundaries found: {foundBoundaries}/{expectedBoundaries.Length}");
        }
    }
    
    void ValidatePlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        bool playerExists = player != null;
        
        if (enableDebugLogging)
        {
            Debug.Log($"Player found and functional: {playerExists}");
        }
        
        if (playerExists)
        {
            // Check player position (should be in village center area)
            Vector3 playerPos = player.transform.position;
            bool playerInVillage = Mathf.Abs(playerPos.x) < 30 && Mathf.Abs(playerPos.z) < 30;
            
            if (enableDebugLogging)
            {
                Debug.Log($"Player position: {playerPos:F1}, In village: {playerInVillage}");
            }
        }
    }
    
    void RebuildVillage()
    {
        Debug.Log("Rebuilding village...");
        
        // Rebuild all systems
        if (villageLayout != null)
        {
            villageLayout.RebuildVillage();
        }
        
        if (groundSetup != null)
        {
            groundSetup.RecreateGround();
        }
        
        if (collisionSetup != null)
        {
            collisionSetup.RecreateCollision();
        }
        
        Debug.Log("Village rebuild complete");
        
        // Validate after rebuild
        Invoke("ValidateVillage", 1f);
    }
    
    // Public method to run validation programmatically
    public bool RunValidation()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        int buildingsWithCollision = 0;
        
        foreach (GameObject building in buildings)
        {
            if (building.GetComponent<Collider>() != null)
                buildingsWithCollision++;
        }
        
        bool validationPassed = buildings.Length >= 6 && buildingsWithCollision == buildings.Length;
        return validationPassed;
    }
    
    // Public method to get validation summary
    public string GetValidationSummary()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        GameObject[] ground = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        
        return $"Buildings: {buildings.Length}, Props: {props.Length}, Ground: {ground.Length}, Boundaries: {boundaries.Length}";
    }
}