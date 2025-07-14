using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Input Settings")]
    public bool inputEnabled = true;
    
    // Input events (use C# Actions)
    public System.Action<Vector2> OnMovementInput;
    public System.Action<bool> OnInteractInput;
    public System.Action<bool> OnMenuInput;
    
    private Vector2 lastMovementInput;
    private bool lastInteractInput;
    private bool lastMenuInput;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Register with GameManager if it exists
        if (GameManager.Instance != null)
        {
            Debug.Log("InputManager initialized and registered with GameManager");
        }
    }
    
    void Update()
    {
        // Only process input if enabled
        if (!inputEnabled) return;
        
        // Poll input and fire events
        HandleMovementInput();
        HandleInteractInput();
        HandleMenuInput();
    }
    
    private void HandleMovementInput()
    {
        Vector2 currentMovement = GetMovementInput();
        
        // Only fire event if movement input changed or is non-zero
        if (currentMovement != lastMovementInput || currentMovement != Vector2.zero)
        {
            OnMovementInput?.Invoke(currentMovement);
            lastMovementInput = currentMovement;
        }
    }
    
    private void HandleInteractInput()
    {
        bool currentInteract = GetInteractPressed();
        
        // Fire event on input change
        if (currentInteract != lastInteractInput)
        {
            OnInteractInput?.Invoke(currentInteract);
            lastInteractInput = currentInteract;
        }
    }
    
    private void HandleMenuInput()
    {
        bool currentMenu = GetMenuPressed();
        
        // Fire event on input change
        if (currentMenu != lastMenuInput)
        {
            OnMenuInput?.Invoke(currentMenu);
            lastMenuInput = currentMenu;
        }
    }
    
    public Vector2 GetMovementInput()
    {
        // WASD/Arrow detection using Unity's legacy Input system
        float horizontal = 0f;
        float vertical = 0f;
        
        // Horizontal input (A/D or Left/Right arrows)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;
        
        // Vertical input (W/S or Up/Down arrows)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;
        
        Vector2 movement = new(horizontal, vertical);
        
        // Normalize to ensure consistent speed in diagonal movement
        return movement.normalized;
    }
    
    public bool GetInteractPressed()
    {
        // Space/E detection
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E);
    }
    
    public bool GetMenuPressed()
    {
        // Escape/M detection
        return Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.M);
    }
    
    public void EnableInput(bool enabled)
    {
        inputEnabled = enabled;
        
        // Clear any active input when disabled
        if (!enabled)
        {
            OnMovementInput?.Invoke(Vector2.zero);
            OnInteractInput?.Invoke(false);
            OnMenuInput?.Invoke(false);
        }
        
        Debug.Log($"Input {(enabled ? "enabled" : "disabled")}");
        
        // Broadcast input state change event
        EventBus.Publish(new InputStateChangedEvent(enabled));
    }
}