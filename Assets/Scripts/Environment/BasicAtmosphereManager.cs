using System.Collections.Generic;
using UnityEngine;

public class BasicAtmosphereManager : MonoBehaviour
{
    [Header("Atmospheric Effects")]
    public bool enableAtmosphericEffects = true;
    public GameObject chimneySmokePrefab;
    public List<Transform> smokeSpawnPoints = new List<Transform>();
    
    [Header("Effect Settings")]
    public int maxActiveParticles = 50;
    public float smokeEmissionRate = 10f;
    public float smokeLifetime = 3f;
    public Vector3 smokeVelocity = new Vector3(0, 2f, 0.5f);
    
    [Header("Performance")]
    public bool optimizeForPerformance = true;
    public float effectUpdateInterval = 0.5f;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    public bool showGizmos = true;
    
    private List<ParticleSystem> activeParticleSystems = new List<ParticleSystem>();
    private float lastUpdateTime = 0f;
    private bool isInitialized = false;
    
    void Start()
    {
        SetupAtmosphere();
    }
    
    void Update()
    {
        if (optimizeForPerformance && Time.time - lastUpdateTime > effectUpdateInterval)
        {
            OptimizePerformance();
            lastUpdateTime = Time.time;
        }
    }
    
    public void SetupAtmosphere()
    {
        if (isInitialized) return;
        
        if (enableAtmosphericEffects)
        {
            CreateSmokeSpawnPoints();
            CreateChimneySmoke();
            CreateAmbientEffects();
        }
        
        isInitialized = true;
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicAtmosphereManager: Atmosphere setup complete");
        }
    }
    
    private void CreateSmokeSpawnPoints()
    {
        // Create smoke spawn points if none exist
        if (smokeSpawnPoints.Count == 0)
        {
            // Define default chimney positions for village buildings
            Vector3[] chimneyPositions = {
                new Vector3(0, 5f, 16),     // TownHall chimney
                new Vector3(15, 4f, -11),   // Blacksmith chimney  
                new Vector3(-15, 4f, 1)     // Inn chimney
            };
            
            for (int i = 0; i < chimneyPositions.Length; i++)
            {
                GameObject spawnPoint = new GameObject($"Chimney_Smoke_Point_{i + 1}");
                spawnPoint.transform.position = chimneyPositions[i];
                spawnPoint.transform.SetParent(transform);
                TrySetAtmosphericTag(spawnPoint);
                
                smokeSpawnPoints.Add(spawnPoint.transform);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"BasicAtmosphereManager: Created smoke spawn point at {chimneyPositions[i]}");
                }
            }
        }
    }
    
    private void CreateChimneySmoke()
    {
        foreach (Transform spawnPoint in smokeSpawnPoints)
        {
            if (spawnPoint == null) continue;
            
            GameObject smokeEffect;
            
            // Use prefab if available, otherwise create basic particle system
            if (chimneySmokePrefab != null)
            {
                smokeEffect = Instantiate(chimneySmokePrefab, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                smokeEffect = CreateBasicSmokeEffect(spawnPoint.position);
            }
            
            smokeEffect.transform.SetParent(spawnPoint);
            TrySetAtmosphericTag(smokeEffect);
            
            ParticleSystem particles = smokeEffect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                activeParticleSystems.Add(particles);
                ConfigureSmokeParticles(particles);
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicAtmosphereManager: Created {smokeSpawnPoints.Count} chimney smoke effects");
        }
    }
    
    private GameObject CreateBasicSmokeEffect(Vector3 position)
    {
        GameObject smokeGO = new GameObject("Chimney_Smoke_Effect");
        smokeGO.transform.position = position;
        
        ParticleSystem particles = smokeGO.AddComponent<ParticleSystem>();
        return smokeGO;
    }
    
    private void ConfigureSmokeParticles(ParticleSystem particles)
    {
        if (particles == null) return;
        
        var main = particles.main;
        main.startLifetime = smokeLifetime;
        main.startSpeed = smokeVelocity.magnitude;
        main.startSize = 0.5f;
        main.startColor = new Color(0.8f, 0.8f, 0.8f, 0.3f); // Light gray, semi-transparent
        main.maxParticles = 15; // Limit per effect
        
        var emission = particles.emission;
        emission.rateOverTime = smokeEmissionRate;
        
        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;
        
        var velocityOverLifetime = particles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.x = smokeVelocity.x;
        velocityOverLifetime.y = smokeVelocity.y;
        velocityOverLifetime.z = smokeVelocity.z;
        
        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);
        sizeCurve.AddKey(1f, 1.5f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.gray, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(0.3f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = gradient;
        
        // Performance optimizations
        var renderer = particles.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.material = CreateSmokeMaterial();
        }
    }
    
    private Material CreateSmokeMaterial()
    {
        // Create a simple smoke material using Unity's default particle shader
        Material smokeMaterial = new Material(Shader.Find("Sprites/Default"));
        smokeMaterial.color = new Color(1f, 1f, 1f, 0.5f);
        
        // Make it slightly transparent
        smokeMaterial.SetFloat("_Mode", 2); // Fade mode
        smokeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        smokeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        smokeMaterial.SetInt("_ZWrite", 0);
        smokeMaterial.DisableKeyword("_ALPHATEST_ON");
        smokeMaterial.EnableKeyword("_ALPHABLEND_ON");
        smokeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        smokeMaterial.renderQueue = 3000;
        
        return smokeMaterial;
    }
    
    private void CreateAmbientEffects()
    {
        // Future expansion point for additional atmospheric effects
        // Could include: floating dust particles, ambient wind effects, etc.
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicAtmosphereManager: Ambient effects placeholder created");
        }
    }
    
    public void ToggleEffects(bool enabled)
    {
        enableAtmosphericEffects = enabled;
        
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null)
            {
                if (enabled)
                {
                    particles.Play();
                }
                else
                {
                    particles.Stop();
                }
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicAtmosphereManager: Atmospheric effects {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    public void OptimizePerformance()
    {
        int totalActiveParticles = 0;
        
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null)
            {
                totalActiveParticles += particles.particleCount;
            }
        }
        
        // If we exceed the particle limit, reduce emission rates
        if (totalActiveParticles > maxActiveParticles)
        {
            float reductionFactor = (float)maxActiveParticles / totalActiveParticles;
            
            foreach (ParticleSystem particles in activeParticleSystems)
            {
                if (particles != null)
                {
                    var emission = particles.emission;
                    emission.rateOverTime = emission.rateOverTime.constant * reductionFactor;
                }
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"BasicAtmosphereManager: Performance optimization applied. Particles: {totalActiveParticles} -> {maxActiveParticles}");
            }
        }
    }
    
    public void SetMaxActiveParticles(int maxParticles)
    {
        maxActiveParticles = Mathf.Clamp(maxParticles, 10, 200);
        OptimizePerformance();
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicAtmosphereManager: Max active particles set to {maxActiveParticles}");
        }
    }
    
    public void AddSmokeSpawnPoint(Vector3 position)
    {
        GameObject spawnPoint = new GameObject($"Custom_Smoke_Point_{smokeSpawnPoints.Count + 1}");
        spawnPoint.transform.position = position;
        spawnPoint.transform.SetParent(transform);
        TrySetAtmosphericTag(spawnPoint);
        
        smokeSpawnPoints.Add(spawnPoint.transform);
        
        // Create smoke effect at this point
        GameObject smokeEffect = CreateBasicSmokeEffect(position);
        smokeEffect.transform.SetParent(spawnPoint.transform);
        TrySetAtmosphericTag(smokeEffect);
        
        ParticleSystem particles = smokeEffect.GetComponent<ParticleSystem>();
        if (particles != null)
        {
            activeParticleSystems.Add(particles);
            ConfigureSmokeParticles(particles);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicAtmosphereManager: Added smoke spawn point at {position}");
        }
    }
    
    public void RemoveAllEffects()
    {
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null)
            {
                DestroyImmediate(particles.gameObject);
            }
        }
        
        activeParticleSystems.Clear();
        
        // Clear spawn points
        foreach (Transform spawnPoint in smokeSpawnPoints)
        {
            if (spawnPoint != null)
            {
                DestroyImmediate(spawnPoint.gameObject);
            }
        }
        
        smokeSpawnPoints.Clear();
        
        if (enableDebugLogs)
        {
            Debug.Log("BasicAtmosphereManager: All atmospheric effects removed");
        }
    }
    
    // Validation and info methods
    public string GetAtmosphereInfo()
    {
        string info = "=== ATMOSPHERE INFO ===\n";
        info += $"Effects Enabled: {enableAtmosphericEffects}\n";
        info += $"Smoke Spawn Points: {smokeSpawnPoints.Count}\n";
        info += $"Active Particle Systems: {activeParticleSystems.Count}\n";
        
        int totalParticles = 0;
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null)
            {
                totalParticles += particles.particleCount;
            }
        }
        
        info += $"Total Active Particles: {totalParticles} / {maxActiveParticles}\n";
        info += $"Performance Optimization: {optimizeForPerformance}\n";
        
        return info;
    }
    
    public bool ValidateAtmosphereSetup()
    {
        bool valid = true;
        
        if (enableAtmosphericEffects && smokeSpawnPoints.Count == 0)
        {
            Debug.LogWarning("BasicAtmosphereManager: No smoke spawn points configured");
        }
        
        int workingParticleSystems = 0;
        foreach (ParticleSystem particles in activeParticleSystems)
        {
            if (particles != null)
            {
                workingParticleSystems++;
            }
        }
        
        if (workingParticleSystems != activeParticleSystems.Count)
        {
            Debug.LogWarning($"BasicAtmosphereManager: {activeParticleSystems.Count - workingParticleSystems} particle systems are null");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"BasicAtmosphereManager: Atmosphere validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
    
    private void TrySetAtmosphericTag(GameObject obj)
    {
        try
        {
            obj.tag = "Atmospheric";
        }
        catch (UnityException)
        {
            // Tag doesn't exist, use default tag instead
            if (enableDebugLogs)
            {
                Debug.LogWarning($"BasicAtmosphereManager: 'Atmospheric' tag not found, using 'Untagged' for {obj.name}");
                Debug.LogWarning("To create the 'Atmospheric' tag: Go to Edit > Project Settings > Tags and Layers > Tags, click '+' and add 'Atmospheric'");
            }
            obj.tag = "Untagged";
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // Draw smoke spawn points
        Gizmos.color = Color.yellow;
        foreach (Transform spawnPoint in smokeSpawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                Gizmos.DrawRay(spawnPoint.position, Vector3.up * 2f);
            }
        }
    }
}