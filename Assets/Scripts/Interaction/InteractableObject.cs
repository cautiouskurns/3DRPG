using UnityEngine;

[System.Serializable]
public class InteractionContent
{
    [Header("Display")]
    public string title;
    [TextArea(3, 8)]
    public string description;
    
    [Header("Lore")]
    [TextArea(2, 6)]
    public string loreText;
    public string category = "General";
    
    [Header("Audio")]
    public AudioClip interactionSound;
    
    [Header("Behavior")]
    public bool showContentPanel = true;
    public float contentDisplayTime = 5f;
    public bool canRepeatInteraction = true;
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description);
    }
}

public abstract class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string displayName = "Interactable";
    public InteractionType interactionType = InteractionType.General;
    public string promptText = "Press E to interact";
    
    [Header("Content")]
    public InteractionContent content;
    
    [Header("Visual Feedback")]
    public bool enableHighlighting = true;
    public Material highlightMaterial;
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;
    
    [Header("Audio")]
    public AudioClip interactionSound;
    
    [Header("State")]
    public bool isInteractable = true;
    public bool hasBeenInteracted = false;
    public int timesInteracted = 0;
    
    protected Renderer objectRenderer;
    protected Material originalMaterial;
    protected Material currentHighlightMaterial;
    protected bool isHighlighted = false;
    
    public string DisplayName => displayName;
    public InteractionType InteractionType => interactionType;
    public AudioClip InteractionSound => interactionSound ?? content?.interactionSound;
    public bool HasValidContent => content != null && content.IsValid();
    
    protected virtual void Awake()
    {
        InitializeRenderer();
        SetupHighlightMaterial();
        ValidateSetup();
    }
    
    protected virtual void Start()
    {
        RegisterWithInteractionSystem();
    }
    
    protected virtual void OnDestroy()
    {
        UnregisterWithInteractionSystem();
    }
    
    void InitializeRenderer()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
        
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        else
        {
            Debug.LogWarning($"InteractableObject: No Renderer found on {gameObject.name}");
        }
    }
    
    void SetupHighlightMaterial()
    {
        if (!enableHighlighting || objectRenderer == null) return;
        
        if (highlightMaterial != null)
        {
            currentHighlightMaterial = highlightMaterial;
        }
        else if (originalMaterial != null)
        {
            currentHighlightMaterial = new Material(originalMaterial);
            
            if (currentHighlightMaterial.HasProperty("_Color"))
            {
                currentHighlightMaterial.color = highlightColor;
            }
            
            if (currentHighlightMaterial.HasProperty("_EmissionColor"))
            {
                currentHighlightMaterial.EnableKeyword("_EMISSION");
                currentHighlightMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
            }
        }
    }
    
    void RegisterWithInteractionSystem()
    {
        if (InteractionSystem.Instance != null)
        {
            InteractionSystem.Instance.RegisterInteractable(this);
        }
    }
    
    void UnregisterWithInteractionSystem()
    {
        if (InteractionSystem.Instance != null)
        {
            InteractionSystem.Instance.UnregisterInteractable(this);
        }
    }
    
    public virtual bool CanInteract()
    {
        if (!isInteractable || !gameObject.activeInHierarchy)
        {
            return false;
        }
        
        if (!content.canRepeatInteraction && hasBeenInteracted)
        {
            return false;
        }
        
        return true;
    }
    
    public virtual void Interact()
    {
        if (!CanInteract())
        {
            Debug.LogWarning($"InteractableObject: Cannot interact with {displayName}");
            return;
        }
        
        hasBeenInteracted = true;
        timesInteracted++;
        
        OnInteracted();
        
        if (HasValidContent && content.showContentPanel)
        {
            ShowContentPanel();
        }
        
        EventBus.Publish(new InteractionPromptEvent(
            InteractionAction.Update,
            GetPromptText(),
            content?.contentDisplayTime ?? 3f,
            interactionType
        ));
    }
    
    protected virtual void OnInteracted()
    {
        // Override in derived classes for specific behavior
    }
    
    public virtual void SetHighlighted(bool highlighted)
    {
        if (!enableHighlighting || objectRenderer == null || isHighlighted == highlighted)
        {
            return;
        }
        
        isHighlighted = highlighted;
        
        if (highlighted && currentHighlightMaterial != null)
        {
            objectRenderer.material = currentHighlightMaterial;
        }
        else if (!highlighted && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
    
    public virtual string GetPromptText()
    {
        if (!CanInteract())
        {
            return "";
        }
        
        if (!string.IsNullOrEmpty(promptText))
        {
            return promptText;
        }
        
        return $"Press E to examine {displayName}";
    }
    
    protected virtual void ShowContentPanel()
    {
        if (!HasValidContent) return;
        
        if (InteractionContentPanel.Instance != null)
        {
            InteractionContentPanel.Instance.ShowContent(content);
        }
        else
        {
            Debug.Log($"=== {content.title} ===");
            Debug.Log(content.description);
            if (!string.IsNullOrEmpty(content.loreText))
            {
                Debug.Log($"Lore: {content.loreText}");
            }
        }
    }
    
    void ValidateSetup()
    {
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = gameObject.name;
        }
        
        if (content == null)
        {
            Debug.LogWarning($"InteractableObject: No content assigned to {displayName}");
        }
        
        if (objectRenderer == null)
        {
            Debug.LogWarning($"InteractableObject: No renderer found for highlighting on {displayName}");
            enableHighlighting = false;
        }
    }
    
    [ContextMenu("Test Interaction")]
    public void TestInteractionContext()
    {
        Interact();
    }
    
    [ContextMenu("Toggle Highlight")]
    public void ToggleHighlightContext()
    {
        SetHighlighted(!isHighlighted);
    }
    
    [ContextMenu("Reset Interaction State")]
    public void ResetInteractionStateContext()
    {
        hasBeenInteracted = false;
        timesInteracted = 0;
        isInteractable = true;
        SetHighlighted(false);
    }
}

public class BuildingInteractable : InteractableObject
{
    [Header("Building Specific")]
    public bool hasInterior = false;
    public string interiorSceneName;
    
    protected override void OnInteracted()
    {
        base.OnInteracted();
        
        if (hasInterior && !string.IsNullOrEmpty(interiorSceneName))
        {
            // Future: Handle interior transition
            Debug.Log($"BuildingInteractable: Would transition to {interiorSceneName}");
        }
    }
}

public class PropInteractable : InteractableObject
{
    [Header("Prop Specific")]
    public bool consumeOnUse = false;
    public float respawnTime = 0f;
    
    protected override void OnInteracted()
    {
        base.OnInteracted();
        
        if (consumeOnUse)
        {
            isInteractable = false;
            
            if (respawnTime > 0f)
            {
                Invoke(nameof(Respawn), respawnTime);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    
    void Respawn()
    {
        isInteractable = true;
        hasBeenInteracted = false;
        SetHighlighted(false);
        gameObject.SetActive(true);
    }
}

public class LoreInteractable : InteractableObject
{
    [Header("Lore Specific")]
    public bool unlockJournalEntry = false;
    public string journalEntryId;
    
    protected override void OnInteracted()
    {
        base.OnInteracted();
        
        if (unlockJournalEntry && !string.IsNullOrEmpty(journalEntryId))
        {
            // Future: Unlock journal entry
            Debug.Log($"LoreInteractable: Would unlock journal entry {journalEntryId}");
        }
    }
}