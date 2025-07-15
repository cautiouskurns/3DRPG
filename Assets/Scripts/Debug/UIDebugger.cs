using UnityEngine;
using UnityEngine.UI;

public class UIDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool autoDebugOnStart = true;
    
    void Start()
    {
        if (autoDebugOnStart)
        {
            Invoke("DebugUISystem", 1f); // Delay to let systems initialize
        }
    }
    
    void Update()
    {
        // Debug hotkeys
        if (Input.GetKeyDown(KeyCode.F12))
        {
            DebugUISystem();
        }
        
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ForceCreateSettingsPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.F10))
        {
            TestSettingsMenu();
        }
    }
    
    public void DebugUISystem()
    {
        Debug.Log("=== UI SYSTEM DEBUG ===");
        
        // Check UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager.Instance is NULL");
            return;
        }
        else
        {
            Debug.Log("✅ UIManager.Instance found");
        }
        
        // Check Canvases
        Debug.Log($"HUD Canvas: {(uiManager.hudCanvas != null ? "✅" : "❌")}");
        Debug.Log($"Menu Canvas: {(uiManager.menuCanvas != null ? "✅" : "❌")}");
        Debug.Log($"Overlay Canvas: {(uiManager.overlayCanvas != null ? "✅" : "❌")}");
        
        // Check SettingsPanel
        SettingsPanel settingsPanel = uiManager.settingsPanel;
        if (settingsPanel == null)
        {
            Debug.LogError("❌ SettingsPanel is NULL in UIManager");
            
            // Try to find it manually
            SettingsPanel foundPanel = FindFirstObjectByType<SettingsPanel>();
            if (foundPanel != null)
            {
                Debug.LogWarning("⚠️ SettingsPanel exists but not registered in UIManager");
                Debug.Log($"Found SettingsPanel on GameObject: {foundPanel.gameObject.name}");
            }
            else
            {
                Debug.LogError("❌ No SettingsPanel found in scene");
            }
        }
        else
        {
            Debug.Log("✅ SettingsPanel found in UIManager");
            Debug.Log($"SettingsPanel GameObject: {settingsPanel.gameObject.name}");
            Debug.Log($"SettingsPanel Active: {settingsPanel.gameObject.activeInHierarchy}");
        }
        
        // Check SettingsManager
        SettingsManager settingsManager = SettingsManager.Instance;
        Debug.Log($"SettingsManager: {(settingsManager != null ? "✅" : "❌")}");
        
        // Check all canvases in scene
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log($"Total Canvases in scene: {allCanvases.Length}");
        foreach (Canvas canvas in allCanvases)
        {
            Debug.Log($"  - {canvas.name} (Sort Order: {canvas.sortingOrder}, Active: {canvas.gameObject.activeInHierarchy})");
        }
        
        Debug.Log("=== DEBUG COMPLETE ===");
    }
    
    public void ForceCreateSettingsPanel()
    {
        Debug.Log("=== FORCE CREATING SETTINGS PANEL ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found");
            return;
        }
        
        if (uiManager.menuCanvas == null)
        {
            Debug.LogError("Menu Canvas not found");
            return;
        }
        
        // Create SettingsPanel GameObject
        GameObject settingsPanelGO = new GameObject("Settings Panel (Debug)");
        settingsPanelGO.transform.SetParent(uiManager.menuCanvas.transform, false);
        
        // Add SettingsPanel component
        SettingsPanel panel = settingsPanelGO.AddComponent<SettingsPanel>();
        
        // Register with UIManager
        uiManager.settingsPanel = panel;
        uiManager.RegisterPanel("Settings", settingsPanelGO);
        
        Debug.Log("✅ Settings Panel created and registered");
    }
    
    public void TestSettingsMenu()
    {
        Debug.Log("=== TESTING SETTINGS MENU ===");
        
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found");
            return;
        }
        
        Debug.Log($"Current UI State: {uiManager.CurrentUIState}");
        
        if (uiManager.CurrentUIState == UIState.Game)
        {
            Debug.Log("Showing settings menu...");
            uiManager.ShowSettingsMenu();
        }
        else if (uiManager.CurrentUIState == UIState.Settings)
        {
            Debug.Log("Hiding settings menu...");
            uiManager.HideSettingsMenu();
        }
    }
    
    [ContextMenu("Debug UI System")]
    public void DebugUISystemContext()
    {
        DebugUISystem();
    }
    
    [ContextMenu("Force Create Settings Panel")]
    public void ForceCreateSettingsPanelContext()
    {
        ForceCreateSettingsPanel();
    }
    
    [ContextMenu("Test Settings Menu")]
    public void TestSettingsMenuContext()
    {
        TestSettingsMenu();
    }
}