using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InteractionAudioSet
{
    [Header("General Interactions")]
    public AudioClip defaultInteraction;
    public AudioClip highlight;
    public AudioClip unhighlight;
    
    [Header("Building Interactions")]
    public AudioClip buildingEnter;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    
    [Header("Item Interactions")]
    public AudioClip itemPickup;
    public AudioClip itemExamine;
    public AudioClip containerOpen;
    
    [Header("Lore Interactions")]
    public AudioClip loreReveal;
    public AudioClip mysticalChime;
    public AudioClip ancientWhisper;
    
    [Header("UI Feedback")]
    public AudioClip panelOpen;
    public AudioClip panelClose;
    public AudioClip textReveal;
    
    public AudioClip GetClipForInteractionType(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Door:
                return doorOpen ?? buildingEnter ?? defaultInteraction;
            case InteractionType.Item:
                return itemExamine ?? itemPickup ?? defaultInteraction;
            case InteractionType.NPC:
                return defaultInteraction;
            case InteractionType.Combat:
                return defaultInteraction;
            case InteractionType.Dialogue:
                return defaultInteraction;
            default:
                return defaultInteraction;
        }
    }
}

public class InteractionAudioManager : MonoBehaviour
{
    public static InteractionAudioManager Instance { get; private set; }
    
