using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Static UIManager that works with pre-built UI hierarchy (no runtime creation)
/// Safe replacement for the original UIManager that was causing crashes
/// </summary>
public class StaticUIManager : MonoBehaviour
{
    public static StaticUIManager Instance { get; private set; }
    
    [Header("Pre-built UI References")]
    public Canvas hudCanvas;
    public Canvas menuCanvas;
    public Canvas overlayCanvas;
    
    [Header("Pre-built Controllers")]
    public HUDController hudController;
    public GameObject settingsPanel;
    
    [Header("Settings")]
    public float transitionDuration = 0.3f;
    public bool enableDebugLogs = true;
    
    [Header("Input Settings")]
    public bool enableEscapeKey = true;
    
    private bool isSettingsOpen = false;
    private CanvasGroup settingsPanelCanvasGroup;
    private Coroutine currentTransition;
    
    public bool IsUIMode => isSettingsOpen;
    public UIState CurrentUIState => isSettingsOpen ? UIState.Settings : UIState.Game;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("StaticUIManager: Initialized with pre-built UI (crash-safe)");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeStaticUI();
    }
    
    void Update()
    {
        if (enableEscapeKey && Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }
    
    void InitializeStaticUI()
    {
        // Find UI components if not assigned
        if (hudCanvas == null)
        {
            hudCanvas = GameObject.Find("HUD Canvas")?.GetComponent<Canvas>();
        }
        
        if (menuCanvas == null)
        {
            menuCanvas = GameObject.Find("Menu Canvas")?.GetComponent<Canvas>();
        }
        
        if (overlayCanvas == null)
        {
            overlayCanvas = GameObject.Find("Overlay Canvas")?.GetComponent<Canvas>();
        }
        
        if (hudController == null)
        {
            hudController = FindFirstObjectByType<HUDController>();
            
            // If found, ensure it's configured for static UI
            if (hudController != null)
            {
                hudController.autoCreateUI = false;
            }
        }
        
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("Settings Panel");
        }
        
        // Get CanvasGroup for settings panel
        if (settingsPanel != null)
        {
            settingsPanelCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
            if (settingsPanelCanvasGroup == null)
            {
                settingsPanelCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
            }
            
            // Ensure panel starts hidden
            settingsPanel.SetActive(false);
            settingsPanelCanvasGroup.alpha = 0f;
            settingsPanelCanvasGroup.interactable = false;
            settingsPanelCanvasGroup.blocksRaycasts = false;
        }
        
        // Setup canvas visibility
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
        }
        
        if (overlayCanvas != null)
        {
            overlayCanvas.gameObject.SetActive(false);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"StaticUIManager: UI initialized - HUD: {hudCanvas != null}, Menu: {menuCanvas != null}, Settings: {settingsPanel != null}");
        }
    }
    
    void HandleEscapeKey()
    {
        try
        {
            if (isSettingsOpen)
            {
                HideSettingsMenu();
            }
            else
            {
                ShowSettingsMenu();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"StaticUIManager: Error handling escape key: {e.Message}");
            enableEscapeKey = false; // Disable to prevent further crashes
        }
    }
    
    public void ShowSettingsMenu()
    {
        if (settingsPanel == null || isSettingsOpen) return;
        
        try
        {
            isSettingsOpen = true;
            
            // Show menu canvas
            if (menuCanvas != null)
            {
                menuCanvas.gameObject.SetActive(true);
            }
            
            // Show settings panel
            settingsPanel.SetActive(true);
            
            // Animate in
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }
            currentTransition = StartCoroutine(FadePanel(true));
            
            // Disable game input
            InputManager inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.inputEnabled = false;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("StaticUIManager: Settings menu shown");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"StaticUIManager: Error showing settings menu: {e.Message}");
        }
    }
    
    public void HideSettingsMenu()
    {
        if (settingsPanel == null || !isSettingsOpen) return;
        
        try
        {
            isSettingsOpen = false;
            
            // Animate out
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }
            currentTransition = StartCoroutine(FadePanel(false));
            
            // Enable game input
            InputManager inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.inputEnabled = true;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("StaticUIManager: Settings menu hidden");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"StaticUIManager: Error hiding settings menu: {e.Message}");
        }
    }
    
    IEnumerator FadePanel(bool fadeIn)
    {
        if (settingsPanelCanvasGroup == null) yield break;
        
        float startAlpha = settingsPanelCanvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        
        if (fadeIn)
        {
            settingsPanelCanvasGroup.interactable = true;
            settingsPanelCanvasGroup.blocksRaycasts = true;
        }
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionDuration;
            
            // Smooth step easing
            t = t * t * (3f - 2f * t);
            
            settingsPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        settingsPanelCanvasGroup.alpha = targetAlpha;
        
        if (!fadeIn)
        {
            settingsPanelCanvasGroup.interactable = false;
            settingsPanelCanvasGroup.blocksRaycasts = false;
            settingsPanel.SetActive(false);
            
            if (menuCanvas != null)
            {
                menuCanvas.gameObject.SetActive(false);
            }
        }
    }
    
    // Public methods for external control
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
    
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
    
    public void SetUIMode(bool enabled)
    {
        if (enabled && !isSettingsOpen)
        {
            ShowSettingsMenu();
        }
        else if (!enabled && isSettingsOpen)
        {
            HideSettingsMenu();
        }
    }
    
    // Validation
    public bool ValidateUISetup()
    {
        bool valid = true;
        
        if (hudCanvas == null)
        {
            Debug.LogWarning("StaticUIManager: HUD Canvas not assigned or found");
        }
        
        if (menuCanvas == null)
        {
            Debug.LogWarning("StaticUIManager: Menu Canvas not assigned or found");
            valid = false;
        }
        
        if (settingsPanel == null)
        {
            Debug.LogWarning("StaticUIManager: Settings Panel not assigned or found");
            valid = false;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"StaticUIManager: Validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
    
    // Context menu methods for testing
    [ContextMenu("Show Settings")]
    public void ShowSettingsContext()
    {
        ShowSettingsMenu();
    }
    
    [ContextMenu("Hide Settings")]
    public void HideSettingsContext()
    {
        HideSettingsMenu();
    }
    
    [ContextMenu("Toggle Settings")]
    public void ToggleSettingsContext()
    {
        if (isSettingsOpen)
        {
            HideSettingsMenu();
        }
        else
        {
            ShowSettingsMenu();
        }
    }
    
    [ContextMenu("Validate UI")]
    public void ValidateUIContext()
    {
        ValidateUISetup();
    }
}