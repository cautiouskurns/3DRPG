using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    public string targetSceneName;
    public string spawnPointName;
    public string doorName;
    
    [Header("Interaction")]
    public float interactionRange = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    
    [Header("UI Feedback")]
    public GameObject interactionPrompt;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private bool playerInRange = false;
    private PlayerController playerController;
    private InputManager inputManager;
    private InteractionPrompt promptUI;
    
    void Start()
    {
        // Initialize door trigger
        SetupTriggerCollider();
        
        // Get reference to InputManager
        inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogError($"DoorTrigger ({doorName}): InputManager not found!");
        }
        
        // Setup interaction prompt
        SetupInteractionPrompt();
        
        // Validate configuration
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"DoorTrigger ({doorName}): Target scene name not set!");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"DoorTrigger initialized for {doorName} -> {targetSceneName}");
        }
    }
    
    void Update()
    {
        // Check for interaction input when player is in range
        if (playerInRange && inputManager != null && inputManager.GetInteractPressed())
        {
            HandleInteraction();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player enters interaction range
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerInRange = true;
                ShowPrompt();
                
                if (enableDebugLogs)
                {
                    Debug.Log($"Player entered range of {doorName}");
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if player leaves interaction range
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerController = null;
            HidePrompt();
            
            if (enableDebugLogs)
            {
                Debug.Log($"Player left range of {doorName}");
            }
        }
    }
    
    private void HandleInteraction()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"Cannot interact with {doorName}: No target scene set!");
            return;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"Player interacting with {doorName} -> Loading {targetSceneName} with spawn point {spawnPointName}");
        }
        
        // Hide prompt before transition
        HidePrompt();
        
        // Trigger scene transition
        SimpleSceneTransition transitionManager = SimpleSceneTransition.Instance;
        if (transitionManager != null)
        {
            transitionManager.TransitionToScene(targetSceneName, spawnPointName);
        }
        else
        {
            // Fallback to basic scene loading if no transition manager
            Debug.LogWarning("SimpleSceneTransition not found, using basic scene loading");
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
    
    private void ShowPrompt()
    {
        if (promptUI != null)
        {
            string promptText = $"Press E to enter {doorName}";
            promptUI.ShowPrompt(promptText);
        }
        else if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
    
    private void HidePrompt()
    {
        if (promptUI != null)
        {
            promptUI.HidePrompt();
        }
        else if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    private void SetupTriggerCollider()
    {
        // Ensure we have a trigger collider
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            // Add a box collider if none exists
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(interactionRange, 3f, interactionRange);
            boxCollider.isTrigger = true;
            
            if (enableDebugLogs)
            {
                Debug.Log($"Added trigger collider to {doorName}");
            }
        }
        else
        {
            // Ensure existing collider is set as trigger
            triggerCollider.isTrigger = true;
        }
        
        // Set layer to Interaction if it exists
        int interactionLayer = LayerMask.NameToLayer("Interaction");
        if (interactionLayer != -1)
        {
            gameObject.layer = interactionLayer;
        }
    }
    
    private void SetupInteractionPrompt()
    {
        // Try to find InteractionPrompt component
        promptUI = FindFirstObjectByType<InteractionPrompt>();
        
        if (promptUI == null && interactionPrompt == null)
        {
            Debug.LogWarning($"DoorTrigger ({doorName}): No interaction prompt setup found!");
        }
    }
    
    // Public method to update door configuration
    public void SetDoorConfiguration(string sceneName, string spawn, string name)
    {
        targetSceneName = sceneName;
        spawnPointName = spawn;
        doorName = name;
        
        if (enableDebugLogs)
        {
            Debug.Log($"Door configuration updated: {doorName} -> {targetSceneName}");
        }
    }
    
    // Public method to check if player is in range (for validation)
    public bool IsPlayerInRange()
    {
        return playerInRange;
    }
    
    // Public method to get target scene name (for validation)
    public string GetTargetScene()
    {
        return targetSceneName;
    }
}