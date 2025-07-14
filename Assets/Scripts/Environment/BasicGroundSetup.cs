using UnityEngine;

public class BasicGroundSetup : MonoBehaviour
{
    [Header("Ground Materials")]
    public Material grassMaterial;
    public Material stonePathMaterial;
    public Material dirtMaterial;
    public Material cobbleMaterial;
    
    [Header("Ground Settings")]
    public bool setupOnStart = true;
    public bool createMaterialsIfMissing = true;
    
    private GameObject groundParent;
    
    void Start()
    {
        if (setupOnStart) SetupVillageGround();
    }
    
    public void SetupVillageGround()
    {
        Debug.Log("Setting up village ground...");
        
        // Create parent GameObject for organization
        groundParent = new GameObject("Village Ground");
        groundParent.transform.position = Vector3.zero;
        
        // Create materials if missing
        if (createMaterialsIfMissing)
        {
            CreateMaterialsIfNeeded();
        }
        
        // Create main grass area (80x80 units) - base layer
        CreateGroundPlane(Vector3.zero, new Vector3(8, 1, 8), grassMaterial, "Main Grass Area");
        
        // Create stone paths connecting buildings - slightly elevated to prevent Z-fighting
        CreateGroundPlane(new Vector3(0, 0.02f, 0), new Vector3(0.4f, 1, 4), stonePathMaterial, "Main Path NS");
        CreateGroundPlane(new Vector3(0, 0.02f, 0), new Vector3(4, 1, 0.4f), stonePathMaterial, "Main Path EW");
        
        // Create additional paths to buildings - elevated more for layering
        CreateGroundPlane(new Vector3(7.5f, 0.03f, 2.5f), new Vector3(1.5f, 1, 0.3f), stonePathMaterial, "Path to Shop");
        CreateGroundPlane(new Vector3(-7.5f, 0.03f, 2.5f), new Vector3(1.5f, 1, 0.3f), stonePathMaterial, "Path to Inn");
        CreateGroundPlane(new Vector3(7.5f, 0.03f, -7.5f), new Vector3(1.5f, 1, 0.3f), stonePathMaterial, "Path to Blacksmith");
        CreateGroundPlane(new Vector3(-7.5f, 0.03f, 7.5f), new Vector3(1.5f, 1, 0.3f), stonePathMaterial, "Path to Chapel");
        
        // Create dirt area around blacksmith - elevated to show above grass
        CreateGroundPlane(new Vector3(15, 0.04f, -15), new Vector3(0.8f, 1, 0.8f), dirtMaterial, "Blacksmith Work Area");
        
        // Create cobblestone town center - highest layer for prominence
        CreateGroundPlane(new Vector3(0, 0.05f, 0), new Vector3(1, 1, 1), cobbleMaterial, "Town Square");
        
        Debug.Log("Village ground setup complete");
    }
    
    void CreateMaterialsIfNeeded()
    {
        if (grassMaterial == null)
        {
            grassMaterial = CreateBasicMaterial("Grass Material", new Color(0.3f, 0.7f, 0.3f));
        }
        
        if (stonePathMaterial == null)
        {
            stonePathMaterial = CreateBasicMaterial("Stone Path Material", new Color(0.6f, 0.6f, 0.6f));
        }
        
        if (dirtMaterial == null)
        {
            dirtMaterial = CreateBasicMaterial("Dirt Material", new Color(0.5f, 0.3f, 0.2f));
        }
        
        if (cobbleMaterial == null)
        {
            cobbleMaterial = CreateBasicMaterial("Cobble Material", new Color(0.4f, 0.4f, 0.4f));
        }
        
        Debug.Log("Created basic materials for ground textures");
    }
    
    Material CreateBasicMaterial(string name, Color color)
    {
        // Try Standard shader first, fallback to Unlit if not available
        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
            Debug.LogWarning($"Standard shader not found, using Unlit/Color for {name}");
        }
        
        Material material = new Material(shader);
        material.name = name;
        
        // Set color based on shader type
        if (shader.name.Contains("Standard"))
        {
            material.color = color;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Glossiness", 0.1f);
        }
        else
        {
            // For Unlit shader, just set main color
            material.SetColor("_Color", color);
        }
        
        Debug.Log($"Created material: {name} using shader: {shader.name}");
        return material;
    }
    
    void CreateGroundPlane(Vector3 position, Vector3 scale, Material material, string name)
    {
        // Create Unity plane primitive
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        // Set name and parent
        plane.name = name;
        plane.transform.SetParent(groundParent.transform);
        
        // Set position and scale
        plane.transform.position = position;
        plane.transform.localScale = scale;
        
        // Apply material
        if (material != null)
        {
            Renderer renderer = plane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
        }
        
        // Tag for identification
        plane.tag = "Ground";
        
        Debug.Log($"Created ground plane: {name} at {position} with scale {scale}");
    }
    
    // Public method to recreate ground
    public void RecreateGround()
    {
        // Clear existing ground
        if (groundParent != null)
        {
            DestroyImmediate(groundParent);
        }
        
        // Recreate
        SetupVillageGround();
    }
    
    // Public method to get ground parent for validation
    public GameObject GetGroundParent()
    {
        return groundParent;
    }
    
    // Public method to update a specific ground material
    public void UpdateGroundMaterial(string groundType, Material newMaterial)
    {
        switch (groundType.ToLower())
        {
            case "grass":
                grassMaterial = newMaterial;
                break;
            case "stone":
                stonePathMaterial = newMaterial;
                break;
            case "dirt":
                dirtMaterial = newMaterial;
                break;
            case "cobble":
                cobbleMaterial = newMaterial;
                break;
            default:
                Debug.LogWarning($"Unknown ground type: {groundType}");
                break;
        }
        
        Debug.Log($"Updated {groundType} material");
    }
}