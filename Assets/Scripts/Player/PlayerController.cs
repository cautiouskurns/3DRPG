using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float acceleration = 10.0f;
    public float deceleration = 15.0f;
    
    private Rigidbody rb;
    private Vector2 movementInput;
    private Vector3 previousPosition;
    private bool isInitialized = false;
    
    void Start()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody component not found!");
            return;
        }
        
        // Subscribe to InputManager events
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMovementInput += HandleMovementInput;
            Debug.Log("PlayerController: Subscribed to InputManager movement events");
        }
        else
        {
            Debug.LogError("PlayerController: InputManager instance not found!");
        }
        
        // Store initial position for movement events
        previousPosition = transform.position;
        isInitialized = true;
        
        Debug.Log("PlayerController: Initialized successfully");
    }
    
    void FixedUpdate()
    {
        if (!isInitialized) return;
        
        // Apply physics movement
        ApplyMovement();
        
        // Check for position changes and fire events
        CheckForMovement();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMovementInput -= HandleMovementInput;
        }
    }
    
    private void HandleMovementInput(Vector2 input)
    {
        // Store input for use in FixedUpdate
        movementInput = input;
    }
    
    private void ApplyMovement()
    {
        // Convert 2D input to 3D world movement (X and Z axes only)
        Vector3 targetMovement = new Vector3(movementInput.x, 0, movementInput.y);
        
        // Calculate target velocity at exactly 5 units/second max speed
        Vector3 targetVelocity = targetMovement * moveSpeed;
        
        // Get current velocity (preserve Y component for gravity)
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        
        // Apply smooth acceleration/deceleration
        Vector3 newHorizontalVelocity;
        if (targetVelocity.magnitude > 0.1f)
        {
            // Accelerating towards target velocity
            newHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerating to stop
            newHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
        
        // Apply new velocity (preserve Y component for gravity)
        rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
    }
    
    private void CheckForMovement()
    {
        // Check if player has moved significantly
        Vector3 currentPosition = transform.position;
        float movementDistance = Vector3.Distance(previousPosition, currentPosition);
        
        if (movementDistance > 0.01f) // Threshold to avoid tiny movements
        {
            // Fire PlayerMovedEvent through EventBus
            EventBus.Publish(new PlayerMovedEvent(currentPosition, previousPosition, movementInput));
            
            // Update previous position
            previousPosition = currentPosition;
        }
    }
    
    // Public method to get current movement state
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }
    
    // Public method to check if player is moving
    public bool IsMoving()
    {
        return movementInput.magnitude > 0.1f;
    }
    
    // Public method to get current speed
    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        return horizontalVelocity.magnitude;
    }
}