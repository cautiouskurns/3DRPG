using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float followSpeed = 5f;
    public float lookAheadDistance = 2f;
    
    [Header("Isometric Settings")]
    public Vector3 offset = new Vector3(0, 10, -10);
    public float isometricAngle = 30f;
    
    [Header("Smoothing")]
    public float positionSmoothing = 1.0f;
    public float rotationSmoothing = 0.1f;
    public bool useInitialSmoothing = true;
    
    private Camera cameraComponent;
    private Vector3 velocity;
    private Vector3 targetPosition;
    private bool isInitialized = false;
    
    void Start()
    {
        // Get Camera component
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError("CameraController: Camera component not found!");
            return;
        }
        
        // Find player target if not assigned
        if (target == null)
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                target = playerController.transform;
                Debug.Log("CameraController: Automatically found player target");
            }
            else
            {
                Debug.LogError("CameraController: No target assigned and no PlayerController found!");
                return;
            }
        }
        
        // Subscribe to player movement events for look-ahead
        EventBus.Subscribe<PlayerMovedEvent>(OnPlayerMoved);
        
        // Set initial isometric position and rotation
        SetIsometricView();
        
        // Set initial target position
        targetPosition = target.position + offset;
        
        // Choose between smooth initialization or immediate positioning
        if (useInitialSmoothing)
        {
            // Let smoothing handle initial positioning
            Debug.Log("CameraController: Using smooth initialization");
        }
        else
        {
            // Immediate positioning (original behavior)
            transform.position = targetPosition;
        }
        
        isInitialized = true;
        Debug.Log("CameraController: Initialized with isometric follow camera");
    }
    
    void LateUpdate()
    {
        if (!isInitialized || target == null) return;
        
        // Update camera position and rotation
        UpdateCameraPosition();
        UpdateCameraRotation();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        EventBus.Unsubscribe<PlayerMovedEvent>(OnPlayerMoved);
    }
    
    private void SetIsometricView()
    {
        // Set isometric rotation (looking down at angle)
        transform.rotation = Quaternion.Euler(isometricAngle, 0, 0);
        
        // Set orthographic camera for true isometric view
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = 8f;
        
        Debug.Log($"CameraController: Set isometric view with {isometricAngle}° angle");
    }
    
    private void UpdateCameraPosition()
    {
        // Calculate base target position
        Vector3 baseTargetPosition = target.position + offset;
        
        // Apply look-ahead based on player movement
        Vector3 lookAheadOffset = Vector3.zero;
        PlayerController playerController = target.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsMoving())
        {
            Vector2 movementInput = playerController.GetMovementInput();
            lookAheadOffset = new Vector3(movementInput.x, 0, movementInput.y) * lookAheadDistance;
        }
        
        // Combine base position with look-ahead
        targetPosition = baseTargetPosition + lookAheadOffset;
        
        // Smooth camera movement using SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothing);
    }
    
    private void UpdateCameraRotation()
    {
        // Maintain isometric angle
        Quaternion targetRotation = Quaternion.Euler(isometricAngle, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }
    
    private void OnPlayerMoved(PlayerMovedEvent moveEvent)
    {
        // Camera will update in LateUpdate based on player position
        // This event can be used for additional camera behaviors if needed
    }
    
    // Public method to change camera target
    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            Debug.Log($"CameraController: Target changed to {newTarget.name}");
        }
    }
    
    // Public method to adjust camera offset
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        Debug.Log($"CameraController: Offset changed to {newOffset}");
    }
    
    // Public method to adjust isometric angle
    public void SetIsometricAngle(float newAngle)
    {
        isometricAngle = newAngle;
        SetIsometricView();
        Debug.Log($"CameraController: Isometric angle changed to {newAngle}°");
    }
    
    // Public method to check if camera is following
    public bool IsFollowing()
    {
        return target != null && isInitialized;
    }
}