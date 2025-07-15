using UnityEngine;

/// <summary>
/// Helps diagnose what's causing Unity crashes
/// </summary>
public class CrashDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool enableDiagnosticLogs = true;
    public bool safeMode = true; // Disable problematic components
    
    void Start()
    {
        if (enableDiagnosticLogs)
        {
            Debug.Log("=== CRASH DIAGNOSTIC STARTING ===");
        }
        
        if (safeMode)
        {
            DisableProblematicComponents();
        }
        
        DiagnoseUISystem();
    }
    
    void Update()
    {
        // Safe diagnostic hotkey
        if (Input.GetKeyDown(KeyCode.F6))
        {
            DiagnoseUISystem();
        }
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            ToggleSafeMode();
        }
    }
    
    void DisableProblematicComponents()
    {
        if (enableDiagnosticLogs)
        {
            Debug.Log("=== DISABLING PROBLEMATIC COMPONENTS ===");
        }
        
        // Disable UIManager components that might cause crashes
        UIManager[] uiManagers = FindObjectsByType<UIManager>(FindObjectsSortMode.None);
        foreach (UIManager manager in uiManagers)
        {
            manager.enabled = false;
            Debug.Log($"Disabled UIManager: {manager.gameObject.name}");
        }
        
        // Disable SettingsPanel components
        SettingsPanel[] settingsPanels = FindObjectsByType<SettingsPanel>(FindObjectsSortMode.None);
        foreach (SettingsPanel panel in settingsPanels)
        {
            panel.enabled = false;
            Debug.Log($"Disabled SettingsPanel: {panel.gameObject.name}");
        }
        
        // Disable SettingsPanelFix components
        SettingsPanelFix[] fixes = FindObjectsByType<SettingsPanelFix>(FindObjectsSortMode.None);
        foreach (SettingsPanelFix fix in fixes)
        {
            fix.enabled = false;
            Debug.Log($"Disabled SettingsPanelFix: {fix.gameObject.name}");
        }
        
        // Disable UISystemSetup components
        UISystemSetup[] setups = FindObjectsByType<UISystemSetup>(FindObjectsSortMode.None);
        foreach (UISystemSetup setup in setups)
        {
            setup.enabled = false;
            Debug.Log($"Disabled UISystemSetup: {setup.gameObject.name}");
        }
        
        if (enableDiagnosticLogs)
        {
            Debug.Log("✅ Problematic components disabled - Escape key should be safe now");
        }
    }
    
    void DiagnoseUISystem()
    {
        if (enableDiagnosticLogs)
        {
            Debug.Log("=== UI SYSTEM DIAGNOSIS ===");
        }
        
        // Check for multiple UIManager instances
        UIManager[] uiManagers = FindObjectsByType<UIManager>(FindObjectsSortMode.None);
        Debug.Log($"UIManager instances found: {uiManagers.Length}");
        if (uiManagers.Length > 1)
        {
            Debug.LogWarning("⚠️ Multiple UIManager instances detected - this can cause crashes!");
        }
        
        // Check for multiple SettingsManager instances
        SettingsManager[] settingsManagers = FindObjectsByType<SettingsManager>(FindObjectsSortMode.None);
        Debug.Log($"SettingsManager instances found: {settingsManagers.Length}");
        if (settingsManagers.Length > 1)
        {
            Debug.LogWarning("⚠️ Multiple SettingsManager instances detected - this can cause crashes!");
        }
        
        // Check for Canvas issues
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log($"Canvas components found: {canvases.Length}");
        
        // Check EventSystem
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        Debug.Log($"EventSystem instances found: {eventSystems.Length}");
        
        // Check for problematic scripts
        SettingsPanel[] settingsPanels = FindObjectsByType<SettingsPanel>(FindObjectsSortMode.None);
        Debug.Log($"SettingsPanel instances found: {settingsPanels.Length}");
        
        SettingsPanelFix[] fixes = FindObjectsByType<SettingsPanelFix>(FindObjectsSortMode.None);
        Debug.Log($"SettingsPanelFix instances found: {fixes.Length}");
        
        UISystemSetup[] setups = FindObjectsByType<UISystemSetup>(FindObjectsSortMode.None);
        Debug.Log($"UISystemSetup instances found: {setups.Length}");
        
        if (enableDiagnosticLogs)
        {
            Debug.Log("=== DIAGNOSIS COMPLETE ===");
            Debug.Log("Press F6 to re-run diagnosis");
            Debug.Log("Press F7 to toggle safe mode");
        }
    }
    
    void ToggleSafeMode()
    {
        safeMode = !safeMode;
        
        if (safeMode)
        {
            Debug.Log("Safe Mode ENABLED - Disabling problematic components");
            DisableProblematicComponents();
        }
        else
        {
            Debug.Log("Safe Mode DISABLED - Re-enabling components");
            EnableComponents();
        }
    }
    
    void EnableComponents()
    {
        // Re-enable UIManager components
        UIManager[] uiManagers = FindObjectsByType<UIManager>(FindObjectsSortMode.None);
        foreach (UIManager manager in uiManagers)
        {
            manager.enabled = true;
            Debug.Log($"Enabled UIManager: {manager.gameObject.name}");
        }
        
        // Re-enable SettingsPanel components
        SettingsPanel[] settingsPanels = FindObjectsByType<SettingsPanel>(FindObjectsSortMode.None);
        foreach (SettingsPanel panel in settingsPanels)
        {
            panel.enabled = true;
            Debug.Log($"Enabled SettingsPanel: {panel.gameObject.name}");
        }
        
        Debug.Log("⚠️ Components re-enabled - Escape key may cause crashes again!");
    }
    
    [ContextMenu("Disable Problematic Components")]
    public void DisableProblematicComponentsContext()
    {
        DisableProblematicComponents();
    }
    
    [ContextMenu("Diagnose UI System")]
    public void DiagnoseUISystemContext()
    {
        DiagnoseUISystem();
    }
    
    [ContextMenu("Toggle Safe Mode")]
    public void ToggleSafeModeContext()
    {
        ToggleSafeMode();
    }
}