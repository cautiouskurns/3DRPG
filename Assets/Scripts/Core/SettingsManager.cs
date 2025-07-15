using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    [Header("Audio Settings")]
    [Range(0f, 100f)] public float masterVolume = 100f;
    [Range(0f, 100f)] public float sfxVolume = 100f;
    [Range(0f, 100f)] public float musicVolume = 70f;
    [Range(0f, 100f)] public float ambientVolume = 60f;
    
    [Header("Graphics Settings")]
    public bool isFullscreen = true;
    public bool vSyncEnabled = true;
    public int targetFrameRate = 60;
    public int qualityLevel = 2; // Unity quality levels: 0-5
    
    [Header("Resolution Settings")]
    public Vector2Int currentResolution = new Vector2Int(1920, 1080);
    public int refreshRate = 60;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    public bool autoSave = true;
    public float autoSaveInterval = 30f;
    
    // Private settings data
    private Resolution[] availableResolutions;
    private List<Vector2Int> supportedResolutions = new List<Vector2Int>();
    private bool isInitialized = false;
    private float lastAutoSave = 0f;
    
    // Settings keys for PlayerPrefs
    private const string MASTER_VOLUME_KEY = "Settings_MasterVolume";
    private const string SFX_VOLUME_KEY = "Settings_SFXVolume";
    private const string MUSIC_VOLUME_KEY = "Settings_MusicVolume";
    private const string AMBIENT_VOLUME_KEY = "Settings_AmbientVolume";
    private const string FULLSCREEN_KEY = "Settings_Fullscreen";
    private const string VSYNC_KEY = "Settings_VSync";
    private const string RESOLUTION_WIDTH_KEY = "Settings_ResolutionWidth";
    private const string RESOLUTION_HEIGHT_KEY = "Settings_ResolutionHeight";
    private const string REFRESH_RATE_KEY = "Settings_RefreshRate";
    private const string QUALITY_LEVEL_KEY = "Settings_QualityLevel";
    private const string TARGET_FRAMERATE_KEY = "Settings_TargetFrameRate";
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("SettingsManager: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("SettingsManager: Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeSettings();
    }
    
    void Update()
    {
        // Auto-save settings periodically
        if (autoSave && Time.time - lastAutoSave > autoSaveInterval)
        {
            SaveSettings();
            lastAutoSave = Time.time;
        }
    }
    
    private void InitializeSettings()
    {
        if (isInitialized) return;
        
        // Get available resolutions
        GetAvailableResolutions();
        
        // Load settings from PlayerPrefs
        LoadSettings();
        
        // Apply loaded settings
        ApplySettings();
        
        isInitialized = true;
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsManager: Settings system initialized");
            LogCurrentSettings();
        }
    }
    
    private void GetAvailableResolutions()
    {
        availableResolutions = Screen.resolutions;
        supportedResolutions.Clear();
        
        // Filter and deduplicate resolutions
        HashSet<Vector2Int> uniqueResolutions = new HashSet<Vector2Int>();
        
        foreach (Resolution res in availableResolutions)
        {
            Vector2Int resVector = new Vector2Int(res.width, res.height);
            if (res.width >= 1024 && res.height >= 768) // Minimum supported resolution
            {
                uniqueResolutions.Add(resVector);
            }
        }
        
        supportedResolutions = uniqueResolutions.OrderBy(r => r.x).ThenBy(r => r.y).ToList();
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Found {supportedResolutions.Count} supported resolutions");
        }
    }
    
    // Audio Settings Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp(volume, 0f, 100f);
        ApplyAudioSettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.MasterVolume, masterVolume));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Master volume set to {masterVolume}%");
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 100f);
        ApplyAudioSettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.SFXVolume, sfxVolume));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: SFX volume set to {sfxVolume}%");
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0f, 100f);
        ApplyAudioSettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.MusicVolume, musicVolume));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Music volume set to {musicVolume}%");
        }
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp(volume, 0f, 100f);
        ApplyAudioSettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.AmbientVolume, ambientVolume));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Ambient volume set to {ambientVolume}%");
        }
    }
    
    // Graphics Settings Methods
    public void SetFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
        ApplyDisplaySettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.Fullscreen, isFullscreen));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Fullscreen set to {isFullscreen}");
        }
    }
    
    public void SetVSync(bool enabled)
    {
        vSyncEnabled = enabled;
        ApplyDisplaySettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.VSync, vSyncEnabled));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: VSync set to {vSyncEnabled}");
        }
    }
    
    public void SetResolution(int width, int height, int refreshRate = 60)
    {
        Vector2Int newResolution = new Vector2Int(width, height);
        
        if (supportedResolutions.Contains(newResolution))
        {
            currentResolution = newResolution;
            this.refreshRate = refreshRate;
            ApplyDisplaySettings();
            
            EventBus.Publish(new SettingsChangedEvent(SettingsType.Resolution, $"{width}x{height}@{refreshRate}"));
            
            if (enableDebugLogs)
            {
                Debug.Log($"SettingsManager: Resolution set to {width}x{height}@{refreshRate}Hz");
            }
        }
        else
        {
            Debug.LogWarning($"SettingsManager: Unsupported resolution {width}x{height}");
        }
    }
    
    public void SetResolution(Vector2Int resolution, int refreshRate = 60)
    {
        SetResolution(resolution.x, resolution.y, refreshRate);
    }
    
    public void SetQualityLevel(int level)
    {
        qualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        ApplyQualitySettings();
        
        EventBus.Publish(new SettingsChangedEvent(SettingsType.GraphicsQuality, qualityLevel));
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Quality level set to {qualityLevel} ({QualitySettings.names[qualityLevel]})");
        }
    }
    
    public void SetTargetFrameRate(int frameRate)
    {
        targetFrameRate = Mathf.Clamp(frameRate, 30, 144);
        ApplyPerformanceSettings();
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Target frame rate set to {targetFrameRate} FPS");
        }
    }
    
    // Settings Application Methods
    private void ApplySettings()
    {
        ApplyAudioSettings();
        ApplyDisplaySettings();
        ApplyQualitySettings();
        ApplyPerformanceSettings();
    }
    
    private void ApplyAudioSettings()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            // Convert percentage to 0-1 range
            audioManager.SetMasterVolume(masterVolume / 100f);
            audioManager.SetSFXVolume(sfxVolume / 100f);
            audioManager.SetMusicVolume(musicVolume / 100f);
            audioManager.SetAmbientVolume(ambientVolume / 100f);
        }
    }
    
    private void ApplyDisplaySettings()
    {
        // Apply resolution and fullscreen
        FullScreenMode fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        RefreshRate refreshRateStruct = new RefreshRate() { numerator = (uint)refreshRate, denominator = 1 };
        Screen.SetResolution(currentResolution.x, currentResolution.y, fullScreenMode, refreshRateStruct);
        
        // Apply VSync
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
    }
    
    private void ApplyQualitySettings()
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }
    
    private void ApplyPerformanceSettings()
    {
        Application.targetFrameRate = targetFrameRate;
    }
    
    // Settings Persistence Methods
    public void SaveSettings()
    {
        try
        {
            // Audio settings
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.SetFloat(AMBIENT_VOLUME_KEY, ambientVolume);
            
            // Graphics settings
            PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
            PlayerPrefs.SetInt(VSYNC_KEY, vSyncEnabled ? 1 : 0);
            PlayerPrefs.SetInt(RESOLUTION_WIDTH_KEY, currentResolution.x);
            PlayerPrefs.SetInt(RESOLUTION_HEIGHT_KEY, currentResolution.y);
            PlayerPrefs.SetInt(REFRESH_RATE_KEY, refreshRate);
            PlayerPrefs.SetInt(QUALITY_LEVEL_KEY, qualityLevel);
            PlayerPrefs.SetInt(TARGET_FRAMERATE_KEY, targetFrameRate);
            
            PlayerPrefs.Save();
            
            if (enableDebugLogs)
            {
                Debug.Log("SettingsManager: Settings saved successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SettingsManager: Failed to save settings - {e.Message}");
        }
    }
    
    private void LoadSettings()
    {
        try
        {
            // Load audio settings with fallbacks
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, masterVolume);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, sfxVolume);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume);
            ambientVolume = PlayerPrefs.GetFloat(AMBIENT_VOLUME_KEY, ambientVolume);
            
            // Load graphics settings with fallbacks
            isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0) == 1;
            vSyncEnabled = PlayerPrefs.GetInt(VSYNC_KEY, vSyncEnabled ? 1 : 0) == 1;
            qualityLevel = PlayerPrefs.GetInt(QUALITY_LEVEL_KEY, qualityLevel);
            targetFrameRate = PlayerPrefs.GetInt(TARGET_FRAMERATE_KEY, targetFrameRate);
            
            // Load resolution settings
            int savedWidth = PlayerPrefs.GetInt(RESOLUTION_WIDTH_KEY, currentResolution.x);
            int savedHeight = PlayerPrefs.GetInt(RESOLUTION_HEIGHT_KEY, currentResolution.y);
            int savedRefreshRate = PlayerPrefs.GetInt(REFRESH_RATE_KEY, refreshRate);
            
            Vector2Int savedResolution = new Vector2Int(savedWidth, savedHeight);
            if (supportedResolutions.Contains(savedResolution))
            {
                currentResolution = savedResolution;
                refreshRate = savedRefreshRate;
            }
            else
            {
                // Fallback to current screen resolution
                currentResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
                refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator);
            }
            
            // Validate loaded settings
            ValidateSettings();
            
            if (enableDebugLogs)
            {
                Debug.Log("SettingsManager: Settings loaded successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SettingsManager: Failed to load settings - {e.Message}");
            ResetToDefaults();
        }
    }
    
    private void ValidateSettings()
    {
        // Clamp audio settings
        masterVolume = Mathf.Clamp(masterVolume, 0f, 100f);
        sfxVolume = Mathf.Clamp(sfxVolume, 0f, 100f);
        musicVolume = Mathf.Clamp(musicVolume, 0f, 100f);
        ambientVolume = Mathf.Clamp(ambientVolume, 0f, 100f);
        
        // Validate quality level
        qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.names.Length - 1);
        
        // Validate frame rate
        targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 144);
    }
    
    public void ResetToDefaults()
    {
        // Reset audio settings
        masterVolume = 100f;
        sfxVolume = 100f;
        musicVolume = 70f;
        ambientVolume = 60f;
        
        // Reset graphics settings
        isFullscreen = true;
        vSyncEnabled = true;
        qualityLevel = 2;
        targetFrameRate = 60;
        
        // Reset resolution to current screen
        currentResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator);
        
        ApplySettings();
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsManager: Settings reset to defaults");
        }
    }
    
    // Public Getters
    public List<Vector2Int> GetSupportedResolutions()
    {
        return new List<Vector2Int>(supportedResolutions);
    }
    
    public string[] GetQualityLevels()
    {
        return QualitySettings.names;
    }
    
    public bool IsResolutionSupported(Vector2Int resolution)
    {
        return supportedResolutions.Contains(resolution);
    }
    
    // Utility methods
    private void LogCurrentSettings()
    {
        Debug.Log("=== CURRENT SETTINGS ===");
        Debug.Log($"Audio - Master: {masterVolume}%, SFX: {sfxVolume}%, Music: {musicVolume}%, Ambient: {ambientVolume}%");
        Debug.Log($"Graphics - Resolution: {currentResolution.x}x{currentResolution.y}@{refreshRate}Hz, Fullscreen: {isFullscreen}, VSync: {vSyncEnabled}");
        Debug.Log($"Quality - Level: {qualityLevel} ({QualitySettings.names[qualityLevel]}), Target FPS: {targetFrameRate}");
        Debug.Log("========================");
    }
    
    public bool ValidateSettingsSetup()
    {
        bool valid = true;
        
        if (!isInitialized)
        {
            Debug.LogWarning("SettingsManager: Not initialized");
            valid = false;
        }
        
        if (supportedResolutions.Count == 0)
        {
            Debug.LogError("SettingsManager: No supported resolutions found");
            valid = false;
        }
        
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogWarning("SettingsManager: AudioManager not found for audio settings integration");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsManager: Validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
    
    // Event handling for external settings changes
    private void OnEnable()
    {
        EventBus.Subscribe<SettingsChangedEvent>(OnSettingsChanged);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<SettingsChangedEvent>(OnSettingsChanged);
    }
    
    private void OnSettingsChanged(SettingsChangedEvent evt)
    {
        // Handle external settings changes if needed
        if (autoSave)
        {
            SaveSettings();
        }
    }
}