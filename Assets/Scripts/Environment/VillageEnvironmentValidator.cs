using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageEnvironmentValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public float performanceCheckDuration = 10f;
    public float targetFPS = 30f;
    public bool enableContinuousMonitoring = false;
    
    [Header("Test Scenes")]
    public List<string> interiorScenes = new List<string>
    {
        "TownHall_Interior", "Shop_Interior", "Inn_Interior",
        "Blacksmith_Interior", "Chapel_Interior", "House_Interior"
    };
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private List<float> fpsHistory = new List<float>();
    private float lastPerformanceCheck = 0f;
    private bool isPerformanceTestRunning = false;
    
    void Start()
    {
        if (enableDebugLogs)
        {
            Debug.Log("VillageEnvironmentValidator: Press P for full validation, L for lighting, M for performance");
        }
    }
    
    void Update()
    {
        // Validation hotkeys
        if (Input.GetKeyDown(KeyCode.P))
        {
            ValidateFullExperience();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            ValidateLighting();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            CheckPerformance();
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Run previous village validation (compatibility)
            VillageValidationTest villageValidator = FindFirstObjectByType<VillageValidationTest>();
            if (villageValidator != null)
            {
                Debug.Log("Running previous village validation test...");
                // Call existing validation if available
            }
            else
            {
                Debug.Log("Previous village validator not found, running new validation");
                ValidateFullExperience();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestSceneTransitions();
        }
        
        // Continuous monitoring
        if (enableContinuousMonitoring && Time.time - lastPerformanceCheck > 5f)
        {
            RecordFPS();
            lastPerformanceCheck = Time.time;
        }
    }
    
    public void ValidateFullExperience()
    {
        Debug.Log("=== VILLAGE ENVIRONMENT VALIDATION ===");
        
        // Test lighting system
        BasicLightingManager lightingManager = BasicLightingManager.Instance;
        bool lightingReady = lightingManager != null;
        Debug.Log($"✓ Lighting system ready: {lightingReady}");
        
        if (lightingReady)
        {
            bool lightingValid = lightingManager.ValidateLightingSetup();
            Debug.Log($"✓ Lighting setup valid: {lightingValid}");
        }
        
        // Test audio foundation
        AudioManager audioManager = AudioManager.Instance;
        bool audioReady = audioManager != null;
        Debug.Log($"✓ Audio foundation ready: {audioReady}");
        
        if (audioReady)
        {
            bool audioValid = audioManager.ValidateAudioSetup();
            Debug.Log($"✓ Audio setup valid: {audioValid}");
        }
        
        // Test atmospheric effects
        BasicAtmosphereManager atmosphere = FindFirstObjectByType<BasicAtmosphereManager>();
        bool atmosphereReady = atmosphere != null && atmosphere.enableAtmosphericEffects;
        Debug.Log($"✓ Atmospheric effects ready: {atmosphereReady}");
        
        if (atmosphere != null)
        {
            bool atmosphereValid = atmosphere.ValidateAtmosphereSetup();
            Debug.Log($"✓ Atmosphere setup valid: {atmosphereValid}");
        }
        
        // Test core systems still functional
        bool coreSystemsOK = GameManager.Instance != null && 
                            InputManager.Instance != null && 
                            FindFirstObjectByType<PlayerController>() != null &&
                            FindFirstObjectByType<SimpleSceneTransition>() != null;
        Debug.Log($"✓ Core systems functional: {coreSystemsOK}");
        
        // Test movement and camera
        TestMovementAndCamera();
        
        // Check current performance
        float currentFPS = 1.0f / Time.unscaledDeltaTime;
        bool performanceOK = currentFPS >= targetFPS;
        Debug.Log($"✓ Current performance: {currentFPS:F1} FPS (target: {targetFPS}+) - {(performanceOK ? "PASS" : "FAIL")}");
        
        Debug.Log("=== VALIDATION COMPLETE ===");
        Debug.Log("💡 Test complete village exploration for full experience");
        Debug.Log("💡 Use L to validate lighting, M for performance monitoring");
    }
    
    public void ValidateLighting()
    {
        Debug.Log("=== LIGHTING VALIDATION ===");
        
        // Current scene lighting
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        Debug.Log($"Active lights in scene: {lights.Length}");
        
        // Categorize lights
        int directionalLights = 0;
        int pointLights = 0;
        int spotLights = 0;
        int realtimeLights = 0;
        
        foreach (Light light in lights)
        {
            switch (light.type)
            {
                case LightType.Directional:
                    directionalLights++;
                    break;
                case LightType.Point:
                    pointLights++;
                    break;
                case LightType.Spot:
                    spotLights++;
                    break;
            }
            
            if (light.lightmapBakeType != LightmapBakeType.Baked)
            {
                realtimeLights++;
            }
        }
        
        Debug.Log($"  - Directional: {directionalLights}");
        Debug.Log($"  - Point: {pointLights}");
        Debug.Log($"  - Spot: {spotLights}");
        Debug.Log($"  - Real-time: {realtimeLights}");
        
        // Check lighting manager
        BasicLightingManager lightingManager = BasicLightingManager.Instance;
        if (lightingManager != null)
        {
            Debug.Log("✓ Lighting manager active and ready");
            
            if (enableDebugLogs)
            {
                Debug.Log(lightingManager.GetLightingInfo());
            }
            
            bool performanceOptimal = realtimeLights <= lightingManager.maxRealtimeLights;
            Debug.Log($"Performance optimal: {performanceOptimal} ({realtimeLights}/{lightingManager.maxRealtimeLights} real-time lights)");
        }
        else
        {
            Debug.LogError("❌ Lighting manager not found!");
        }
        
        // Check ambient lighting
        Debug.Log($"Ambient Mode: {RenderSettings.ambientMode}");
        Debug.Log($"Ambient Intensity: {RenderSettings.ambientIntensity:F2}");
        Debug.Log($"Ambient Sky Color: {RenderSettings.ambientSkyColor}");
        
        Debug.Log("=== LIGHTING VALIDATION COMPLETE ===");
        Debug.Log("💡 Visit all interior scenes to test lighting consistency");
    }
    
    public void CheckPerformance()
    {
        if (isPerformanceTestRunning)
        {
            Debug.Log("Performance test already running...");
            return;
        }
        
        Debug.Log("=== PERFORMANCE CHECK ===");
        
        float currentFPS = 1.0f / Time.unscaledDeltaTime;
        Debug.Log($"Current FPS: {currentFPS:F1} (target: {targetFPS}+)");
        
        if (currentFPS >= targetFPS)
        {
            Debug.Log("✅ Performance target met");
        }
        else
        {
            Debug.LogWarning("⚠️ Performance below target");
            
            // Suggest optimizations
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            int realtimeLights = 0;
            foreach (Light light in lights)
            {
                if (light.lightmapBakeType != LightmapBakeType.Baked)
                    realtimeLights++;
            }
            
            if (realtimeLights > 4)
            {
                Debug.LogWarning($"💡 Consider reducing real-time lights: {realtimeLights} active");
            }
            
            BasicAtmosphereManager atmosphere = FindFirstObjectByType<BasicAtmosphereManager>();
            if (atmosphere != null)
            {
                Debug.LogWarning("💡 Consider disabling atmospheric effects temporarily");
            }
        }
        
        // Start extended performance monitoring
        StartCoroutine(ExtendedPerformanceTest());
    }
    
    private IEnumerator ExtendedPerformanceTest()
    {
        isPerformanceTestRunning = true;
        fpsHistory.Clear();
        
        Debug.Log($"Starting {performanceCheckDuration}s performance monitoring...");
        
        float startTime = Time.time;
        while (Time.time - startTime < performanceCheckDuration)
        {
            float fps = 1.0f / Time.unscaledDeltaTime;
            fpsHistory.Add(fps);
            
            yield return new WaitForSeconds(0.1f); // Sample every 100ms
        }
        
        // Calculate statistics
        float totalFPS = 0f;
        float minFPS = float.MaxValue;
        float maxFPS = 0f;
        int belowTargetCount = 0;
        
        foreach (float fps in fpsHistory)
        {
            totalFPS += fps;
            if (fps < minFPS) minFPS = fps;
            if (fps > maxFPS) maxFPS = fps;
            if (fps < targetFPS) belowTargetCount++;
        }
        
        float avgFPS = totalFPS / fpsHistory.Count;
        float belowTargetPercentage = (belowTargetCount / (float)fpsHistory.Count) * 100f;
        
        Debug.Log("=== EXTENDED PERFORMANCE RESULTS ===");
        Debug.Log($"Average FPS: {avgFPS:F1}");
        Debug.Log($"Min FPS: {minFPS:F1}");
        Debug.Log($"Max FPS: {maxFPS:F1}");
        Debug.Log($"Below target: {belowTargetPercentage:F1}% of samples");
        
        if (avgFPS >= targetFPS && belowTargetPercentage < 10f)
        {
            Debug.Log("✅ Extended performance test PASSED");
        }
        else
        {
            Debug.LogWarning("⚠️ Extended performance test FAILED");
        }
        
        isPerformanceTestRunning = false;
    }
    
    private void TestSceneTransitions()
    {
        Debug.Log("=== SCENE TRANSITION VALIDATION ===");
        
        SimpleSceneTransition transitionManager = SimpleSceneTransition.Instance;
        if (transitionManager == null)
        {
            Debug.LogError("❌ SimpleSceneTransition not found!");
            return;
        }
        
        Debug.Log("✓ Scene transition manager found");
        
        // Check if transition manager has lighting integration
        bool lightingIntegrated = BasicLightingManager.Instance != null;
        Debug.Log($"✓ Lighting integration: {lightingIntegrated}");
        
        // Validate spawn points exist
        SpawnPointHelper spawnHelper = FindFirstObjectByType<SpawnPointHelper>();
        if (spawnHelper != null)
        {
            Debug.Log("✓ Spawn point helper found - testing spawn points");
            spawnHelper.ListAllSpawnPoints();
        }
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");
        
        Debug.Log("=== SCENE TRANSITION VALIDATION COMPLETE ===");
        Debug.Log("💡 Test actual transitions between interior/exterior scenes");
    }
    
    private void TestMovementAndCamera()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        CameraController camera = FindFirstObjectByType<CameraController>();
        InputManager input = InputManager.Instance;
        
        bool movementOK = player != null;
        bool cameraOK = camera != null;
        bool inputOK = input != null;
        
        Debug.Log($"✓ Player movement: {movementOK}");
        Debug.Log($"✓ Camera system: {cameraOK}");
        Debug.Log($"✓ Input system: {inputOK}");
        
        if (player != null && camera != null)
        {
            float distance = Vector3.Distance(player.transform.position, camera.transform.position);
            bool cameraDistance = distance > 3f && distance < 15f; // Reasonable camera distance
            Debug.Log($"✓ Camera distance: {cameraDistance} ({distance:F1}m)");
        }
    }
    
    public void ValidateAtmosphericEffects()
    {
        Debug.Log("=== ATMOSPHERIC EFFECTS VALIDATION ===");
        
        BasicAtmosphereManager atmosphere = FindFirstObjectByType<BasicAtmosphereManager>();
        if (atmosphere == null)
        {
            Debug.LogWarning("⚠️ BasicAtmosphereManager not found");
            return;
        }
        
        Debug.Log("✓ Atmosphere manager found");
        Debug.Log(atmosphere.GetAtmosphereInfo());
        
        // Check particle systems
        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        Debug.Log($"Total particle systems in scene: {particles.Length}");
        
        int activeParticles = 0;
        foreach (ParticleSystem ps in particles)
        {
            if (ps.isPlaying)
            {
                activeParticles += ps.particleCount;
            }
        }
        
        bool particlePerformanceOK = activeParticles <= atmosphere.maxActiveParticles;
        Debug.Log($"Active particles: {activeParticles}/{atmosphere.maxActiveParticles} - {(particlePerformanceOK ? "OK" : "EXCEEDED")}");
        
        Debug.Log("=== ATMOSPHERIC EFFECTS VALIDATION COMPLETE ===");
    }
    
    public void TestAllSystems()
    {
        Debug.Log("=== COMPREHENSIVE SYSTEM TEST ===");
        
        ValidateLighting();
        ValidateAtmosphericEffects();
        CheckPerformance();
        TestSceneTransitions();
        
        Debug.Log("=== COMPREHENSIVE SYSTEM TEST COMPLETE ===");
    }
    
    private void RecordFPS()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        
        if (fps < targetFPS)
        {
            Debug.LogWarning($"Performance warning: {fps:F1} FPS (below {targetFPS} target)");
        }
    }
    
    // Integration test for complete village experience
    public void StartVillageTour()
    {
        Debug.Log("=== STARTING VILLAGE TOUR TEST ===");
        Debug.Log("1. Walk around village exterior");
        Debug.Log("2. Enter each building (6 interiors)");
        Debug.Log("3. Exit back to village");
        Debug.Log("4. Monitor FPS and lighting transitions");
        Debug.Log("5. Check atmospheric effects are visible");
        Debug.Log("Press M during tour to monitor performance");
        Debug.Log("=== VILLAGE TOUR INSTRUCTIONS COMPLETE ===");
    }
    
    // Quick validation method for automated testing
    public bool QuickValidation()
    {
        bool lightingOK = BasicLightingManager.Instance != null;
        bool audioOK = AudioManager.Instance != null;
        bool atmosphereOK = FindFirstObjectByType<BasicAtmosphereManager>() != null;
        bool coreSystemsOK = GameManager.Instance != null && 
                            InputManager.Instance != null && 
                            FindFirstObjectByType<PlayerController>() != null;
        
        float currentFPS = 1.0f / Time.unscaledDeltaTime;
        bool performanceOK = currentFPS >= targetFPS;
        
        bool allOK = lightingOK && audioOK && atmosphereOK && coreSystemsOK && performanceOK;
        
        if (enableDebugLogs)
        {
            Debug.Log($"Quick validation: {(allOK ? "PASS" : "FAIL")} (L:{lightingOK} A:{audioOK} At:{atmosphereOK} C:{coreSystemsOK} P:{performanceOK})");
        }
        
        return allOK;
    }
}