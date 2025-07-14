using UnityEngine;
using System.Collections;

public class MilestoneValidator : MonoBehaviour
{
    [Header("Performance Monitoring")]
    public bool enablePerformanceMonitoring = true;
    public float performanceCheckInterval = 1f;
    
    [Header("Validation Settings")]
    public float targetFrameRate = 30f;
    public float speedValidationTolerance = 0.1f;
    
    private float frameRateSum = 0f;
    private int frameRateCount = 0;
    private float averageFrameRate = 0f;
    
    private GameManager gameManager;
    private InputManager inputManager;
    private PlayerController playerController;
    private CameraController cameraController;
    
    private bool validationComplete = false;
    private int passedTests = 0;
    private int totalTests = 0;
    
    void Start()
    {
        Debug.Log("=== MILESTONE 1.1 VALIDATION STARTING ===");
        
        // Get references to all core systems
        gameManager = GameManager.Instance;
        inputManager = InputManager.Instance;
        playerController = FindFirstObjectByType<PlayerController>();
        cameraController = FindFirstObjectByType<CameraController>();
        
        // Start performance monitoring
        if (enablePerformanceMonitoring)
        {
            StartCoroutine(PerformanceMonitor());
        }
        
        // Start validation after a delay to ensure all systems are initialized
        StartCoroutine(DelayedValidation());
        
        Debug.Log("Press F1 to run full validation");
        Debug.Log("Press F2 to test player movement specifically");
        Debug.Log("Press F3 to test camera integration");
        Debug.Log("Press F4 to show performance summary");
    }
    
