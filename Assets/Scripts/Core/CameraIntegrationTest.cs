using UnityEngine;

public class CameraIntegrationTest : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerController playerController;
    private Camera mainCamera;
    private bool logCameraDetails = false;
    private float testTimer = 0f;
    
    void Start()
    {
        // Get references to components
        cameraController = FindFirstObjectByType<CameraController>();
        playerController = FindFirstObjectByType<PlayerController>();
        mainCamera = Camera.main;
        
        if (cameraController == null)
        {
            Debug.LogError("CameraIntegrationTest: CameraController not found!");
            return;
        }
        
        if (playerController == null)
        {
            Debug.LogError("CameraIntegrationTest: PlayerController not found!");
            return;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("CameraIntegrationTest: Main Camera not found!");
            return;
        }
        
        // Subscribe to player movement events
        EventBus.Subscribe<PlayerMovedEvent>(OnPlayerMoved);
        
        Debug.Log("CameraIntegrationTest: Monitoring camera-player integration");
        Debug.Log("Press C to toggle detailed camera logging");
        Debug.Log("Press V to validate camera setup");
        
        // Initial validation
        ValidateCameraSetup();
    }
    
    void Update()
    {
        testTimer += Time.deltaTime;
        
        // Test controls
        if (Input.GetKeyDown(KeyCode.C))
        {
            logCameraDetails = !logCameraDetails;
            Debug.Log($"Detailed camera logging: {(logCameraDetails ? "ON" : "OFF")}");
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            ValidateCameraSetup();
        }
        
        // Log camera details every 2 seconds if enabled
        if (logCameraDetails && testTimer >= 2f)
        {
            LogCameraDetails();
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
        if (logCameraDetails)
        {
            Vector3 playerPos = moveEvent.NewPosition;
            Vector3 cameraPos = mainCamera.transform.position;
            float distance = Vector3.Distance(playerPos, cameraPos);
            
            Debug.Log($"Camera following player move - Distance: {distance:F2}, Player: {playerPos}, Camera: {cameraPos}");
        }
    }
    
    private void ValidateCameraSetup()
    {
        Debug.Log("=== CAMERA INTEGRATION VALIDATION ===");
        
        // Check if camera is following
        if (cameraController.IsFollowing())
        {
            Debug.Log("✅ Camera is following player");
        }
        else
        {
            Debug.LogError("❌ Camera is NOT following player");
        }
        
        // Check camera projection
        if (mainCamera.orthographic)
        {
            Debug.Log($"✅ Camera is orthographic (size: {mainCamera.orthographicSize})");
        }
        else
        {
            Debug.LogWarning("⚠️ Camera is perspective (should be orthographic for isometric)");
        }
        
        // Check camera rotation for isometric angle
        float xRotation = mainCamera.transform.rotation.eulerAngles.x;
        if (Mathf.Abs(xRotation - 30f) < 5f)
        {
            Debug.Log($"✅ Camera has isometric angle ({xRotation:F1}°)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Camera angle might not be isometric ({xRotation:F1}° - expected ~30°)");
        }
        
        // Check if player is in camera view
        Vector3 playerScreenPos = mainCamera.WorldToViewportPoint(playerController.transform.position);
        bool playerInView = playerScreenPos.x >= 0 && playerScreenPos.x <= 1 && 
                           playerScreenPos.y >= 0 && playerScreenPos.y <= 1 && 
                           playerScreenPos.z > 0;
        
        if (playerInView)
        {
            Debug.Log($"✅ Player is in camera view (screen pos: {playerScreenPos})");
        }
        else
        {
            Debug.LogError($"❌ Player is NOT in camera view (screen pos: {playerScreenPos})");
        }
        
        Debug.Log("=== VALIDATION COMPLETE ===");
    }
    
    private void LogCameraDetails()
    {
        if (cameraController == null || playerController == null) return;
        
        Vector3 playerPos = playerController.transform.position;
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        
        float distanceToPlayer = Vector3.Distance(playerPos, cameraPos);
        bool playerMoving = playerController.IsMoving();
        
        Debug.Log($"Camera Status - Distance: {distanceToPlayer:F2}, Rotation: {cameraRot}, Player Moving: {playerMoving}");
        
        // Check if camera is maintaining proper distance
        float expectedDistance = Mathf.Sqrt(10*10 + 10*10); // Based on default offset (0, 10, -10)
        if (Mathf.Abs(distanceToPlayer - expectedDistance) > 2f)
        {
            Debug.LogWarning($"Camera distance unusual! Expected: ~{expectedDistance:F2}, Actual: {distanceToPlayer:F2}");
        }
    }
    
    // Test method to demonstrate camera responsiveness
    public void TestCameraResponsiveness()
    {
        Debug.Log("Testing camera responsiveness...");
        
        // This would simulate player movement and check camera response
        // For now, just log the current state
        if (cameraController != null && playerController != null)
        {
            Debug.Log($"Camera following: {cameraController.IsFollowing()}");
            Debug.Log($"Player moving: {playerController.IsMoving()}");
        }
    }
}