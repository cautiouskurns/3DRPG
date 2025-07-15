using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Connects HUDController to pre-built UI elements
/// Run this after building UI in edit mode
/// </summary>
public class HUDConnector : MonoBehaviour
{
    [Header("Auto-Connect Settings")]
    public bool autoConnectOnStart = true;
    public bool enableDebugLogs = true;
    
    void Start()
    {
        if (autoConnectOnStart)
        {
            ConnectHUDElements();
        }
    }
    
    void Update()
    {
        // Manual connect hotkey
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ConnectHUDElements();
        }
    }
    
    [ContextMenu("Connect HUD Elements")]
    public void ConnectHUDElements()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== CONNECTING HUD ELEMENTS ===");
        }
        
        // Find HUDController
        HUDController hudController = FindFirstObjectByType<HUDController>();
        if (hudController == null)
        {
            Debug.LogError("HUDConnector: No HUDController found in scene");
            return;
        }
        
        // Disable auto-creation
        hudController.autoCreateUI = false;
        
        // Find and assign stat bars
        hudController.healthBar = FindSliderByName("Health Bar");
        hudController.mpBar = FindSliderByName("MP Bar");  
        hudController.xpBar = FindSliderByName("XP Bar");
        
        // Find and assign text elements
        hudController.levelText = FindTextByName("Level Text");
        hudController.interactionPromptText = FindTextByName("Interaction Prompt");
        
        // Log results
        bool allFound = true;
        
        if (hudController.healthBar == null)
        {
            Debug.LogWarning("HUDConnector: Health Bar not found");
            allFound = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("✅ Health Bar connected");
        }
        
        if (hudController.mpBar == null)
        {
            Debug.LogWarning("HUDConnector: MP Bar not found");
            allFound = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("✅ MP Bar connected");
        }
        
        if (hudController.xpBar == null)
        {
            Debug.LogWarning("HUDConnector: XP Bar not found");
            allFound = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("✅ XP Bar connected");
        }
        
        if (hudController.levelText == null)
        {
            Debug.LogWarning("HUDConnector: Level Text not found");
            allFound = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("✅ Level Text connected");
        }
        
        if (hudController.interactionPromptText == null)
        {
            Debug.LogWarning("HUDConnector: Interaction Prompt not found");
            allFound = false;
        }
        else if (enableDebugLogs)
        {
            Debug.Log("✅ Interaction Prompt connected");
        }
        
        if (allFound)
        {
            Debug.Log("🎉 All HUD elements connected successfully!");
        }
        else
        {
            Debug.LogWarning("⚠️ Some HUD elements missing - check UI Builder setup");
        }
        
        // Force HUD to re-initialize with new references
        if (Application.isPlaying)
        {
            // Trigger re-initialization
            hudController.SendMessage("InitializeHUD", SendMessageOptions.DontRequireReceiver);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("=== HUD CONNECTION COMPLETE ===");
        }
    }
    
    private Slider FindSliderByName(string name)
    {
        // First try direct find
        GameObject found = GameObject.Find(name);
        if (found != null)
        {
            Slider slider = found.GetComponent<Slider>();
            if (slider != null) return slider;
        }
        
        // Try finding by searching all sliders
        Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        foreach (Slider slider in allSliders)
        {
            if (slider.gameObject.name == name)
            {
                return slider;
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.LogWarning($"HUDConnector: Could not find slider '{name}'");
        }
        
        return null;
    }
    
    private Text FindTextByName(string name)
    {
        // First try direct find
        GameObject found = GameObject.Find(name);
        if (found != null)
        {
            Text text = found.GetComponent<Text>();
            if (text != null) return text;
        }
        
        // Try finding by searching all texts
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            if (text.gameObject.name == name)
            {
                return text;
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.LogWarning($"HUDConnector: Could not find text '{name}'");
        }
        
        return null;
    }
    
    [ContextMenu("List All UI Elements")]
    public void ListAllUIElements()
    {
        Debug.Log("=== ALL UI ELEMENTS IN SCENE ===");
        
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        Debug.Log($"Sliders found ({sliders.Length}):");
        foreach (Slider slider in sliders)
        {
            Debug.Log($"  - {slider.gameObject.name} (Path: {GetGameObjectPath(slider.gameObject)})");
        }
        
        Text[] texts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        Debug.Log($"Texts found ({texts.Length}):");
        foreach (Text text in texts)
        {
            Debug.Log($"  - {text.gameObject.name} (Path: {GetGameObjectPath(text.gameObject)})");
        }
    }
    
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}