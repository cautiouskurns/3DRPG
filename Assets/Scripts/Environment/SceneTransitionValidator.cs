using UnityEngine;

public class SceneTransitionValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool enableDebugLogs = true;
    public bool enableAutoValidation = true;
    
    [Header("Test Results")]
    public int totalTests = 0;
    public int passedTests = 0;
    
    void Start()
    {
        if (enableAutoValidation)
        {
            // Run validation after a brief delay
            Invoke("ValidateSceneTransitions", 2f);
        }
        
        Debug.Log("SceneTransitionValidator: Press T to validate scene transitions");
        Debug.Log("SceneTransitionValidator: Press Y to test individual door triggers");
        Debug.Log("SceneTransitionValidator: Press U to validate core systems integration");
    }
    
    void Update()
    {
        // Manual validation triggers
        if (Input.GetKeyDown(KeyCode.T))
        {
            ValidateSceneTransitions();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TestDoorTriggers();
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            ValidateCoreSystemsIntegration();
        }
    }
    
    void ValidateSceneTransitions()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== SCENE TRANSITION VALIDATION ===");
        }
        
        totalTests = 0;
        passedTests = 0;
        
        // Test 1: Check door triggers
        ValidateTest("Door Triggers Setup", ValidateDoorTriggers());
        
        // Test 2: Check scene transition manager
        ValidateTest("Scene Transition Manager", ValidateTransitionManager());
        
        // Test 3: Check interaction prompt system
        ValidateTest("Interaction Prompt System", ValidateInteractionPrompt());
        
        // Test 4: Check core systems integration
        ValidateTest("Core Systems Integration", ValidateCoreSystemsIntegration());
        
        // Test 5: Check spawn points in current scene
        ValidateTest("Spawn Points Setup", ValidateSpawnPoints());
        
        // Test 6: Check player state preservation capability
        ValidateTest("Player State Preservation", ValidatePlayerStateSystem());
        
        // Show final results
        ShowValidationResults();
    }
    
    void ValidateTest(string testName, bool result)
    {
        totalTests++;
        if (result)
        {
            passedTests++;
            if (enableDebugLogs)
            {
                Debug.Log($"✅ {testName}: PASSED");
            }
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.LogError($"❌ {testName}: FAILED");
            }
        }
    }
    
    bool ValidateDoorTriggers()
    {
        // Count door triggers
        DoorTrigger[] doors = FindObjectsByType<DoorTrigger>(FindObjectsSortMode.None);
        
        if (enableDebugLogs)
        {
            Debug.Log($"Door triggers found: {doors.Length} (expected: 6)");
        }
        
        if (doors.Length == 0)
        {
            Debug.LogError("No door triggers found! Add DoorTrigger scripts to building entrances.");
            return false;
        }
        
        // Check each door configuration
        int validDoors = 0;
        foreach (DoorTrigger door in doors)
        {
            string targetScene = door.GetTargetScene();
            if (!string.IsNullOrEmpty(targetScene))
            {
                validDoors++;
                if (enableDebugLogs)
                {
                    Debug.Log($"Door '{door.doorName}' -> '{targetScene}' configured correctly");
                }
            }
            else
            {
                Debug.LogWarning($"Door '{door.doorName}' has no target scene set!");
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"Valid door configurations: {validDoors}/{doors.Length}");
        }
        
        return validDoors > 0;
    }
    
    bool ValidateTransitionManager()
    {
        SimpleSceneTransition transitionManager = SimpleSceneTransition.Instance;
        bool managerExists = transitionManager != null;
        
        if (enableDebugLogs)
        {
            Debug.Log($"Scene transition manager ready: {managerExists}");
        }
        
        if (managerExists)
        {
            bool setupValid = transitionManager.ValidateTransitionSetup();
            if (enableDebugLogs)
            {
                Debug.Log($"Transition manager setup valid: {setupValid}");
            }
            return setupValid;
        }
        else
        {
            Debug.LogError("SimpleSceneTransition manager not found! Add it to the scene.");
            return false;
        }
    }
    
    bool ValidateInteractionPrompt()
    {
        InteractionPrompt[] prompts = FindObjectsByType<InteractionPrompt>(FindObjectsSortMode.None);
        
        if (enableDebugLogs)
        {
            Debug.Log($"Interaction prompts found: {prompts.Length}");
        }
        
        if (prompts.Length == 0)
        {
            Debug.LogWarning("No InteractionPrompt components found. UI feedback may not work.");
            return false; // This could be optional depending on setup
        }
        
        // Check if at least one prompt is properly configured
        foreach (InteractionPrompt prompt in prompts)
        {
            if (prompt.promptCanvas != null || prompt.autoCreateUI)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("Valid interaction prompt configuration found");
                }
                return true;
            }
        }
        
        Debug.LogWarning("No properly configured interaction prompts found");
        return false;
    }
    
    bool ValidateCoreSystemsIntegration()
    {
        bool allSystemsOK = true;
        
        // Check GameManager
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager instance not found!");
            allSystemsOK = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"GameManager ready, current state: {gameManager.currentState}");
        }
        
        // Check InputManager
        InputManager inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogError("InputManager instance not found!");
            allSystemsOK = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"InputManager ready, input enabled: {inputManager.inputEnabled}");
        }
        
        // Check PlayerController
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController not found!");
            allSystemsOK = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"PlayerController ready at position: {player.transform.position}");
        }
        
        // Check CameraController
        CameraController camera = FindFirstObjectByType<CameraController>();
        if (camera == null)
        {
            Debug.LogError("CameraController not found!");
            allSystemsOK = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("CameraController ready");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"Core systems integration: {(allSystemsOK ? "PASSED" : "FAILED")}");
        }
        
        return allSystemsOK;
    }
    
    bool ValidateSpawnPoints()
    {
        // Look for spawn points in current scene
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        
        if (enableDebugLogs)
        {
            Debug.Log($"Spawn points with 'SpawnPoint' tag: {spawnPoints.Length}");
        }
        
        // Also look for common spawn point names
        string[] commonSpawnNames = { "PlayerSpawn", "ExitSpawn", "SpawnPoint" };
        int namedSpawnPoints = 0;
        
        foreach (string spawnName in commonSpawnNames)
        {
            GameObject spawn = GameObject.Find(spawnName);
            if (spawn != null)
            {
                namedSpawnPoints++;
                if (enableDebugLogs)
                {
                    Debug.Log($"Found spawn point: {spawnName} at {spawn.transform.position}");
                }
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"Named spawn points found: {namedSpawnPoints}");
        }
        
        // At least one spawn point should exist
        return spawnPoints.Length > 0 || namedSpawnPoints > 0;
    }
    
    bool ValidatePlayerStateSystem()
    {
        // Test player state data creation
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Cannot test player state: No PlayerController found");
            return false;
        }
        
        try
        {
            // Create test player state
            PlayerStateData testState = new PlayerStateData(player);
            
            // Validate state data
            bool isValid = testState.IsValid();
            
            if (enableDebugLogs)
            {
                Debug.Log($"Player state test: {(isValid ? "PASSED" : "FAILED")}");
                Debug.Log($"Test state summary: {testState.GetSummary()}");
            }
            
            return isValid;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Player state validation failed with exception: {e.Message}");
            return false;
        }
    }
    
    void TestDoorTriggers()
    {
        Debug.Log("=== DOOR TRIGGER TESTING ===");
        
        DoorTrigger[] doors = FindObjectsByType<DoorTrigger>(FindObjectsSortMode.None);
        
        if (doors.Length == 0)
        {
            Debug.LogError("No door triggers found to test!");
            return;
        }
        
        foreach (DoorTrigger door in doors)
        {
            Debug.Log($"Door: {door.doorName}");
            Debug.Log($"  Target Scene: {door.GetTargetScene()}");
            Debug.Log($"  Player In Range: {door.IsPlayerInRange()}");
            Debug.Log($"  Interaction Key: {door.interactionKey}");
        }
        
        Debug.Log("=== DOOR TESTING COMPLETE ===");
        Debug.Log("Walk up to doors and check console for range detection");
    }
    
    void ShowValidationResults()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== SCENE TRANSITION VALIDATION RESULTS ===");
            Debug.Log($"Tests Passed: {passedTests}/{totalTests}");
            Debug.Log($"Success Rate: {(passedTests * 100f / totalTests):F1}%");
        }
        
        if (passedTests == totalTests)
        {
            Debug.Log("🎉 SCENE TRANSITION VALIDATION: COMPLETE SUCCESS!");
            Debug.Log("All systems ready for scene transitions. Walk up to building doors and press E to test!");
        }
        else
        {
            Debug.LogWarning("⚠️ SCENE TRANSITION VALIDATION: ISSUES FOUND");
            Debug.LogWarning("Please fix failed tests before testing door interactions");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("=== VALIDATION COMPLETE ===");
            Debug.Log("TESTING INSTRUCTIONS:");
            Debug.Log("1. Walk up to building doors");
            Debug.Log("2. Look for 'Press E to enter' prompt");
            Debug.Log("3. Press E to transition to interior");
            Debug.Log("4. Use exit door to return to village");
        }
    }
    
    // Public method for external validation calls
    public bool RunQuickValidation()
    {
        bool doorTriggersOK = ValidateDoorTriggers();
        bool transitionManagerOK = ValidateTransitionManager();
        bool coreSystemsOK = ValidateCoreSystemsIntegration();
        
        return doorTriggersOK && transitionManagerOK && coreSystemsOK;
    }
    
    // Public method to get validation summary
    public string GetValidationSummary()
    {
        if (totalTests == 0)
        {
            return "No validation run yet";
        }
        
        return $"Scene Transition Validation: {passedTests}/{totalTests} tests passed ({(passedTests * 100f / totalTests):F1}%)";
    }
}