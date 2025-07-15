using UnityEngine;

/// <summary>
/// Ensures UI system is properly set up and validates all components
/// Add this script to a GameObject in your scene to auto-setup the UI system
/// </summary>
public class UISystemSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    public bool validateAfterSetup = true;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupUISystem();
        }
    }
    
    void Update()
    {
        // Setup hotkey
        if (Input.GetKeyDown(KeyCode.F9))
        {
            SetupUISystem();
        }
    }
    
    [ContextMenu("Setup UI System")]
    public void SetupUISystem()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== UI SYSTEM SETUP STARTING ===");
        }
        
        // Step 1: Ensure UIManager exists
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            GameObject uiManagerGO = new GameObject("UIManager");
            uiManager = uiManagerGO.AddComponent<UIManager>();
            Debug.Log("✅ Created UIManager");
        }
        
        // Step 2: Ensure SettingsManager exists
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            GameObject settingsManagerGO = new GameObject("SettingsManager");
            settingsManager = settingsManagerGO.AddComponent<SettingsManager>();
            Debug.Log("✅ Created SettingsManager");
        }
        
        // Step 3: Ensure EventSystem exists
        UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("✅ Created EventSystem");
        }
        
        // Step 4: Force UI system initialization
        if (uiManager != null)
        {
            // Force canvas creation by calling ShowSettingsMenu then hiding it
            uiManager.ShowSettingsMenu();
            uiManager.HideSettingsMenu();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("=== UI SYSTEM SETUP COMPLETE ===");
        }
        
        if (validateAfterSetup)
        {
            Invoke("ValidateUISystem", 0.5f); // Small delay to let everything initialize
        }
    }
    
    [ContextMenu("Validate UI System")]
    public void ValidateUISystem()
    {
        Debug.Log("=== UI SYSTEM VALIDATION ===");
        
        bool allValid = true;
        
        // Check UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is NULL");
            allValid = false;
        }
        else
        {
            Debug.Log("✅ UIManager found");
            
            // Validate canvases
            if (uiManager.hudCanvas == null)
            {
                Debug.LogError("❌ HUD Canvas is NULL");
                allValid = false;
            }
            else
            {
                Debug.Log("✅ HUD Canvas found");
            }
            
            if (uiManager.menuCanvas == null)
            {
                Debug.LogError("❌ Menu Canvas is NULL");
                allValid = false;
            }
            else
            {
                Debug.Log("✅ Menu Canvas found");
            }
            
            if (uiManager.overlayCanvas == null)
            {
                Debug.LogError("❌ Overlay Canvas is NULL");
                allValid = false;
            }
            else
            {
                Debug.Log("✅ Overlay Canvas found");
            }
            
            // Validate controllers
            if (uiManager.hudController == null)
            {
                Debug.LogWarning("⚠️ HUD Controller is NULL");
            }
            else
            {
                Debug.Log("✅ HUD Controller found");
            }
            
            if (uiManager.settingsPanel == null)
            {
                Debug.LogError("❌ Settings Panel is NULL");
                allValid = false;
            }
            else
            {
                Debug.Log("✅ Settings Panel found");
            }
        }
        
        // Check SettingsManager
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            Debug.LogError("❌ SettingsManager.Instance is NULL");
            allValid = false;
        }
        else
        {
            Debug.Log("✅ SettingsManager found");
        }
        
        // Check EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ EventSystem not found");
            allValid = false;
        }
        else
        {
            Debug.Log("✅ EventSystem found");
        }
        
        // Final result
        if (allValid)
        {
            Debug.Log("🎉 UI SYSTEM VALIDATION: ALL SYSTEMS READY!");
            Debug.Log("Press Escape to test Settings Menu");
        }
        else
        {
            Debug.LogError("❌ UI SYSTEM VALIDATION: ISSUES FOUND");
            Debug.LogError("Run Setup again or check the errors above");
        }
        
        Debug.Log("=== VALIDATION COMPLETE ===");
    }
    
    [ContextMenu("Test Settings Menu")]
    public void TestSettingsMenu()
    {
        Debug.Log("=== TESTING SETTINGS MENU ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found");
            return;
        }
        
        if (uiManager.CurrentUIState == UIState.Game)
        {
            Debug.Log("Opening Settings Menu...");
            uiManager.ShowSettingsMenu();
        }
        else
        {
            Debug.Log("Closing Settings Menu...");
            uiManager.HideSettingsMenu();
        }
    }
}