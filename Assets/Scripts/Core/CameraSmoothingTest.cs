using UnityEngine;

public class CameraSmoothingTest : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerController playerController;
    private bool logCameraMovement = false;
    
    void Start()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        playerController = FindFirstObjectByType<PlayerController>();
        
        if (cameraController == null)
        {
            Debug.LogError("CameraSmoothingTest: CameraController not found!");
            return;
        }
        
        if (playerController == null)
        {
            Debug.LogError("CameraSmoothingTest: PlayerController not found!");
            return;
        }
        
        Debug.Log("CameraSmoothingTest: Press G to toggle camera movement logging");
        Debug.Log("CameraSmoothingTest: Press H to test camera smoothing settings");
        Debug.Log("CameraSmoothingTest: Use WASD to move and observe camera following");
        
        // Log initial camera settings
        Debug.Log($"Camera Smoothing Settings - Position: {cameraController.positionSmoothing}, Rotation: {cameraController.rotationSmoothing}");
        Debug.Log($"Camera Offset: {cameraController.offset}, Look Ahead: {cameraController.lookAheadDistance}");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            logCameraMovement = !logCameraMovement;
            Debug.Log($"Camera movement logging: {(logCameraMovement ? "ON" : "OFF")}");
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            TestCameraSmoothing();
        }
        
        if (logCameraMovement)
        {
            LogCameraMovement();
        }
    }
    
    void LogCameraMovement()
    {
        if (cameraController == null || playerController == null) return;
        
        Vector3 playerPos = playerController.transform.position;
        Vector3 cameraPos = cameraController.transform.position;
        Vector3 expectedPos = playerPos + cameraController.offset;
        
        float distanceToTarget = Vector3.Distance(cameraPos, expectedPos);
        bool playerMoving = playerController.IsMoving();
        
        if (playerMoving || distanceToTarget > 0.1f)
        {
            Debug.Log($"Camera Follow - Player: {playerPos:F2}, Camera: {cameraPos:F2}, Distance to target: {distanceToTarget:F2}, Player moving: {playerMoving}");
        }
    }
    
    void TestCameraSmoothing()
    {
        Debug.Log("=== CAMERA SMOOTHING TEST ===");
        
        if (cameraController == null) return;
        
        // Test different smoothing values
        float[] testValues = { 0.1f, 0.5f, 1.0f, 2.0f };
        
        Debug.Log("Testing different smoothing values:");
        foreach (float value in testValues)
        {
            Debug.Log($"- {value}f: {(value < 0.5f ? "Very fast (may pop)" : value < 1.0f ? "Fast" : value < 2.0f ? "Smooth" : "Very smooth (may lag)")}");
        }
        
        Debug.Log($"Current smoothing: {cameraController.positionSmoothing}f");
        Debug.Log("Recommended: 0.5f - 1.5f for smooth following");
        
        // Test camera responsiveness
        Vector3 playerPos = playerController.transform.position;
        Vector3 cameraPos = cameraController.transform.position;
        Vector3 expectedPos = playerPos + cameraController.offset;
        
        float currentDistance = Vector3.Distance(cameraPos, expectedPos);
        Debug.Log($"Current camera distance from target: {currentDistance:F2}");
        
        if (currentDistance > 2.0f)
        {
            Debug.LogWarning("Camera seems far from target - may need adjustment");
        }
        else if (currentDistance < 0.1f)
        {
            Debug.Log("Camera is very close to target - good tracking");
        }
        else
        {
            Debug.Log("Camera distance is normal - smooth following");
        }
    }
}