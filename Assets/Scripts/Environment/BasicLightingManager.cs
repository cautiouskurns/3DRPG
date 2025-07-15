using UnityEngine;
using UnityEngine.Rendering;

public class BasicLightingManager : MonoBehaviour
{
    public static BasicLightingManager Instance { get; private set; }
    
    [Header("Exterior Lighting")]
    public Color exteriorAmbientColor = new Color(0.7f, 0.8f, 1.0f);
    public float exteriorAmbientIntensity = 0.4f;
    public Color exteriorDirectionalColor = new Color(1.0f, 0.96f, 0.9f);
    public float exteriorDirectionalIntensity = 1.2f;
    public Vector3 exteriorDirectionalRotation = new Vector3(30f, 135f, 0f);
    
    [Header("Interior Lighting")]
    public Color interiorAmbientColor = new Color(1.0f, 0.95f, 0.9f);
    public float interiorAmbientIntensity = 0.6f;
    public Color interiorDirectionalColor = new Color(1.0f, 0.96f, 0.9f);
    public float interiorDirectionalIntensity = 0.8f;
    public Vector3 interiorDirectionalRotation = new Vector3(45f, 0f, 0f);
    
    [Header("Performance")]
    public int maxRealtimeLights = 4;
    public bool enableShadows = true;
    public ShadowQuality shadowQuality = ShadowQuality.All;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private Light mainDirectionalLight;
    private bool isInitialized = false;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("BasicLightingManager: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("BasicLightingManager: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeLightingSystem();
    }
    