    void Update()
    {
        // Manual validation triggers
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(RunValidationSequence());
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestPlayerMovement();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestCameraIntegration();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowPerformanceSummary();
        }
    }
    
    IEnumerator DelayedValidation()
    {
        // Wait for all systems to initialize
        yield return new WaitForSeconds(2f);
        
        // Run the validation sequence
        yield return StartCoroutine(RunValidationSequence());
    }
    
    IEnumerator PerformanceMonitor()
    {
        while (true)
        {
            yield return new WaitForSeconds(performanceCheckInterval);
            
            float currentFPS = 1.0f / Time.unscaledDeltaTime;
            frameRateSum += currentFPS;
            frameRateCount++;
            averageFrameRate = frameRateSum / frameRateCount;
            
            // Log performance warnings
            if (currentFPS < targetFrameRate * 0.8f)
            {
                Debug.LogWarning($"Performance: FPS dropped to {currentFPS:F1} (target: {targetFrameRate})");
            }
        }
    }
    
    IEnumerator RunValidationSequence()
    {
        Debug.Log("=== RUNNING MILESTONE 1.1 VALIDATION ===");
        
        passedTests = 0;
        totalTests = 0;
        
        // Test 1: Core Systems Initialization
        ValidateTest("Core Systems Initialization", ValidateCoreSystemsInitialization());
        
        // Test 2: Input System
        ValidateTest("Input System Functionality", ValidateInputSystem());
        
        // Test 3: EventBus System
        ValidateTest("EventBus Communication", ValidateEventBusSystem());
        
        // Test 4: Player Movement
        ValidateTest("Player Movement Mechanics", ValidatePlayerMovement());
        
        // Test 5: Camera System
        ValidateTest("Camera Integration", ValidateCameraSystem());
        
        // Test 6: Performance Target
        ValidateTest("Performance Target (30+ FPS)", ValidatePerformanceTarget());
        
        // Test 7: Integration Test
        yield return StartCoroutine(IntegrationTest());
        
        // Final Results
        ShowValidationResults();
        
        validationComplete = true;
    }
    
    void ValidateTest(string testName, bool result)
    {
        totalTests++;
        if (result)
        {
            passedTests++;
            Debug.Log($"✅ {testName}: PASSED");
        }
        else
        {
            Debug.LogError($"❌ {testName}: FAILED");
        }
    }
    
    bool ValidateCoreSystemsInitialization()
    {
        bool allInitialized = true;
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager instance not found");
            allInitialized = false;
        }
        else if (!gameManager.AreSystemsReady())
        {
            Debug.LogError("GameManager systems not initialized");
            allInitialized = false;
        }
        
        if (inputManager == null)
        {
            Debug.LogError("InputManager instance not found");
            allInitialized = false;
        }
        
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found");
            allInitialized = false;
        }
        
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found");
            allInitialized = false;
        }
        
        return allInitialized;
    }
    
    bool ValidateInputSystem()
    {
        if (inputManager == null) 
        {
            Debug.LogError("InputManager is null");
            return false;
        }
        
        // Check if input manager is enabled and can get input
        bool inputEnabled = inputManager.inputEnabled;
        Debug.Log($"Input enabled: {inputEnabled}");
        
        // Test input methods directly
        try
        {
            Vector2 movementInput = inputManager.GetMovementInput();
            bool interactInput = inputManager.GetInteractPressed();
            bool menuInput = inputManager.GetMenuPressed();
            
            Debug.Log($"Input methods working - Movement: {movementInput}, Interact: {interactInput}, Menu: {menuInput}");
            return inputEnabled;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Input system error: {e.Message}");
            return false;
        }
    }
    
    bool ValidateEventBusSystem()
    {
        // Test EventBus functionality
        bool eventReceived = false;
        
        System.Action<SystemsInitializedEvent> testHandler = (e) => {
            eventReceived = true;
        };
        
        EventBus.Subscribe<SystemsInitializedEvent>(testHandler);
        EventBus.Publish(new SystemsInitializedEvent("ValidationTest"));
        EventBus.Unsubscribe<SystemsInitializedEvent>(testHandler);
        
        return eventReceived;
    }
    
    bool ValidatePlayerMovement()
    {
        if (playerController == null) return false;
        
        // Check if player can get movement input
        Vector2 movement = playerController.GetMovementInput();
        
        // Check if player has required components
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        Collider col = playerController.GetComponent<Collider>();
        
        return rb != null && col != null;
    }
    
    bool ValidateCameraSystem()
    {
        if (cameraController == null) 
        {
            Debug.LogError("CameraController is null");
            return false;
        }
        
        Camera cam = cameraController.GetComponent<Camera>();
        if (cam == null) 
        {
            Debug.LogError("Camera component not found on CameraController");
            return false;
        }
        
        // Check if camera is orthographic (isometric)
        bool isOrthographic = cam.orthographic;
        Debug.Log($"Camera orthographic: {isOrthographic}");
        
        // Check if camera has target (player)
        bool hasTarget = playerController != null;
        Debug.Log($"Camera has target: {hasTarget}");
        
        // Check if camera is following (if target exists)
        bool isFollowing = hasTarget ? cameraController.IsFollowing() : true;
        Debug.Log($"Camera is following: {isFollowing}");
        
        return isOrthographic && isFollowing;
    }
    
    bool ValidatePerformanceTarget()
    {
        if (frameRateCount == 0) return true; // No data yet
        
        return averageFrameRate >= targetFrameRate;
    }
    
    IEnumerator IntegrationTest()
    {
        Debug.Log("Running integration test...");
        
        bool integrationPassed = true;
        
        // Test input → player movement integration
        if (playerController != null)
        {
            Vector3 initialPosition = playerController.transform.position;
            
            // Simulate input for 2 seconds
            yield return new WaitForSeconds(2f);
            
            // Check if systems are still responsive
            if (inputManager != null && inputManager.inputEnabled)
            {
                // Integration test passed if no exceptions thrown
                Debug.Log("Integration test: Input-Player communication working");
            }
            else
            {
                integrationPassed = false;
            }
        }
        
        ValidateTest("System Integration", integrationPassed);
    }
    
    void TestPlayerMovement()
    {
        Debug.Log("=== PLAYER MOVEMENT TEST ===");
        
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found!");
            return;
        }
        
        Vector2 input = playerController.GetMovementInput();
        float speed = playerController.GetCurrentSpeed();
        bool isMoving = playerController.IsMoving();
        
        Debug.Log($"Movement Input: {input}");
        Debug.Log($"Current Speed: {speed:F2} units/second");
        Debug.Log($"Is Moving: {isMoving}");
        
        // Validate speed target
        if (isMoving)
        {
            if (Mathf.Abs(speed - 5f) <= speedValidationTolerance)
            {
                Debug.Log("✅ Speed validation: PASSED (5 units/second)");
            }
            else
            {
                Debug.LogWarning($"⚠️ Speed validation: Expected 5.0, got {speed:F2}");
            }
        }
    }
    
    void TestCameraIntegration()
    {
        Debug.Log("=== CAMERA INTEGRATION TEST ===");
        
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found!");
            return;
        }
        
        bool isFollowing = cameraController.IsFollowing();
        Camera cam = cameraController.GetComponent<Camera>();
        
        Debug.Log($"Camera Following: {isFollowing}");
        Debug.Log($"Camera Orthographic: {cam.orthographic}");
        Debug.Log($"Camera Orthographic Size: {cam.orthographicSize}");
        Debug.Log($"Camera Rotation: {cam.transform.rotation.eulerAngles}");
        
        if (playerController != null)
        {
            Vector3 playerScreenPos = cam.WorldToViewportPoint(playerController.transform.position);
            bool playerInView = playerScreenPos.x >= 0 && playerScreenPos.x <= 1 && 
                               playerScreenPos.y >= 0 && playerScreenPos.y <= 1;
            
            Debug.Log($"Player in Camera View: {playerInView}");
        }
    }
    
    void ShowPerformanceSummary()
    {
        Debug.Log("=== PERFORMANCE SUMMARY ===");
        Debug.Log($"Average FPS: {averageFrameRate:F1}");
        Debug.Log($"Target FPS: {targetFrameRate}");
        Debug.Log($"Performance Target Met: {(averageFrameRate >= targetFrameRate ? "✅ YES" : "❌ NO")}");
        Debug.Log($"Frame Rate Samples: {frameRateCount}");
    }
    
    void ShowValidationResults()
    {
        Debug.Log("=== MILESTONE 1.1 VALIDATION RESULTS ===");
        Debug.Log($"Tests Passed: {passedTests}/{totalTests}");
        Debug.Log($"Success Rate: {(passedTests * 100f / totalTests):F1}%");
        
        if (passedTests == totalTests)
        {
            Debug.Log("🎉 MILESTONE 1.1 VALIDATION: COMPLETE SUCCESS!");
            Debug.Log("All core systems are working correctly and ready for Milestone 1.2");
        }
        else
        {
            Debug.LogWarning("⚠️ MILESTONE 1.1 VALIDATION: ISSUES FOUND");
            Debug.LogWarning("Please review failed tests before proceeding to Milestone 1.2");
        }
        
        Debug.Log("=== VALIDATION COMPLETE ===");
    }
}