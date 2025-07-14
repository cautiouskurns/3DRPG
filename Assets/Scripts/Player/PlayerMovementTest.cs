using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    private PlayerController playerController;
    private float testTimer = 0f;
    private bool logMovementDetails = false;
    
    void Start()
    {
        // Get reference to PlayerController
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerMovementTest: PlayerController not found in scene!");
            return;
        }
        
        // Subscribe to EventBus events to test integration
        EventBus.Subscribe<PlayerMovedEvent>(OnPlayerMoved);
        
        Debug.Log("PlayerMovementTest: Monitoring player movement integration");
        Debug.Log("Press L to toggle detailed movement logging");
        Debug.Log("Press I to test input disable/enable");
    }
    
    void Update()
    {
        testTimer += Time.deltaTime;
        
        // Test controls
        if (Input.GetKeyDown(KeyCode.L))
        {
            logMovementDetails = !logMovementDetails;
            Debug.Log($"Detailed movement logging: {(logMovementDetails ? "ON" : "OFF")}");
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Test input disable/enable
            if (InputManager.Instance != null)
            {
                bool currentState = InputManager.Instance.inputEnabled;
                InputManager.Instance.EnableInput(!currentState);
                Debug.Log($"Input toggled: {(!currentState ? "ENABLED" : "DISABLED")}");
            }
        }
        
        // Log movement details every second if enabled
        if (logMovementDetails && testTimer >= 1f)
        {
            LogMovementDetails();
            testTimer = 0f;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        EventBus.Unsubscribe<PlayerMovedEvent>(OnPlayerMoved);
    }
    
    private void OnPlayerMoved(PlayerMovedEvent moveEvent)
    {
        if (logMovementDetails)
        {
            float distance = Vector3.Distance(moveEvent.PreviousPosition, moveEvent.NewPosition);
            Debug.Log($"Player moved {distance:F3} units. Input: {moveEvent.MovementInput} at {moveEvent.Timestamp:F2}s");
        }
    }
    
    private void LogMovementDetails()
    {
        if (playerController != null)
        {
            Vector2 input = playerController.GetMovementInput();
            float speed = playerController.GetCurrentSpeed();
            bool isMoving = playerController.IsMoving();
            
            Debug.Log($"Movement Status - Input: {input}, Speed: {speed:F2} u/s, Moving: {isMoving}");
            
            // Validate speed is exactly 5 units/second when moving
            if (isMoving && Mathf.Abs(speed - 5f) > 0.1f)
            {
                Debug.LogWarning($"Speed validation failed! Expected: 5.0, Actual: {speed:F2}");
            }
        }
    }
}