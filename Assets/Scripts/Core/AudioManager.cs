using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    public AudioSource uiSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1.0f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    [Range(0f, 1f)] public float ambientVolume = 0.6f;
    [Range(0f, 1f)] public float uiVolume = 1.0f;
    
    [Header("Audio Settings")]
    public bool muteOnFocusLoss = true;
    public bool enableSpatialAudio = true;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private float previousMasterVolume;
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
                Debug.Log("AudioManager: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeAudioSystem();
    }
    
    void Update()
    {
        // Monitor volume changes
        if (Mathf.Abs(masterVolume - previousMasterVolume) > 0.01f)
        {
            UpdateAllVolumes();
            previousMasterVolume = masterVolume;
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (muteOnFocusLoss)
        {
            if (hasFocus)
            {
                SetMasterVolume(masterVolume);
            }
            else
            {
                SetMasterVolume(0f);
            }
        }
    }
    
    private void InitializeAudioSystem()
    {
        if (isInitialized) return;
        
        // Create audio sources if they don't exist
        SetupAudioSources();
        
        // Apply initial volume settings
        UpdateAllVolumes();
        
        // Configure audio settings
        ConfigureAudioSettings();
        
        isInitialized = true;
        previousMasterVolume = masterVolume;
        
        if (enableDebugLogs)
        {
            Debug.Log("AudioManager: Audio system initialized");
        }
    }
    
    private void SetupAudioSources()
    {
        // Music Source
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("Music Source");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume * masterVolume;
        }
        
        // SFX Source
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFX Source");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume * masterVolume;
        }
        
        // Ambient Source
        if (ambientSource == null)
        {
            GameObject ambientGO = new GameObject("Ambient Source");
            ambientGO.transform.SetParent(transform);
            ambientSource = ambientGO.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
            ambientSource.volume = ambientVolume * masterVolume;
        }
        
        // UI Source
        if (uiSource == null)
        {
            GameObject uiGO = new GameObject("UI Source");
            uiGO.transform.SetParent(transform);
            uiSource = uiGO.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.playOnAwake = false;
            uiSource.volume = uiVolume * masterVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("AudioManager: Audio sources setup complete");
        }
    }
    
    private void ConfigureAudioSettings()
    {
        // Configure spatial audio settings
        if (enableSpatialAudio)
        {
            // SFX and Ambient sources can be spatial
            if (sfxSource != null)
            {
                sfxSource.spatialBlend = 1.0f; // 3D sound
                sfxSource.rolloffMode = AudioRolloffMode.Logarithmic;
                sfxSource.maxDistance = 20f;
            }
            
            if (ambientSource != null)
            {
                ambientSource.spatialBlend = 0.5f; // Semi-spatial for ambient
                ambientSource.rolloffMode = AudioRolloffMode.Linear;
                ambientSource.maxDistance = 50f;
            }
        }
        
        // Music and UI are always 2D
        if (musicSource != null)
        {
            musicSource.spatialBlend = 0f;
        }
        
        if (uiSource != null)
        {
            uiSource.spatialBlend = 0f;
        }
    }
    
    // Foundation methods for future implementation (hooks only)
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: SFX hook called for {clip?.name ?? "null"} at volume {volume:F2}");
        }
        
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
    }
    
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Spatial SFX hook called for {clip?.name ?? "null"} at {position}");
        }
        
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume * masterVolume);
        }
    }
    
    public void PlayMusic(AudioClip clip, bool loop = true, float fadeTime = 1.0f)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Music hook called for {clip?.name ?? "null"}, loop: {loop}");
        }
        
        if (clip != null && musicSource != null)
        {
            // Simple implementation - future versions can add crossfading
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }
    
    public void PlayAmbient(AudioClip clip, bool loop = true)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Ambient hook called for {clip?.name ?? "null"}, loop: {loop}");
        }
        
        if (clip != null && ambientSource != null)
        {
            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.Play();
        }
    }
    
    public void PlayUI(AudioClip clip, float volume = 1.0f)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: UI hook called for {clip?.name ?? "null"}");
        }
        
        if (clip != null && uiSource != null)
        {
            uiSource.PlayOneShot(clip, volume * uiVolume * masterVolume);
        }
    }
    
    // Volume control methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Master volume set to {masterVolume:F2}");
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Music volume set to {musicVolume:F2}");
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: SFX volume set to {sfxVolume:F2}");
        }
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume * masterVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Ambient volume set to {ambientVolume:F2}");
        }
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        if (uiSource != null)
        {
            uiSource.volume = uiVolume * masterVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: UI volume set to {uiVolume:F2}");
        }
    }
    
    private void UpdateAllVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
        
        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume * masterVolume;
        }
        
        if (uiSource != null)
        {
            uiSource.volume = uiVolume * masterVolume;
        }
    }
    
    // Audio control methods
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            
            if (enableDebugLogs)
            {
                Debug.Log("AudioManager: Music stopped");
            }
        }
    }
    
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
            
            if (enableDebugLogs)
            {
                Debug.Log("AudioManager: Music paused");
            }
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
            
            if (enableDebugLogs)
            {
                Debug.Log("AudioManager: Music resumed");
            }
        }
    }
    
    public void StopAmbient()
    {
        if (ambientSource != null && ambientSource.isPlaying)
        {
            ambientSource.Stop();
            
            if (enableDebugLogs)
            {
                Debug.Log("AudioManager: Ambient stopped");
            }
        }
    }
    
    public void StopAllAudio()
    {
        StopMusic();
        StopAmbient();
        
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
        
        if (uiSource != null)
        {
            uiSource.Stop();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("AudioManager: All audio stopped");
        }
    }
    
    // Status and validation methods
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }
    
    public bool IsAmbientPlaying()
    {
        return ambientSource != null && ambientSource.isPlaying;
    }
    
    public string GetAudioInfo()
    {
        string info = "=== AUDIO INFO ===\n";
        info += $"Master Volume: {masterVolume:F2}\n";
        info += $"Music Volume: {musicVolume:F2} (Playing: {IsMusicPlaying()})\n";
        info += $"SFX Volume: {sfxVolume:F2}\n";
        info += $"Ambient Volume: {ambientVolume:F2} (Playing: {IsAmbientPlaying()})\n";
        info += $"UI Volume: {uiVolume:F2}\n";
        info += $"Spatial Audio: {enableSpatialAudio}\n";
        info += $"Mute on Focus Loss: {muteOnFocusLoss}\n";
        
        return info;
    }
    
    public bool ValidateAudioSetup()
    {
        bool valid = true;
        
        if (musicSource == null)
        {
            Debug.LogError("AudioManager: Music source is null!");
            valid = false;
        }
        
        if (sfxSource == null)
        {
            Debug.LogError("AudioManager: SFX source is null!");
            valid = false;
        }
        
        if (ambientSource == null)
        {
            Debug.LogError("AudioManager: Ambient source is null!");
            valid = false;
        }
        
        if (uiSource == null)
        {
            Debug.LogError("AudioManager: UI source is null!");
            valid = false;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"AudioManager: Audio validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
}