    private void InitializeLightingSystem()
    {
        if (isInitialized) return;
        
        // Set initialized flag FIRST to prevent recursion
        isInitialized = true;
        
        // Find or create main directional light
        SetupMainDirectionalLight();
        
        // Apply performance optimizations
        OptimizeForPerformance();
        
        // Set initial lighting based on current scene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        ApplySceneLighting(currentScene);
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicLightingManager: Lighting system initialized");
        }
    }
    
    private void SetupMainDirectionalLight()
    {
        // Find existing directional light
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in allLights)
        {
            if (light.type == LightType.Directional)
            {
                mainDirectionalLight = light;
                break;
            }
        }
        
        // Create directional light if none exists
        if (mainDirectionalLight == null)
        {
            GameObject lightGO = new GameObject("Main Directional Light");
            mainDirectionalLight = lightGO.AddComponent<Light>();
            mainDirectionalLight.type = LightType.Directional;
            
            if (enableDebugLogs)
            {
                Debug.Log("BasicLightingManager: Created main directional light");
            }
        }
        
        // Configure directional light
        mainDirectionalLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None;
    }
    
    public void ApplyExteriorLighting()
    {
        if (!isInitialized)
        {
            InitializeLightingSystem();
        }
        
        // Set ambient lighting
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = exteriorAmbientColor;
        RenderSettings.ambientEquatorColor = exteriorAmbientColor * 0.8f;
        RenderSettings.ambientGroundColor = exteriorAmbientColor * 0.5f;
        RenderSettings.ambientIntensity = exteriorAmbientIntensity;
        
        // Set directional light
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.color = exteriorDirectionalColor;
            mainDirectionalLight.intensity = exteriorDirectionalIntensity;
            mainDirectionalLight.transform.rotation = Quaternion.Euler(exteriorDirectionalRotation);
            mainDirectionalLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicLightingManager: Applied exterior lighting preset");
        }
    }
    
    public void ApplyInteriorLighting()
    {
        if (!isInitialized)
        {
            InitializeLightingSystem();
        }
        
        // Set ambient lighting
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = interiorAmbientColor;
        RenderSettings.ambientEquatorColor = interiorAmbientColor * 0.9f;
        RenderSettings.ambientGroundColor = interiorAmbientColor * 0.7f;
        RenderSettings.ambientIntensity = interiorAmbientIntensity;
        
        // Set directional light (simulating window light)
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.color = interiorDirectionalColor;
            mainDirectionalLight.intensity = interiorDirectionalIntensity;
            mainDirectionalLight.transform.rotation = Quaternion.Euler(interiorDirectionalRotation);
            mainDirectionalLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicLightingManager: Applied interior lighting preset");
        }
    }
    
    public void ApplySceneLighting(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        
        // Determine lighting type based on scene name
        if (sceneName.Contains("Interior") || sceneName.Contains("interior"))
        {
            ApplyInteriorLighting();
        }
        else if (sceneName.Contains("Village") || sceneName.Contains("Exterior") || sceneName.Contains("exterior"))
        {
            ApplyExteriorLighting();
        }
        else
        {
            // Default to exterior lighting for unknown scenes
            ApplyExteriorLighting();
            
            if (enableDebugLogs)
            {
                Debug.Log($"BasicLightingManager: Unknown scene '{sceneName}', applying exterior lighting");
            }
        }
    }
    
    public void OptimizeForPerformance()
    {
        // Set quality settings for lighting
        QualitySettings.shadows = enableShadows ? ShadowQuality.All : ShadowQuality.Disable;
        QualitySettings.shadowResolution = ShadowResolution.Medium;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowDistance = 50f; // Reasonable shadow distance for village
        
        // Limit real-time lights
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        int realtimeLightCount = 0;
        
        foreach (Light light in allLights)
        {
            if (light.type != LightType.Directional)
            {
                if (realtimeLightCount < maxRealtimeLights)
                {
                    light.lightmapBakeType = LightmapBakeType.Mixed;
                    realtimeLightCount++;
                }
                else
                {
                    light.lightmapBakeType = LightmapBakeType.Baked;
                }
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicLightingManager: Performance optimized - {realtimeLightCount} real-time lights active");
        }
    }
    
    public void SetShadowsEnabled(bool enabled)
    {
        enableShadows = enabled;
        
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.shadows = enabled ? LightShadows.Soft : LightShadows.None;
        }
        
        QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicLightingManager: Shadows {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    public void SetMaxRealtimeLights(int maxLights)
    {
        maxRealtimeLights = Mathf.Clamp(maxLights, 1, 8);
        OptimizeForPerformance();
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicLightingManager: Max real-time lights set to {maxRealtimeLights}");
        }
    }
    
    // Get current lighting info for debugging
    public string GetLightingInfo()
    {
        string info = "=== LIGHTING INFO ===\n";
        info += $"Ambient Mode: {RenderSettings.ambientMode}\n";
        info += $"Ambient Intensity: {RenderSettings.ambientIntensity:F2}\n";
        info += $"Ambient Sky Color: {RenderSettings.ambientSkyColor}\n";
        
        if (mainDirectionalLight != null)
        {
            info += $"Directional Light Color: {mainDirectionalLight.color}\n";
            info += $"Directional Light Intensity: {mainDirectionalLight.intensity:F2}\n";
            info += $"Directional Light Rotation: {mainDirectionalLight.transform.rotation.eulerAngles}\n";
            info += $"Shadows: {mainDirectionalLight.shadows}\n";
        }
        
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        info += $"Total Lights: {allLights.Length}\n";
        
        return info;
    }
    
    // Validate lighting setup
    public bool ValidateLightingSetup()
    {
        bool valid = true;
        
        if (mainDirectionalLight == null)
        {
            Debug.LogError("BasicLightingManager: No main directional light found!");
            valid = false;
        }
        
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        int realtimeLights = 0;
        
        foreach (Light light in allLights)
        {
            if (light.type != LightType.Directional && light.lightmapBakeType != LightmapBakeType.Baked)
            {
                realtimeLights++;
            }
        }
        
        if (realtimeLights > maxRealtimeLights)
        {
            Debug.LogWarning($"BasicLightingManager: {realtimeLights} real-time lights exceed maximum of {maxRealtimeLights}");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicLightingManager: Lighting validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
}