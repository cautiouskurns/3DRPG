using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BasicVillageLayout : MonoBehaviour
{
    [Header("Building Prefabs")]
    public GameObject townHallPrefab;
    public GameObject shopPrefab;
    public GameObject innPrefab;
    public GameObject blacksmithPrefab;
    public GameObject chapelPrefab;
    public GameObject housePrefab;
    
    [Header("Building Materials")]
    public Material townHallMaterial;
    public Material shopMaterial;
    public Material innMaterial;
    public Material blacksmithMaterial;
    public Material chapelMaterial;
    public Material houseMaterial;
    
    [Header("Props")]
    public GameObject wellPrefab;
    public GameObject noticeBoardPrefab;
    public List<GameObject> decorativeProps = new List<GameObject>(); // barrels, crates, etc.
    
    [Header("Prop Materials")]
    public Material wellMaterial;
    public Material noticeBoardMaterial;
    public Material barrelMaterial;
    public Material crateMaterial;
    public Material fenceMaterial;
    public Material rockMaterial;
    
    [Header("Village Settings")]
    public Vector2 villageSize = new Vector2(60, 60);
    public bool buildOnStart = true;
    public bool useCustomMaterials = false;
    
    [Header("Editor Tools")]
    [Space]
    public bool generateInEditor = false;
    
    private GameObject villageParent;
    private List<GameObject> placedBuildings = new List<GameObject>();
    
    void Start()
    {
        if (buildOnStart && Application.isPlaying) BuildVillage();
    }
    
    void Update()
    {
        // Editor-time generation
        if (!Application.isPlaying && generateInEditor)
        {
            generateInEditor = false; // Reset the toggle
            BuildVillage();
        }
    }
    
    public void BuildVillage()
    {
        Debug.Log("Building village layout...");
        
        // Create parent GameObject for organization
        villageParent = new GameObject("Village");
        villageParent.transform.position = Vector3.zero;
        
        // Place 6 buildings in logical positions
        PlaceBuildingWithMaterial(townHallPrefab, new Vector3(0, 0, 20), Vector3.zero, "Town Hall", townHallMaterial);
        PlaceBuildingWithMaterial(shopPrefab, new Vector3(15, 0, 5), Vector3.zero, "Shop", shopMaterial);
        PlaceBuildingWithMaterial(innPrefab, new Vector3(-15, 0, 5), Vector3.zero, "Inn", innMaterial);
        PlaceBuildingWithMaterial(blacksmithPrefab, new Vector3(15, 0, -15), Vector3.zero, "Blacksmith", blacksmithMaterial);
        PlaceBuildingWithMaterial(chapelPrefab, new Vector3(-15, 0, 15), Vector3.zero, "Chapel", chapelMaterial);
        PlaceBuildingWithMaterial(housePrefab, new Vector3(0, 0, -20), Vector3.zero, "House", houseMaterial);
        
        // Place central features
        PlaceBuildingWithMaterial(wellPrefab, new Vector3(0, 0, 0), Vector3.zero, "Village Well", wellMaterial);
        PlaceBuildingWithMaterial(noticeBoardPrefab, new Vector3(-3, 0, 3), Vector3.zero, "Notice Board", noticeBoardMaterial);
        
        // Place some decorative props
        PlaceRandomProps();
        
        Debug.Log($"Village layout complete - {placedBuildings.Count} buildings placed");
    }
    
    void PlaceBuilding(GameObject prefab, Vector3 position, Vector3 rotation, string name)
    {
        GameObject building;
        
        // If prefab is assigned, instantiate it
        if (prefab != null)
        {
            building = Instantiate(prefab, position, Quaternion.Euler(rotation));
        }
        else
        {
            // Create placeholder using Unity primitive
            building = CreatePlaceholderBuilding(position, name);
        }
        
        // Set name and parent
        building.name = name;
        building.transform.SetParent(villageParent.transform);
        
        // Tag as building for collision system
        building.tag = "Building";
        
        // Add to building list
        placedBuildings.Add(building);
        
        Debug.Log($"Placed building: {name} at {position}");
    }
    
    void PlaceBuildingWithMaterial(GameObject prefab, Vector3 position, Vector3 rotation, string name, Material customMaterial)
    {
        GameObject building;
        
        // If prefab is assigned, instantiate it
        if (prefab != null)
        {
            building = Instantiate(prefab, position, Quaternion.Euler(rotation));
        }
        else
        {
            // Create placeholder using Unity primitive
            building = CreatePlaceholderBuilding(position, name);
        }
        
        // Set name and parent
        building.name = name;
        building.transform.SetParent(villageParent.transform);
        
        // Tag as building for collision system
        building.tag = "Building";
        
        // Apply custom material if provided and useCustomMaterials is enabled
        if (useCustomMaterials && customMaterial != null)
        {
            ApplyMaterialToBuilding(building, customMaterial);
        }
        else if (!useCustomMaterials && prefab == null)
        {
            // Use default color-based materials for placeholders
            SetBuildingColorFromName(building, name);
        }
        
        // Add to building list
        placedBuildings.Add(building);
        
        Debug.Log($"Placed building: {name} at {position}");
    }
    
    GameObject CreatePlaceholderBuilding(Vector3 position, string buildingType)
    {
        // Create cube primitive as placeholder
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.transform.position = position;
        
        // Set different sizes and colors based on building type
        switch (buildingType)
        {
            case "Town Hall":
                building.transform.localScale = new Vector3(8, 4, 6);
                SetBuildingColor(building, new Color(0.8f, 0.7f, 0.5f)); // Tan
                break;
            case "Shop":
                building.transform.localScale = new Vector3(6, 3, 4);
                SetBuildingColor(building, new Color(0.6f, 0.4f, 0.2f)); // Brown
                break;
            case "Inn":
                building.transform.localScale = new Vector3(7, 3, 5);
                SetBuildingColor(building, new Color(0.7f, 0.5f, 0.3f)); // Light brown
                break;
            case "Blacksmith":
                building.transform.localScale = new Vector3(5, 3, 4);
                SetBuildingColor(building, new Color(0.4f, 0.3f, 0.2f)); // Dark brown
                break;
            case "Chapel":
                building.transform.localScale = new Vector3(6, 5, 4);
                SetBuildingColor(building, new Color(0.9f, 0.9f, 0.8f)); // Light gray
                break;
            case "House":
                building.transform.localScale = new Vector3(5, 3, 4);
                SetBuildingColor(building, new Color(0.5f, 0.6f, 0.4f)); // Green-gray
                break;
            case "Village Well":
                building.transform.localScale = new Vector3(2, 1, 2);
                SetBuildingColor(building, new Color(0.6f, 0.6f, 0.6f)); // Gray
                break;
            case "Notice Board":
                building.transform.localScale = new Vector3(0.3f, 2, 1);
                SetBuildingColor(building, new Color(0.4f, 0.2f, 0.1f)); // Dark brown
                break;
            default:
                building.transform.localScale = new Vector3(3, 2, 3);
                SetBuildingColor(building, Color.gray);
                break;
        }
        
        // Adjust Y position based on building height
        float yOffset = building.transform.localScale.y * 0.5f;
        building.transform.position = new Vector3(position.x, yOffset, position.z);
        
        return building;
    }
    
    void SetBuildingColor(GameObject building, Color color)
    {
        Renderer renderer = building.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Try Standard shader first, fallback to Unlit if not available
            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
                Debug.LogWarning($"Standard shader not found, using Unlit/Color for {building.name}");
            }
            
            Material newMaterial = new Material(shader);
            
            // Set color based on shader type
            if (shader.name.Contains("Standard"))
            {
                newMaterial.color = color;
                newMaterial.SetFloat("_Metallic", 0f);
                newMaterial.SetFloat("_Glossiness", 0.3f);
            }
            else
            {
                // For Unlit shader, just set main color
                newMaterial.SetColor("_Color", color);
            }
            
            // Adjust UV tiling based on object scale for consistent appearance
            Vector3 scale = building.transform.localScale;
            Vector2 tiling = CalculateUVTiling(building, scale);
            newMaterial.mainTextureScale = tiling;
            
            renderer.material = newMaterial;
            Debug.Log($"Applied material to {building.name} using shader: {shader.name} with tiling {tiling}");
        }
    }
    
    void ApplyMaterialToBuilding(GameObject building, Material material)
    {
        Renderer renderer = building.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            // Create a new material instance to avoid affecting other objects
            Material materialInstance = new Material(material);
            
            // Adjust UV tiling based on object scale to maintain consistent texture density
            Vector3 scale = building.transform.localScale;
            Vector2 tiling = CalculateUVTiling(building, scale);
            
            // Apply tiling to main texture
            materialInstance.mainTextureScale = tiling;
            
            // Also apply to normal map if it exists
            if (materialInstance.HasProperty("_BumpMap"))
            {
                materialInstance.SetTextureScale("_BumpMap", tiling);
            }
            
            renderer.material = materialInstance;
            Debug.Log($"Applied custom material to {building.name} with tiling {tiling}");
        }
    }
    
    void SetBuildingColorFromName(GameObject building, string name)
    {
        Color color = GetDefaultColorForBuilding(name);
        SetBuildingColor(building, color);
    }
    
    Color GetDefaultColorForBuilding(string buildingName)
    {
        switch (buildingName)
        {
            case "Town Hall":
                return new Color(0.8f, 0.7f, 0.5f); // Tan
            case "Shop":
                return new Color(0.6f, 0.4f, 0.2f); // Brown
            case "Inn":
                return new Color(0.7f, 0.5f, 0.3f); // Light brown
            case "Blacksmith":
                return new Color(0.4f, 0.3f, 0.2f); // Dark brown
            case "Chapel":
                return new Color(0.9f, 0.9f, 0.8f); // Light gray
            case "House":
                return new Color(0.5f, 0.6f, 0.4f); // Green-gray
            case "Village Well":
                return new Color(0.6f, 0.6f, 0.6f); // Gray
            case "Notice Board":
                return new Color(0.4f, 0.2f, 0.1f); // Dark brown
            default:
                return Color.gray;
        }
    }
    
    Vector2 CalculateUVTiling(GameObject obj, Vector3 scale)
    {
        // Define texture scale factor - lower values = larger texture detail
        float textureScale = 1.0f;
        
        // Different tiling strategies based on object type
        if (obj.name.Contains("Barrel") || obj.name.Contains("Crate") || obj.name.Contains("Rock"))
        {
            // Props: use uniform scaling to maintain square textures
            float averageScale = (scale.x + scale.y + scale.z) / 3f;
            return Vector2.one * averageScale * textureScale;
        }
        else if (obj.name.Contains("Fence"))
        {
            // Fences: tile along length but not height
            return new Vector2(scale.x * textureScale, textureScale);
        }
        else if (obj.name.Contains("Hall") || obj.name.Contains("Shop") || obj.name.Contains("Inn") || 
                 obj.name.Contains("Blacksmith") || obj.name.Contains("Chapel") || obj.name.Contains("House"))
        {
            // Buildings: tile based on horizontal size, moderate vertical tiling
            return new Vector2(scale.x * textureScale * 0.5f, scale.y * textureScale * 0.3f);
        }
        else
        {
            // Default: tile based on X and Z (horizontal) scale
            return new Vector2(scale.x * textureScale, scale.z * textureScale);
        }
    }
    
    void PlaceRandomProps()
    {
        // Define prop positions around the village
        Vector3[] propPositions = {
            new Vector3(12, 0, 7),   // Near shop
            new Vector3(-12, 0, 7),  // Near inn
            new Vector3(17, 0, -12), // Near blacksmith
            new Vector3(-17, 0, 12), // Near chapel
            new Vector3(3, 0, -17),  // Near house
            new Vector3(-2, 0, 17),  // Near town hall
            new Vector3(5, 0, 2),    // Random placement
            new Vector3(-7, 0, -3)   // Random placement
        };
        
        for (int i = 0; i < propPositions.Length && i < 8; i++)
        {
            GameObject prop = CreateDecorativeProp(propPositions[i], i);
            if (prop != null)
            {
                prop.transform.SetParent(villageParent.transform);
                placedBuildings.Add(prop); // Add to building list for validation
            }
        }
        
        Debug.Log($"Placed {propPositions.Length} decorative props");
    }
    
    GameObject CreateDecorativeProp(Vector3 position, int index)
    {
        // Try to use assigned decorative props first
        if (decorativeProps.Count > 0 && decorativeProps[index % decorativeProps.Count] != null)
        {
            return Instantiate(decorativeProps[index % decorativeProps.Count], position, Quaternion.identity);
        }
        
        // Create placeholder props using primitives
        GameObject prop;
        string propName;
        
        switch (index % 4)
        {
            case 0: // Barrel
                prop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                prop.transform.localScale = new Vector3(0.8f, 1, 0.8f);
                propName = "Barrel";
                if (useCustomMaterials && barrelMaterial != null)
                    ApplyMaterialToBuilding(prop, barrelMaterial);
                else
                    SetBuildingColor(prop, new Color(0.6f, 0.4f, 0.2f));
                break;
            case 1: // Crate
                prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                prop.transform.localScale = new Vector3(1, 1, 1);
                propName = "Crate";
                if (useCustomMaterials && crateMaterial != null)
                    ApplyMaterialToBuilding(prop, crateMaterial);
                else
                    SetBuildingColor(prop, new Color(0.8f, 0.6f, 0.4f));
                break;
            case 2: // Small fence
                prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                prop.transform.localScale = new Vector3(2, 0.5f, 0.2f);
                propName = "Fence";
                if (useCustomMaterials && fenceMaterial != null)
                    ApplyMaterialToBuilding(prop, fenceMaterial);
                else
                    SetBuildingColor(prop, new Color(0.4f, 0.2f, 0.1f));
                break;
            default: // Rock
                prop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                prop.transform.localScale = new Vector3(0.7f, 0.5f, 0.7f);
                propName = "Rock";
                if (useCustomMaterials && rockMaterial != null)
                    ApplyMaterialToBuilding(prop, rockMaterial);
                else
                    SetBuildingColor(prop, new Color(0.5f, 0.5f, 0.5f));
                break;
        }
        
        prop.name = $"{propName}_{index}";
        prop.transform.position = new Vector3(position.x, prop.transform.localScale.y * 0.5f, position.z);
        prop.tag = "Prop";
        
        return prop;
    }
    
    // Public method to get placed buildings for validation
    public List<GameObject> GetPlacedBuildings()
    {
        return placedBuildings;
    }
    
    // Public method to rebuild village
    public void RebuildVillage()
    {
        // Clear existing village
        if (villageParent != null)
        {
            DestroyImmediate(villageParent);
        }
        
        placedBuildings.Clear();
        
        // Rebuild
        BuildVillage();
    }
}