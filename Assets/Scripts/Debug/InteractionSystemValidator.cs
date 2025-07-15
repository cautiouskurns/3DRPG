using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InteractionSystemValidator : MonoBehaviour
{
    [Header("Validation Settings")]
    public bool validateOnStart = true;
    public bool enableDetailedLogs = true;
    public bool validateAudioSetup = true;
    public bool validateUISetup = true;
    
    [Header("Test Settings")]
    public bool runInteractionTests = false;
    public float testRadius = 20f;
    public KeyCode validationHotkey = KeyCode.F9;
    
    [System.Serializable]
    public class ValidationResult
    {
        public bool passed;
        public string category;
        public string message;
        public ValidationLevel level;
        
        public ValidationResult(string category, string message, bool passed, ValidationLevel level = ValidationLevel.Info)
        {
            this.category = category;
            this.message = message;
            this.passed = passed;
            this.level = level;
        }
    }
    
    public enum ValidationLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }
    
    private List<ValidationResult> lastValidationResults = new List<ValidationResult>();
    
    void Start()
    {
        if (validateOnStart)
        {
            Invoke(nameof(ValidateCompleteSystem), 1f);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(validationHotkey))
        {
            ValidateCompleteSystem();
        }
    }
    
    [ContextMenu("Validate Complete System")]
    public void ValidateCompleteSystem()
    {
        lastValidationResults.Clear();
        
        Debug.Log("=== INTERACTION SYSTEM VALIDATION STARTED ===");
        
        ValidateInteractionSystem();
        ValidateInteractableObjects();
        ValidateContentSystem();
        ValidatePlayerIntegration();
        
        if (validateUISetup)
        {
            ValidateUIIntegration();
        }
        
        if (validateAudioSetup)
        {
            ValidateAudioIntegration();
        }
        
        if (runInteractionTests)
        {
            RunInteractionTests();
        }
        
        PrintValidationSummary();
        
        Debug.Log("=== INTERACTION SYSTEM VALIDATION COMPLETED ===");
    }
    
    void ValidateInteractionSystem()
    {
        AddResult("Core System", "Validating InteractionSystem...", true);
        
        // Check InteractionSystem singleton
        if (InteractionSystem.Instance == null)
        {
            AddResult("Core System", "InteractionSystem instance not found", false, ValidationLevel.Critical);
            return;
        }
        
        AddResult("Core System", "InteractionSystem singleton found", true);
        
        // Validate InteractionSystem setup
        bool systemValid = InteractionSystem.Instance.ValidateInteractionSetup();
        AddResult("Core System", $"InteractionSystem setup validation: {(systemValid ? "PASSED" : "FAILED")}", systemValid, systemValid ? ValidationLevel.Info : ValidationLevel.Error);
        
        // Check interaction radius
        float radius = InteractionSystem.Instance.interactionRadius;
        if (radius <= 0f)
        {
            AddResult("Core System", $"Invalid interaction radius: {radius}", false, ValidationLevel.Error);
        }
        else if (radius > 5f)
        {
            AddResult("Core System", $"Interaction radius seems large: {radius}m", true, ValidationLevel.Warning);
        }
        else
        {
            AddResult("Core System", $"Interaction radius is appropriate: {radius}m", true);
        }
        
        // Check player reference
        Transform playerTransform = InteractionSystem.Instance.playerTransform;
        if (playerTransform == null)
        {
            AddResult("Core System", "Player transform not assigned to InteractionSystem", false, ValidationLevel.Error);
        }
        else
        {
            AddResult("Core System", $"Player transform found: {playerTransform.name}", true);
        }
        
        // Check registered interactables count
        List<InteractableObject> allInteractables = InteractionSystem.Instance.GetAllInteractables();
        AddResult("Core System", $"Registered interactables: {allInteractables.Count}", allInteractables.Count > 0, allInteractables.Count == 0 ? ValidationLevel.Warning : ValidationLevel.Info);
    }
    
    void ValidateInteractableObjects()
    {
        AddResult("Interactables", "Validating InteractableObjects...", true);
        
        InteractableObject[] allInteractables = FindObjectsOfType<InteractableObject>();
        AddResult("Interactables", $"Found {allInteractables.Length} InteractableObject components in scene", allInteractables.Length > 0, allInteractables.Length == 0 ? ValidationLevel.Warning : ValidationLevel.Info);
        
        int validInteractables = 0;
        int buildingCount = 0;
        int propCount = 0;
        int loreCount = 0;
        int highlightingEnabled = 0;
        int withContent = 0;
        int withAudio = 0;
        
        foreach (InteractableObject interactable in allInteractables)
        {
            bool isValid = true;
            
            // Check basic setup
            if (string.IsNullOrEmpty(interactable.DisplayName))
            {
                AddResult("Interactables", $"InteractableObject on {interactable.gameObject.name} has no display name", false, ValidationLevel.Warning);
                isValid = false;
            }
            
            // Check content
            if (interactable.HasValidContent)
            {
                withContent++;
            }
            else
            {
                AddResult("Interactables", $"{interactable.DisplayName} has no valid content", false, ValidationLevel.Warning);
            }
            
            // Check audio
            if (interactable.InteractionSound != null)
            {
                withAudio++;
            }
            
            // Check highlighting
            if (interactable.enableHighlighting)
            {
                highlightingEnabled++;
            }
            
            // Count types
            if (interactable is BuildingInteractable) buildingCount++;
            else if (interactable is PropInteractable) propCount++;
            else if (interactable is LoreInteractable) loreCount++;
            
            if (isValid) validInteractables++;
        }
        
        AddResult("Interactables", $"Valid interactables: {validInteractables}/{allInteractables.Length}", validInteractables == allInteractables.Length);
        AddResult("Interactables", $"Building interactables: {buildingCount}", buildingCount > 0);
        AddResult("Interactables", $"Prop interactables: {propCount}", propCount > 0);
        AddResult("Interactables", $"Lore interactables: {loreCount}", loreCount > 0);
        AddResult("Interactables", $"With highlighting: {highlightingEnabled}/{allInteractables.Length}", highlightingEnabled > 0);
        AddResult("Interactables", $"With content: {withContent}/{allInteractables.Length}", withContent > allInteractables.Length * 0.8f);
        AddResult("Interactables", $"With audio: {withAudio}/{allInteractables.Length}", withAudio > 0);
        
        // Check if we meet the 15+ requirement
        bool meetsRequirement = allInteractables.Length >= 15;
        AddResult("Interactables", $"Meets 15+ interactable requirement: {meetsRequirement} ({allInteractables.Length}/15)", meetsRequirement, meetsRequirement ? ValidationLevel.Info : ValidationLevel.Warning);
    }
    
    void ValidateContentSystem()
    {
        AddResult("Content System", "Validating content display system...", true);
        
        // Check InteractionContentPanel
        if (InteractionContentPanel.Instance == null)
        {
            AddResult("Content System", "InteractionContentPanel instance not found", false, ValidationLevel.Error);
        }
        else
        {
            AddResult("Content System", "InteractionContentPanel singleton found", true);
        }
        
        // Check VillageInteractables setup
        VillageInteractables villageSetup = FindFirstObjectByType<VillageInteractables>();
        if (villageSetup == null)
        {
            AddResult("Content System", "VillageInteractables component not found", false, ValidationLevel.Warning);
        }
        else
        {
            AddResult("Content System", "VillageInteractables component found", true);
            
            if (villageSetup.autoCreateInteractables)
            {
                AddResult("Content System", "Auto-create interactables is enabled", true);
            }
        }
    }
    
    void ValidatePlayerIntegration()
    {
        AddResult("Player Integration", "Validating player integration...", true);
        
        // Check PlayerController
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null)
        {
            AddResult("Player Integration", "PlayerController not found", false, ValidationLevel.Error);
            return;
        }
        
        AddResult("Player Integration", "PlayerController found", true);
        
        // Check InputManager
        if (InputManager.Instance == null)
        {
            AddResult("Player Integration", "InputManager instance not found", false, ValidationLevel.Error);
        }
        else
        {
            AddResult("Player Integration", "InputManager singleton found", true);
            
            if (InputManager.Instance.inputEnabled)
            {
                AddResult("Player Integration", "Input is enabled", true);
            }
            else
            {
                AddResult("Player Integration", "Input is disabled", false, ValidationLevel.Warning);
            }
        }
        
        // Check EventBus
        bool eventBusWorks = TestEventBus();
        AddResult("Player Integration", $"EventBus functionality: {(eventBusWorks ? "Working" : "Failed")}", eventBusWorks, eventBusWorks ? ValidationLevel.Info : ValidationLevel.Error);
    }
    
    void ValidateUIIntegration()
    {
        AddResult("UI Integration", "Validating UI integration...", true);
        
        // Check StaticUIManager
        if (StaticUIManager.Instance == null)
        {
            AddResult("UI Integration", "StaticUIManager instance not found", false, ValidationLevel.Warning);
        }
        else
        {
            AddResult("UI Integration", "StaticUIManager singleton found", true);
            
            bool uiValid = StaticUIManager.Instance.ValidateUISetup();
            AddResult("UI Integration", $"UI setup validation: {(uiValid ? "PASSED" : "FAILED")}", uiValid, uiValid ? ValidationLevel.Info : ValidationLevel.Warning);
        }
        
        // Check InteractionPrompt
        if (InteractionPrompt.Instance == null)
        {
            AddResult("UI Integration", "InteractionPrompt instance not found", false, ValidationLevel.Warning);
        }
        else
        {
            AddResult("UI Integration", "InteractionPrompt singleton found", true);
        }
        
        // Check for Canvas components
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        AddResult("UI Integration", $"Canvas components found: {canvases.Length}", canvases.Length > 0);
    }
    
    void ValidateAudioIntegration()
    {
        AddResult("Audio Integration", "Validating audio integration...", true);
        
        // Check AudioManager
        if (AudioManager.Instance == null)
        {
            AddResult("Audio Integration", "AudioManager instance not found", false, ValidationLevel.Warning);
        }
        else
        {
            AddResult("Audio Integration", "AudioManager singleton found", true);
            
            bool audioValid = AudioManager.Instance.ValidateAudioSetup();
            AddResult("Audio Integration", $"Audio setup validation: {(audioValid ? "PASSED" : "FAILED")}", audioValid, audioValid ? ValidationLevel.Info : ValidationLevel.Warning);
        }
        
        // Check InteractionAudioManager
        if (InteractionAudioManager.Instance == null)
        {
            AddResult("Audio Integration", "InteractionAudioManager instance not found", false, ValidationLevel.Warning);
        }
        else
        {
            AddResult("Audio Integration", "InteractionAudioManager singleton found", true);
        }
    }
    
    void RunInteractionTests()
    {
        AddResult("Testing", "Running interaction tests...", true);
        
        if (InteractionSystem.Instance == null || InteractionSystem.Instance.playerTransform == null)
        {
            AddResult("Testing", "Cannot run tests - missing core components", false, ValidationLevel.Error);
            return;
        }
        
        Vector3 playerPosition = InteractionSystem.Instance.playerTransform.position;
        List<InteractableObject> nearbyInteractables = InteractionSystem.Instance.GetInteractablesInRange(playerPosition, testRadius);
        
        AddResult("Testing", $"Found {nearbyInteractables.Count} interactables within {testRadius}m for testing", nearbyInteractables.Count > 0);
        
        int testsPassed = 0;
        int testsRun = 0;
        
        foreach (InteractableObject interactable in nearbyInteractables.Take(5))
        {
            testsRun++;
            
            if (TestInteractable(interactable))
            {
                testsPassed++;
            }
        }
        
        AddResult("Testing", $"Interaction tests passed: {testsPassed}/{testsRun}", testsPassed == testsRun, testsPassed < testsRun ? ValidationLevel.Warning : ValidationLevel.Info);
    }
    
    bool TestInteractable(InteractableObject interactable)
    {
        try
        {
            bool canInteract = interactable.CanInteract();
            string promptText = interactable.GetPromptText();
            bool hasValidContent = interactable.HasValidContent;
            
            if (enableDetailedLogs)
            {
                Debug.Log($"Test {interactable.DisplayName}: CanInteract={canInteract}, Prompt='{promptText}', HasContent={hasValidContent}");
            }
            
            return canInteract && !string.IsNullOrEmpty(promptText);
        }
        catch (System.Exception e)
        {
            AddResult("Testing", $"Test failed for {interactable.DisplayName}: {e.Message}", false, ValidationLevel.Error);
            return false;
        }
    }
    
    bool TestEventBus()
    {
        try
        {
            bool receivedEvent = false;
            
            System.Action<InputStateChangedEvent> testHandler = (eventData) => {
                receivedEvent = true;
            };
            
            EventBus.Subscribe<InputStateChangedEvent>(testHandler);
            EventBus.Publish(new InputStateChangedEvent(true));
            EventBus.Unsubscribe<InputStateChangedEvent>(testHandler);
            
            return receivedEvent;
        }
        catch (System.Exception e)
        {
            if (enableDetailedLogs)
            {
                Debug.LogError($"EventBus test failed: {e.Message}");
            }
            return false;
        }
    }
    
    void AddResult(string category, string message, bool passed, ValidationLevel level = ValidationLevel.Info)
    {
        ValidationResult result = new ValidationResult(category, message, passed, level);
        lastValidationResults.Add(result);
        
        if (enableDetailedLogs)
        {
            string prefix = passed ? "✓" : "✗";
            string levelStr = level == ValidationLevel.Info ? "" : $" [{level}]";
            Debug.Log($"{prefix} {category}: {message}{levelStr}");
        }
    }
    
    void PrintValidationSummary()
    {
        int totalTests = lastValidationResults.Count;
        int passedTests = lastValidationResults.Count(r => r.passed);
        int errors = lastValidationResults.Count(r => !r.passed && (r.level == ValidationLevel.Error || r.level == ValidationLevel.Critical));
        int warnings = lastValidationResults.Count(r => !r.passed && r.level == ValidationLevel.Warning);
        
        Debug.Log($"\n=== VALIDATION SUMMARY ===");
        Debug.Log($"Total Tests: {totalTests}");
        Debug.Log($"Passed: {passedTests} ({((float)passedTests / totalTests * 100):F1}%)");
        Debug.Log($"Errors: {errors}");
        Debug.Log($"Warnings: {warnings}");
        
        if (errors == 0 && warnings == 0)
        {
            Debug.Log("🎉 INTERACTION SYSTEM VALIDATION PASSED - All systems operational!");
        }
        else if (errors == 0)
        {
            Debug.Log("⚠️ VALIDATION COMPLETED WITH WARNINGS - System functional but has minor issues");
        }
        else
        {
            Debug.Log("❌ VALIDATION FAILED - Critical issues found that may prevent proper operation");
        }
        
        // Print category breakdown
        var categories = lastValidationResults.GroupBy(r => r.category);
        Debug.Log("\nCategory Breakdown:");
        foreach (var category in categories)
        {
            int categoryPassed = category.Count(r => r.passed);
            int categoryTotal = category.Count();
            Debug.Log($"  {category.Key}: {categoryPassed}/{categoryTotal}");
        }
    }
    
    [ContextMenu("Quick Validate Core")]
    public void QuickValidateCore()
    {
        lastValidationResults.Clear();
        ValidateInteractionSystem();
        ValidateInteractableObjects();
        PrintValidationSummary();
    }
    
    [ContextMenu("Test Nearest Interactable")]
    public void TestNearestInteractable()
    {
        if (InteractionSystem.Instance?.CurrentInteractable != null)
        {
            InteractableObject nearest = InteractionSystem.Instance.CurrentInteractable;
            Debug.Log($"Testing nearest interactable: {nearest.DisplayName}");
            TestInteractable(nearest);
        }
        else
        {
            Debug.Log("No interactable in range to test");
        }
    }
    
    public ValidationResult[] GetLastResults()
    {
        return lastValidationResults.ToArray();
    }
}