    [Header("Audio Sets")]
    public InteractionAudioSet audioSet;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)] public float interactionVolume = 0.8f;
    [Range(0f, 1f)] public float uiVolume = 0.6f;
    [Range(0f, 1f)] public float ambientVolume = 0.4f;
    
    [Header("Spatial Audio")]
    public bool useSpatialAudio = true;
    public float maxDistance = 10f;
    public float minDistance = 1f;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private AudioSource interactionAudioSource;
    private AudioSource uiAudioSource;
    private Dictionary<InteractableObject, AudioSource> spatialSources;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Debug.LogWarning("InteractionAudioManager: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SubscribeToEvents();
        
        if (enableDebugLogs)
        {
            Debug.Log("InteractionAudioManager: Initialized and ready");
        }
    }
    
    void InitializeAudioSystem()
    {
        spatialSources = new Dictionary<InteractableObject, AudioSource>();
        
        CreateAudioSources();
        ValidateAudioSet();
    }
    
    void CreateAudioSources()
    {
        GameObject interactionSourceGO = new GameObject("InteractionAudioSource");
        interactionSourceGO.transform.SetParent(transform);
        interactionAudioSource = interactionSourceGO.AddComponent<AudioSource>();
        
        interactionAudioSource.playOnAwake = false;
        interactionAudioSource.spatialBlend = 0f;
        interactionAudioSource.volume = interactionVolume;
        
        GameObject uiSourceGO = new GameObject("UIAudioSource");
        uiSourceGO.transform.SetParent(transform);
        uiAudioSource = uiSourceGO.AddComponent<AudioSource>();
        
        uiAudioSource.playOnAwake = false;
        uiAudioSource.spatialBlend = 0f;
        uiAudioSource.volume = uiVolume;
    }
    
    void SubscribeToEvents()
    {
        EventBus.Subscribe<GameObjectInteractedEvent>(OnObjectInteracted);
        EventBus.Subscribe<InteractionPromptEvent>(OnInteractionPrompt);
        EventBus.Subscribe<PanelTransitionEvent>(OnPanelTransition);
    }
    
    void OnObjectInteracted(GameObjectInteractedEvent eventData)
    {
        InteractableObject interactable = eventData.InteractedObject?.GetComponent<InteractableObject>();
        if (interactable != null)
        {
            PlayInteractionAudio(interactable, eventData.Position);
        }
    }
    
    void OnInteractionPrompt(InteractionPromptEvent eventData)
    {
        switch (eventData.Action)
        {
            case InteractionAction.Show:
                PlayHighlightAudio();
                break;
            case InteractionAction.Hide:
                PlayUnhighlightAudio();
                break;
        }
    }
    
    void OnPanelTransition(PanelTransitionEvent eventData)
    {
        switch (eventData.Action)
        {
            case TransitionAction.Show:
                PlayPanelOpenAudio();
                break;
            case TransitionAction.Hide:
                PlayPanelCloseAudio();
                break;
        }
    }
    
    public void PlayInteractionAudio(InteractableObject interactable, Vector3 position)
    {
        if (interactable == null) return;
        
        AudioClip clipToPlay = GetInteractionClip(interactable);
        
        if (clipToPlay != null)
        {
            if (useSpatialAudio && AudioManager.Instance != null)
            {
                PlaySpatialAudio(clipToPlay, position, interactionVolume);
            }
            else
            {
                PlayNonSpatialAudio(clipToPlay, interactionVolume);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"InteractionAudioManager: Played {clipToPlay.name} for {interactable.DisplayName}");
            }
        }
    }
    
    AudioClip GetInteractionClip(InteractableObject interactable)
    {
        if (interactable.InteractionSound != null)
        {
            return interactable.InteractionSound;
        }
        
        if (audioSet != null)
        {
            AudioClip typeSpecificClip = audioSet.GetClipForInteractionType(interactable.InteractionType);
            if (typeSpecificClip != null)
            {
                return typeSpecificClip;
            }
        }
        
        return GetSpecialAudioForInteractable(interactable);
    }
    
    AudioClip GetSpecialAudioForInteractable(InteractableObject interactable)
    {
        if (audioSet == null) return null;
        
        string objectName = interactable.DisplayName.ToLower();
        
        if (objectName.Contains("ancient") || objectName.Contains("rune") || objectName.Contains("first kingdom"))
        {
            return audioSet.ancientWhisper ?? audioSet.mysticalChime;
        }
        
        if (objectName.Contains("well") || objectName.Contains("spring") || objectName.Contains("mystical"))
        {
            return audioSet.mysticalChime;
        }
        
        if (objectName.Contains("barrel") || objectName.Contains("crate") || objectName.Contains("container"))
        {
            return audioSet.containerOpen ?? audioSet.itemExamine;
        }
        
        if (objectName.Contains("statue") || objectName.Contains("memorial") || objectName.Contains("monument"))
        {
            return audioSet.loreReveal;
        }
        
        if (interactable is LoreInteractable)
        {
            return audioSet.loreReveal ?? audioSet.mysticalChime;
        }
        
        if (interactable is BuildingInteractable)
        {
            return audioSet.buildingEnter ?? audioSet.doorOpen;
        }
        
        return audioSet.defaultInteraction;
    }
    
    public void PlayHighlightAudio()
    {
        if (audioSet?.highlight != null)
        {
            PlayUIAudio(audioSet.highlight, uiVolume * 0.7f);
        }
    }
    
    public void PlayUnhighlightAudio()
    {
        if (audioSet?.unhighlight != null)
        {
            PlayUIAudio(audioSet.unhighlight, uiVolume * 0.5f);
        }
    }
    
    public void PlayPanelOpenAudio()
    {
        if (audioSet?.panelOpen != null)
        {
            PlayUIAudio(audioSet.panelOpen, uiVolume);
        }
    }
    
    public void PlayPanelCloseAudio()
    {
        if (audioSet?.panelClose != null)
        {
            PlayUIAudio(audioSet.panelClose, uiVolume);
        }
    }
    
    public void PlayTextRevealAudio()
    {
        if (audioSet?.textReveal != null)
        {
            PlayUIAudio(audioSet.textReveal, uiVolume * 0.8f);
        }
    }
    
    void PlaySpatialAudio(AudioClip clip, Vector3 position, float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXAtPosition(clip, position, volume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
    
    void PlayNonSpatialAudio(AudioClip clip, float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip, volume);
        }
        else if (interactionAudioSource != null)
        {
            interactionAudioSource.PlayOneShot(clip, volume);
        }
    }
    
    void PlayUIAudio(AudioClip clip, float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUI(clip, volume);
        }
        else if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(clip, volume);
        }
    }
    
    public void SetInteractionVolume(float volume)
    {
        interactionVolume = Mathf.Clamp01(volume);
        if (interactionAudioSource != null)
        {
            interactionAudioSource.volume = interactionVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionAudioManager: Interaction volume set to {interactionVolume:F2}");
        }
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        if (uiAudioSource != null)
        {
            uiAudioSource.volume = uiVolume;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionAudioManager: UI volume set to {uiVolume:F2}");
        }
    }
    
    public void EnableSpatialAudio(bool enabled)
    {
        useSpatialAudio = enabled;
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionAudioManager: Spatial audio {(enabled ? "enabled" : "disabled")}");
        }
    }
    
    void ValidateAudioSet()
    {
        if (audioSet == null)
        {
            Debug.LogWarning("InteractionAudioManager: No audio set assigned - interactions will be silent");
            return;
        }
        
        if (audioSet.defaultInteraction == null)
        {
            Debug.LogWarning("InteractionAudioManager: No default interaction audio assigned");
        }
        
        if (enableDebugLogs)
        {
            int assignedClips = 0;
            var fields = typeof(InteractionAudioSet).GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(AudioClip))
                {
                    AudioClip clip = (AudioClip)field.GetValue(audioSet);
                    if (clip != null) assignedClips++;
                }
            }
            
            Debug.Log($"InteractionAudioManager: {assignedClips} audio clips assigned in audio set");
        }
    }
    
    public string GetAudioInfo()
    {
        string info = "=== INTERACTION AUDIO INFO ===\n";
        info += $"Interaction Volume: {interactionVolume:F2}\n";
        info += $"UI Volume: {uiVolume:F2}\n";
        info += $"Spatial Audio: {useSpatialAudio}\n";
        info += $"Max Distance: {maxDistance}m\n";
        info += $"Audio Set Assigned: {audioSet != null}\n";
        
        if (audioSet != null)
        {
            info += $"Default Clip: {(audioSet.defaultInteraction != null ? audioSet.defaultInteraction.name : "None")}\n";
        }
        
        return info;
    }
    
    [ContextMenu("Test Default Interaction Audio")]
    public void TestDefaultInteractionAudioContext()
    {
        if (audioSet?.defaultInteraction != null)
        {
            PlayNonSpatialAudio(audioSet.defaultInteraction, interactionVolume);
            Debug.Log("InteractionAudioManager: Played test default interaction audio");
        }
        else
        {
            Debug.LogWarning("InteractionAudioManager: No default interaction audio to test");
        }
    }
    
    [ContextMenu("Test UI Audio")]
    public void TestUIAudioContext()
    {
        PlayHighlightAudio();
        Debug.Log("InteractionAudioManager: Played test UI audio");
    }
    
    [ContextMenu("Validate Audio Setup")]
    public void ValidateAudioSetupContext()
    {
        ValidateAudioSet();
        Debug.Log(GetAudioInfo());
    }
    
    void OnDestroy()
    {
        EventBus.Unsubscribe<GameObjectInteractedEvent>(OnObjectInteracted);
        EventBus.Unsubscribe<InteractionPromptEvent>(OnInteractionPrompt);
        EventBus.Unsubscribe<PanelTransitionEvent>(OnPanelTransition);
    }
}