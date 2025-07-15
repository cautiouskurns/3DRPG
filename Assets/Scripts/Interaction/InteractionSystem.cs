using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }
    
    [Header("Interaction Settings")]
    public float interactionRadius = 1.5f;
    public LayerMask interactableLayerMask = -1;
    public bool enableDebugLogs = true;
    public bool enableDebugVisuals = false;
    
    [Header("Player Reference")]
    public Transform playerTransform;
    
    [Header("Highlighting")]
    public Material highlightMaterial;
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;
    
    private List<InteractableObject> allInteractables = new List<InteractableObject>();
    private InteractableObject currentNearestInteractable;
    private InteractableObject highlightedInteractable;
    
    private PlayerController playerController;
    private bool isInteractionUIVisible = false;
    
    public InteractableObject CurrentInteractable => currentNearestInteractable;
    public bool HasInteractableInRange => currentNearestInteractable != null;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("InteractionSystem: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("InteractionSystem: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeInteractionSystem();
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            UpdateNearestInteractable();
            HandleInteractionInput();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (enableDebugVisuals && playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerTransform.position, interactionRadius);
            
            if (currentNearestInteractable != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(playerTransform.position, currentNearestInteractable.transform.position);
            }
        }
    }
    
    void InitializeInteractionSystem()
    {
        FindPlayerTransform();
        FindPlayerController();
        RegisterExistingInteractables();
        SubscribeToInputEvents();
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionSystem: Initialized with {allInteractables.Count} interactables");
        }
    }
    
    void FindPlayerTransform()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                if (enableDebugLogs)
                {
                    Debug.Log("InteractionSystem: Player transform found via tag");
                }
            }
            else
            {
                PlayerController controller = FindFirstObjectByType<PlayerController>();
                if (controller != null)
                {
                    playerTransform = controller.transform;
                    if (enableDebugLogs)
                    {
                        Debug.Log("InteractionSystem: Player transform found via PlayerController");
                    }
                }
            }
        }
        
        if (playerTransform == null)
        {
            Debug.LogWarning("InteractionSystem: Player transform not found! Interaction system will not function.");
        }
    }
    
    void FindPlayerController()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null && enableDebugLogs)
            {
                Debug.Log("InteractionSystem: PlayerController found for integration");
            }
        }
    }
    
    void RegisterExistingInteractables()
    {
        InteractableObject[] existing = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
        foreach (InteractableObject interactable in existing)
        {
            RegisterInteractable(interactable);
        }
    }
    
    void SubscribeToInputEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractInput += HandleInteractInput;
            if (enableDebugLogs)
            {
                Debug.Log("InteractionSystem: Subscribed to InputManager events");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("InteractionSystem: InputManager not found, using fallback input");
        }
    }
    
    void UpdateNearestInteractable()
    {
        InteractableObject nearest = FindNearestInteractable();
        
        if (nearest != currentNearestInteractable)
        {
            if (currentNearestInteractable != null)
            {
                OnInteractableExited(currentNearestInteractable);
            }
            
            currentNearestInteractable = nearest;
            
            if (currentNearestInteractable != null)
            {
                OnInteractableEntered(currentNearestInteractable);
            }
        }
    }
    
    InteractableObject FindNearestInteractable()
    {
        if (playerTransform == null) return null;
        
        InteractableObject nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (InteractableObject interactable in allInteractables)
        {
            if (interactable == null || !interactable.gameObject.activeInHierarchy || !interactable.CanInteract())
            {
                continue;
            }
            
            float distance = Vector3.Distance(playerTransform.position, interactable.transform.position);
            
            if (distance <= interactionRadius && distance < nearestDistance)
            {
                nearest = interactable;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
    
    void OnInteractableEntered(InteractableObject interactable)
    {
        SetHighlightedInteractable(interactable);
        ShowInteractionPrompt(interactable);
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionSystem: Entered range of {interactable.DisplayName}");
        }
        
        EventBus.Publish(new InteractionPromptEvent(
            InteractionAction.Show, 
            interactable.GetPromptText(), 
            0f, 
            interactable.InteractionType
        ));
    }
    
    void OnInteractableExited(InteractableObject interactable)
    {
        if (highlightedInteractable == interactable)
        {
            SetHighlightedInteractable(null);
        }
        
        HideInteractionPrompt();
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionSystem: Exited range of {interactable.DisplayName}");
        }
        
        EventBus.Publish(new InteractionPromptEvent(InteractionAction.Hide));
    }
    
    void SetHighlightedInteractable(InteractableObject interactable)
    {
        if (highlightedInteractable != null)
        {
            highlightedInteractable.SetHighlighted(false);
        }
        
        highlightedInteractable = interactable;
        
        if (highlightedInteractable != null)
        {
            highlightedInteractable.SetHighlighted(true);
        }
    }
    
    void ShowInteractionPrompt(InteractableObject interactable)
    {
        if (!isInteractionUIVisible)
        {
            isInteractionUIVisible = true;
            
            if (StaticUIManager.Instance != null)
            {
                // Future: Show interaction prompt UI
            }
        }
    }
    
    void HideInteractionPrompt()
    {
        if (isInteractionUIVisible)
        {
            isInteractionUIVisible = false;
            
            if (StaticUIManager.Instance != null)
            {
                // Future: Hide interaction prompt UI
            }
        }
    }
    
    void HandleInteractionInput()
    {
        if (InputManager.Instance == null)
        {
            if (Input.GetKeyDown(KeyCode.E) && currentNearestInteractable != null)
            {
                TriggerInteraction(currentNearestInteractable);
            }
        }
    }
    
    void HandleInteractInput(bool pressed)
    {
        if (pressed && currentNearestInteractable != null)
        {
            TriggerInteraction(currentNearestInteractable);
        }
    }
    
    public void TriggerInteraction(InteractableObject interactable)
    {
        if (interactable == null || !interactable.CanInteract())
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"InteractionSystem: Cannot interact with {interactable?.DisplayName ?? "null"}");
            }
            return;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionSystem: Triggering interaction with {interactable.DisplayName}");
        }
        
        interactable.Interact();
        
        if (AudioManager.Instance != null && interactable.InteractionSound != null)
        {
            AudioManager.Instance.PlaySFX(interactable.InteractionSound);
        }
        
        EventBus.Publish(new GameObjectInteractedEvent(interactable.gameObject, interactable.DisplayName));
    }
    
    public void RegisterInteractable(InteractableObject interactable)
    {
        if (interactable != null && !allInteractables.Contains(interactable))
        {
            allInteractables.Add(interactable);
            
            if (enableDebugLogs)
            {
                Debug.Log($"InteractionSystem: Registered {interactable.DisplayName}");
            }
        }
    }
    
    public void UnregisterInteractable(InteractableObject interactable)
    {
        if (interactable != null && allInteractables.Contains(interactable))
        {
            allInteractables.Remove(interactable);
            
            if (currentNearestInteractable == interactable)
            {
                OnInteractableExited(interactable);
                currentNearestInteractable = null;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"InteractionSystem: Unregistered {interactable.DisplayName}");
            }
        }
    }
    
    public List<InteractableObject> GetAllInteractables()
    {
        return allInteractables.Where(i => i != null).ToList();
    }
    
    public List<InteractableObject> GetInteractablesInRange(Vector3 position, float radius)
    {
        return allInteractables.Where(i => 
            i != null && 
            i.gameObject.activeInHierarchy && 
            Vector3.Distance(position, i.transform.position) <= radius
        ).ToList();
    }
    
    public bool ValidateInteractionSetup()
    {
        bool valid = true;
        
        if (playerTransform == null)
        {
            Debug.LogError("InteractionSystem: Player transform not assigned");
            valid = false;
        }
        
        if (interactionRadius <= 0f)
        {
            Debug.LogError("InteractionSystem: Interaction radius must be positive");
            valid = false;
        }
        
        if (allInteractables.Count == 0)
        {
            Debug.LogWarning("InteractionSystem: No interactables registered");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"InteractionSystem: Validation {(valid ? "PASSED" : "FAILED")} - {allInteractables.Count} interactables");
        }
        
        return valid;
    }
    
    [ContextMenu("Validate Setup")]
    public void ValidateSetupContext()
    {
        ValidateInteractionSetup();
    }
    
    [ContextMenu("Refresh Interactables")]
    public void RefreshInteractablesContext()
    {
        allInteractables.Clear();
        RegisterExistingInteractables();
    }
    
    [ContextMenu("Debug Info")]
    public void DebugInfoContext()
    {
        Debug.Log($"=== INTERACTION SYSTEM DEBUG ===");
        Debug.Log($"Player Transform: {playerTransform?.name ?? "null"}");
        Debug.Log($"Interaction Radius: {interactionRadius}");
        Debug.Log($"Total Interactables: {allInteractables.Count}");
        Debug.Log($"Current Nearest: {currentNearestInteractable?.DisplayName ?? "none"}");
        Debug.Log($"Highlighted: {highlightedInteractable?.DisplayName ?? "none"}");
        Debug.Log($"UI Visible: {isInteractionUIVisible}");
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractInput -= HandleInteractInput;
        }
    }
}