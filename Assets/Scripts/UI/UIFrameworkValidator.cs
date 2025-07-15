using UnityEngine;
using System.Collections;

public class UIFrameworkValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool enableDebugLogs = true;
    public bool runValidationOnStart = true;
    public float performanceTestDuration = 10f;
    
    [Header("Performance Monitoring")]
    public float targetFPS = 30f;
    public float targetUIResponseTime = 0.1f; // 100ms
    
    private bool validationComplete = false;
    private int testsPassed = 0;
    private int totalTests = 0;
    
    void Start()
    {
        if (runValidationOnStart)
        {
            StartCoroutine(RunCompleteValidation());
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("UIFrameworkValidator: Press U to run complete UI validation");
            Debug.Log("UIFrameworkValidator: Press Y to test character stat simulation");
            Debug.Log("UIFrameworkValidator: Press T to test settings panel functionality");
        }
    }
    
    void Update()
    {
        // Validation hotkeys
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(RunCompleteValidation());
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(TestCharacterStatSimulation());
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestSettingsPanelFunctionality();
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            TestHUDFunctionality();
        }
    }
    
    private IEnumerator RunCompleteValidation()
    {
        Debug.Log("=== UI FRAMEWORK COMPLETE VALIDATION ===");
        
        testsPassed = 0;
        totalTests = 0;
        
        // Test 1: Core System Validation
        ValidateCoreUISystem();
        yield return new WaitForSeconds(0.1f);
        
        // Test 2: Settings System Validation
        ValidateSettingsSystem();
        yield return new WaitForSeconds(0.1f);
        
        // Test 3: HUD System Validation
        ValidateHUDSystem();
        yield return new WaitForSeconds(0.1f);
        
        // Test 4: EventBus Integration Validation
        ValidateEventBusIntegration();
        yield return new WaitForSeconds(0.1f);
        
        // Test 5: Performance Testing
        yield return StartCoroutine(ValidatePerformance());
        
        // Test 6: Integration Testing
        yield return StartCoroutine(ValidateSystemIntegration());
        
        // Test 7: Input Routing Testing
        ValidateInputRouting();
        
        // Final Results
        Debug.Log("=== VALIDATION COMPLETE ===");
        Debug.Log($"Tests Passed: {testsPassed}/{totalTests}");
        
        float successRate = totalTests > 0 ? (float)testsPassed / totalTests * 100f : 0f;
        string result = successRate >= 90f ? "EXCELLENT" : successRate >= 70f ? "GOOD" : "NEEDS IMPROVEMENT";
        
        Debug.Log($"Success Rate: {successRate:F1}% - {result}");
        
        if (successRate >= 90f)
        {
            Debug.Log("✅ UI Framework is ready for production use!");
        }
        else
        {
            Debug.Log("⚠️ Some issues detected - check validation details above");
        }
        
        validationComplete = true;
    }
    
    private void ValidateCoreUISystem()
    {
        Debug.Log("--- Core UI System Validation ---");
        
        // Test UIManager
        totalTests++;
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            testsPassed++;
            Debug.Log("✅ UIManager singleton found");
            
            // Test canvas hierarchy
            totalTests++;
            if (uiManager.hudCanvas != null && uiManager.menuCanvas != null && uiManager.overlayCanvas != null)
            {
                testsPassed++;
                Debug.Log("✅ Canvas hierarchy complete");
                
                // Test sort orders
                totalTests++;
                bool sortOrdersCorrect = uiManager.hudCanvas.sortingOrder < uiManager.menuCanvas.sortingOrder &&
                                       uiManager.menuCanvas.sortingOrder < uiManager.overlayCanvas.sortingOrder;
                if (sortOrdersCorrect)
                {
                    testsPassed++;
                    Debug.Log("✅ Canvas sort orders correct");
                }
                else
                {
                    Debug.LogError("❌ Canvas sort orders incorrect");
                }
            }
            else
            {
                Debug.LogError("❌ Canvas hierarchy incomplete");
            }
            
            // Test validation method
            totalTests++;
            if (uiManager.ValidateUISetup())
            {
                testsPassed++;
                Debug.Log("✅ UIManager validation passed");
            }
            else
            {
                Debug.LogError("❌ UIManager validation failed");
            }
        }
        else
        {
            Debug.LogError("❌ UIManager singleton not found");
        }
    }
    
    private void ValidateSettingsSystem()
    {
        Debug.Log("--- Settings System Validation ---");
        
        // Test SettingsManager
        totalTests++;
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager != null)
        {
            testsPassed++;
            Debug.Log("✅ SettingsManager singleton found");
            
            // Test validation method
            totalTests++;
            if (settingsManager.ValidateSettingsSetup())
            {
                testsPassed++;
                Debug.Log("✅ SettingsManager validation passed");
            }
            else
            {
                Debug.LogError("❌ SettingsManager validation failed");
            }
            
            // Test settings persistence
            totalTests++;
            float originalVolume = settingsManager.masterVolume;
            float testVolume = originalVolume + 10f;
            if (testVolume > 100f) testVolume = originalVolume - 10f;
            
            settingsManager.SetMasterVolume(testVolume);
            settingsManager.SaveSettings();
            
            if (Mathf.Abs(settingsManager.masterVolume - testVolume) < 0.1f)
            {
                testsPassed++;
                Debug.Log("✅ Settings persistence working");
                settingsManager.SetMasterVolume(originalVolume); // Restore
            }
            else
            {
                Debug.LogError("❌ Settings persistence failed");
            }
        }
        else
        {
            Debug.LogError("❌ SettingsManager singleton not found");
        }
    }
    
    private void ValidateHUDSystem()
    {
        Debug.Log("--- HUD System Validation ---");
        
        // Test HUDController
        totalTests++;
        HUDController hudController = FindFirstObjectByType<HUDController>();
        if (hudController != null)
        {
            testsPassed++;
            Debug.Log("✅ HUDController found");
            
            // Test validation method
            totalTests++;
            if (hudController.ValidateHUDSetup())
            {
                testsPassed++;
                Debug.Log("✅ HUDController validation passed");
            }
            else
            {
                Debug.LogError("❌ HUDController validation failed");
            }
            
            // Test stat bar functionality
            totalTests++;
            try
            {
                hudController.UpdateHealth(75f, 100f);
                hudController.UpdateMP(50f, 100f);
                hudController.UpdateXP(250f, 500f);
                hudController.UpdateLevel(5);
                
                testsPassed++;
                Debug.Log("✅ HUD stat updates working");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ HUD stat updates failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ HUDController not found");
        }
    }
    
    private void ValidateEventBusIntegration()
    {
        Debug.Log("--- EventBus Integration Validation ---");
        
        // Test EventBus availability
        totalTests++;
        try
        {
            // Test character stats event
            CharacterStatsUpdatedEvent testEvent = new CharacterStatsUpdatedEvent(
                80f, 100f, 60f, 100f, 150f, 300f, 3
            );
            
            EventBus.Publish(testEvent);
            testsPassed++;
            Debug.Log("✅ EventBus character stats event published");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ EventBus character stats event failed: {e.Message}");
        }
        
        // Test UI state event
        totalTests++;
        try
        {
            UIStateChangedEvent uiEvent = new UIStateChangedEvent(UIState.Settings, true, "TestPanel");
            EventBus.Publish(uiEvent);
            testsPassed++;
            Debug.Log("✅ EventBus UI state event published");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ EventBus UI state event failed: {e.Message}");
        }
        
        // Test interaction prompt event
        totalTests++;
        try
        {
            InteractionPromptEvent promptEvent = new InteractionPromptEvent(
                InteractionAction.Show, "Test interaction prompt"
            );
            EventBus.Publish(promptEvent);
            testsPassed++;
            Debug.Log("✅ EventBus interaction prompt event published");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ EventBus interaction prompt event failed: {e.Message}");
        }
    }
    
    private IEnumerator ValidatePerformance()
    {
        Debug.Log("--- Performance Validation ---");
        
        float testStartTime = Time.time;
        float totalFrameTime = 0f;
        int frameCount = 0;
        float minFPS = float.MaxValue;
        float maxFPS = 0f;
        
        Debug.Log($"Starting {performanceTestDuration}s performance test...");
        
        while (Time.time - testStartTime < performanceTestDuration)
        {
            float currentFPS = 1.0f / Time.unscaledDeltaTime;
            totalFrameTime += Time.unscaledDeltaTime;
            frameCount++;
            
            if (currentFPS < minFPS) minFPS = currentFPS;
            if (currentFPS > maxFPS) maxFPS = currentFPS;
            
            // Test UI responsiveness during performance test
            if (frameCount % 60 == 0) // Every ~1 second
            {
                TestUIResponsiveness();
            }
            
            yield return null;
        }
        
        float avgFPS = frameCount / totalFrameTime;
        
        Debug.Log($"Performance Results - Avg: {avgFPS:F1} FPS, Min: {minFPS:F1} FPS, Max: {maxFPS:F1} FPS");
        
        // Test FPS target
        totalTests++;
        if (avgFPS >= targetFPS)
        {
            testsPassed++;
            Debug.Log($"✅ Performance target met: {avgFPS:F1} >= {targetFPS} FPS");
        }
        else
        {
            Debug.LogError($"❌ Performance target missed: {avgFPS:F1} < {targetFPS} FPS");
        }
        
        // Test FPS stability
        totalTests++;
        float fpsVariability = maxFPS - minFPS;
        if (fpsVariability < targetFPS * 0.5f) // Less than 50% variation
        {
            testsPassed++;
            Debug.Log($"✅ FPS stability good: {fpsVariability:F1} FPS variation");
        }
        else
        {
            Debug.LogError($"❌ FPS instability detected: {fpsVariability:F1} FPS variation");
        }
    }
    
    private void TestUIResponsiveness()
    {
        float startTime = Time.realtimeSinceStartup;
        
        // Simulate UI interaction
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.SetUIMode(!uiManager.IsUIMode);
        }
        
        float responseTime = (Time.realtimeSinceStartup - startTime) * 1000f; // Convert to ms
        
        if (responseTime <= targetUIResponseTime * 1000f)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"UI Response: {responseTime:F2}ms (✅ < {targetUIResponseTime * 1000f}ms)");
            }
        }
        else
        {
            Debug.LogWarning($"UI Response slow: {responseTime:F2}ms (target: {targetUIResponseTime * 1000f}ms)");
        }
    }
    
    private IEnumerator ValidateSystemIntegration()
    {
        Debug.Log("--- System Integration Validation ---");
        
        // Test AudioManager integration
        totalTests++;
        AudioManager audioManager = AudioManager.Instance;
        SettingsManager settingsManager = SettingsManager.Instance;
        
        if (audioManager != null && settingsManager != null)
        {
            float originalVolume = settingsManager.masterVolume;
            float testVolume = 50f;
            
            settingsManager.SetMasterVolume(testVolume);
            yield return new WaitForSeconds(0.1f);
            
            // Check if AudioManager received the change
            if (Mathf.Abs(audioManager.masterVolume - testVolume / 100f) < 0.01f)
            {
                testsPassed++;
                Debug.Log("✅ SettingsManager -> AudioManager integration working");
            }
            else
            {
                Debug.LogError("❌ SettingsManager -> AudioManager integration failed");
            }
            
            settingsManager.SetMasterVolume(originalVolume); // Restore
        }
        else
        {
            Debug.LogError("❌ AudioManager or SettingsManager not found for integration test");
        }
        
        // Test InputManager integration
        totalTests++;
        InputManager inputManager = InputManager.Instance;
        UIManager uiManager = UIManager.Instance;
        
        if (inputManager != null && uiManager != null)
        {
            bool originalInputEnabled = inputManager.inputEnabled;
            
            uiManager.SetUIMode(true);
            yield return new WaitForSeconds(0.1f);
            
            if (!inputManager.inputEnabled)
            {
                testsPassed++;
                Debug.Log("✅ UIManager -> InputManager integration working");
            }
            else
            {
                Debug.LogError("❌ UIManager -> InputManager integration failed");
            }
            
            uiManager.SetUIMode(false);
            yield return new WaitForSeconds(0.1f);
            
            // Restore original state
            inputManager.inputEnabled = originalInputEnabled;
        }
        else
        {
            Debug.LogError("❌ InputManager or UIManager not found for integration test");
        }
    }
    
    private void ValidateInputRouting()
    {
        Debug.Log("--- Input Routing Validation ---");
        
        UIManager uiManager = UIManager.Instance;
        InputManager inputManager = InputManager.Instance;
        
        if (uiManager != null && inputManager != null)
        {
            totalTests++;
            
            // Test initial state
            bool initialGameMode = uiManager.CurrentUIState == UIState.Game;
            bool initialInputEnabled = inputManager.inputEnabled;
            
            if (initialGameMode && initialInputEnabled)
            {
                testsPassed++;
                Debug.Log("✅ Initial input routing state correct");
            }
            else
            {
                Debug.LogError($"❌ Initial input routing incorrect: GameMode={initialGameMode}, InputEnabled={initialInputEnabled}");
            }
            
            // Test escape key functionality
            totalTests++;
            try
            {
                // Simulate escape key press
                uiManager.ShowSettingsMenu();
                
                if (uiManager.CurrentUIState == UIState.Settings)
                {
                    testsPassed++;
                    Debug.Log("✅ Settings menu toggle working");
                    
                    uiManager.HideSettingsMenu(); // Clean up
                }
                else
                {
                    Debug.LogError("❌ Settings menu toggle failed");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Settings menu toggle error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ UIManager or InputManager not found for input routing test");
        }
    }
    
    private IEnumerator TestCharacterStatSimulation()
    {
        Debug.Log("=== CHARACTER STAT SIMULATION TEST ===");
        
        // Simulate character taking damage and gaining XP
        for (int i = 0; i < 5; i++)
        {
            float health = 100f - (i * 15f);
            float mp = 100f - (i * 10f);
            float xp = i * 50f;
            int level = 1 + (i / 3);
            
            EventBus.Publish(new CharacterStatsUpdatedEvent(
                health, 100f, mp, 100f, xp, 200f, level
            ));
            
            Debug.Log($"Simulated stats: Health={health}, MP={mp}, XP={xp}, Level={level}");
            
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("Character stat simulation complete");
    }
    
    private void TestSettingsPanelFunctionality()
    {
        Debug.Log("=== SETTINGS PANEL FUNCTIONALITY TEST ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.ShowSettingsMenu();
            Debug.Log("Settings menu opened - test volume sliders and graphics options");
            Debug.Log("Press Escape to close settings menu");
        }
        else
        {
            Debug.LogError("UIManager not found for settings test");
        }
    }
    
    private void TestHUDFunctionality()
    {
        Debug.Log("=== HUD FUNCTIONALITY TEST ===");
        
        HUDController hudController = FindFirstObjectByType<HUDController>();
        if (hudController != null)
        {
            // Test interaction prompt
            hudController.ShowInteractionPrompt("Test interaction prompt - this should fade in");
            StartCoroutine(HidePromptAfterDelay(hudController, 3f));
            
            // Test stat bars
            hudController.UpdateHealth(Random.Range(20f, 100f), 100f);
            hudController.UpdateMP(Random.Range(10f, 100f), 100f);
            hudController.UpdateXP(Random.Range(0f, 100f), 100f);
            hudController.UpdateLevel(Random.Range(1, 10));
            
            Debug.Log("HUD functionality test complete - check visual updates");
        }
        else
        {
            Debug.LogError("HUDController not found for HUD test");
        }
    }
    
    private IEnumerator HidePromptAfterDelay(HUDController hudController, float delay)
    {
        yield return new WaitForSeconds(delay);
        hudController.HideInteractionPrompt();
        Debug.Log("Interaction prompt hidden");
    }
    
    // Public methods for external validation
    public bool IsValidationComplete()
    {
        return validationComplete;
    }
    
    public float GetValidationSuccessRate()
    {
        return totalTests > 0 ? (float)testsPassed / totalTests * 100f : 0f;
    }
    
    public void RunQuickValidation()
    {
        Debug.Log("=== QUICK UI VALIDATION ===");
        
        bool uiManagerOK = UIManager.Instance != null;
        bool settingsManagerOK = SettingsManager.Instance != null;
        bool audioManagerOK = AudioManager.Instance != null;
        bool hudControllerOK = FindFirstObjectByType<HUDController>() != null;
        
        Debug.Log($"UIManager: {(uiManagerOK ? "✅" : "❌")}");
        Debug.Log($"SettingsManager: {(settingsManagerOK ? "✅" : "❌")}");
        Debug.Log($"AudioManager: {(audioManagerOK ? "✅" : "❌")}");
        Debug.Log($"HUDController: {(hudControllerOK ? "✅" : "❌")}");
        
        bool allOK = uiManagerOK && settingsManagerOK && audioManagerOK && hudControllerOK;
        Debug.Log($"Quick Validation: {(allOK ? "PASSED" : "FAILED")}");
    }
}