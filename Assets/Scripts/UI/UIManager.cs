using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Canvas Management")]
    public Canvas hudCanvas;
    public Canvas menuCanvas;
    public Canvas overlayCanvas;
    
    [Header("Panel Controllers")]
    public HUDController hudController;
    public SettingsPanel settingsPanel;
    
    [Header("UI Settings")]
    public float transitionDuration = 0.3f;
    public bool enableDebugLogs = true;
    
    [Header("Canvas Configuration")]
    public int hudSortOrder = 100;
    public int menuSortOrder = 200;
    public int overlaySortOrder = 300;
    
    private Dictionary<string, GameObject> registeredPanels = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, CanvasGroup> panelCanvasGroups = new Dictionary<GameObject, CanvasGroup>();
    private List<Coroutine> activeTransitions = new List<Coroutine>();
    
    private bool isUIMode = false;
    private UIState currentUIState = UIState.Game;
    private InputMode currentInputMode = InputMode.Game;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("UIManager: Singleton instance created");
            }
        }
        else
        {
            Debug.LogWarning("UIManager: Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeUISystem();
    }
    
    void Update()
    {
        HandleUIInput();
    }
    
    private void InitializeUISystem()
    {
        // Create canvas hierarchy if not assigned
        SetupCanvasHierarchy();
        
        // Initialize panel controllers
        InitializePanelControllers();
        
        // Register default panels
        RegisterDefaultPanels();
        
        // Setup initial UI state
        SetUIState(UIState.Game);
        
        if (enableDebugLogs)
        {
            Debug.Log("UIManager: UI system initialized successfully");
        }
    }
    
    private void SetupCanvasHierarchy()
    {
        // HUD Canvas setup
        if (hudCanvas == null)
        {
            hudCanvas = CreateCanvas("HUD Canvas", hudSortOrder);
        }
        else
        {
            hudCanvas.sortingOrder = hudSortOrder;
        }
        
        // Menu Canvas setup
        if (menuCanvas == null)
        {
            menuCanvas = CreateCanvas("Menu Canvas", menuSortOrder);
            menuCanvas.gameObject.SetActive(false); // Start hidden
        }
        else
        {
            menuCanvas.sortingOrder = menuSortOrder;
        }
        
        // Overlay Canvas setup
        if (overlayCanvas == null)
        {
            overlayCanvas = CreateCanvas("Overlay Canvas", overlaySortOrder);
            overlayCanvas.gameObject.SetActive(false); // Start hidden
        }
        else
        {
            overlayCanvas.sortingOrder = overlaySortOrder;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: Canvas hierarchy setup - HUD({hudSortOrder}), Menu({menuSortOrder}), Overlay({overlaySortOrder})");
        }
    }
    
    private Canvas CreateCanvas(string canvasName, int sortOrder)
    {
        GameObject canvasGO = new GameObject(canvasName);
        canvasGO.transform.SetParent(transform);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        return canvas;
    }
    
    private void InitializePanelControllers()
    {
        // Initialize HUD Controller
        if (hudController == null)
        {
            hudController = FindFirstObjectByType<HUDController>();
            if (hudController == null && hudCanvas != null)
            {
                // Create HUD Controller if none exists
                GameObject hudGO = new GameObject("HUD Controller");
                hudGO.transform.SetParent(hudCanvas.transform, false);
                hudController = hudGO.AddComponent<HUDController>();
            }
        }
        
        // Initialize Settings Panel
        if (settingsPanel == null)
        {
            settingsPanel = FindFirstObjectByType<SettingsPanel>();
            if (settingsPanel == null && menuCanvas != null)
            {
                // Create Settings Panel if none exists
                GameObject settingsGO = new GameObject("Settings Panel");
                settingsGO.transform.SetParent(menuCanvas.transform, false);
                settingsPanel = settingsGO.AddComponent<SettingsPanel>();
                
                if (enableDebugLogs)
                {
                    Debug.Log("UIManager: Created new SettingsPanel component");
                }
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: Panel controllers initialized - HUD: {hudController != null}, Settings: {settingsPanel != null}");
        }
    }
    
    private void RegisterDefaultPanels()
    {
        if (settingsPanel != null)
        {
            RegisterPanel("Settings", settingsPanel.gameObject);
            RegisterPanel("Settings Panel", settingsPanel.gameObject); // Register with both names for compatibility
            
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Registered Settings Panel - {settingsPanel.gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning("UIManager: Cannot register Settings Panel - settingsPanel is null");
        }
    }
    
    private void HandleUIInput()
    {
        // Handle escape key for settings menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUIState == UIState.Game)
            {
                ShowSettingsMenu();
            }
            else if (currentUIState == UIState.Settings)
            {
                HideSettingsMenu();
            }
        }
        
        // Handle tab key for UI mode toggle (debug)
        if (Input.GetKeyDown(KeyCode.Tab) && enableDebugLogs)
        {
            ToggleUIMode();
        }
    }
    
    // Public panel management methods
    public void ShowPanel(string panelName, float duration = -1f)
    {
        if (duration < 0) duration = transitionDuration;
        
        if (registeredPanels.TryGetValue(panelName, out GameObject panel))
        {
            ShowPanel(panel, duration);
        }
        else
        {
            Debug.LogWarning($"UIManager: Panel '{panelName}' not found");
        }
    }
    
    public void ShowPanel(GameObject panel, float duration = -1f)
    {
        if (panel == null) return;
        if (duration < 0) duration = transitionDuration;
        
        StopPanelTransitions(panel);
        
        CanvasGroup canvasGroup = GetOrCreateCanvasGroup(panel);
        panel.SetActive(true);
        
        Coroutine transition = StartCoroutine(FadePanel(canvasGroup, 1f, duration, () => {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Panel '{panel.name}' shown");
            }
        }));
        
        activeTransitions.Add(transition);
        
        // Publish panel transition event
        EventBus.Publish(new PanelTransitionEvent(TransitionAction.Show, panel.name, duration));
    }
    
    public void HidePanel(string panelName, float duration = -1f)
    {
        if (duration < 0) duration = transitionDuration;
        
        if (registeredPanels.TryGetValue(panelName, out GameObject panel))
        {
            HidePanel(panel, duration);
        }
        else
        {
            Debug.LogWarning($"UIManager: Panel '{panelName}' not found");
        }
    }
    
    public void HidePanel(GameObject panel, float duration = -1f)
    {
        if (panel == null) return;
        if (duration < 0) duration = transitionDuration;
        
        StopPanelTransitions(panel);
        
        CanvasGroup canvasGroup = GetOrCreateCanvasGroup(panel);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        Coroutine transition = StartCoroutine(FadePanel(canvasGroup, 0f, duration, () => {
            panel.SetActive(false);
            
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Panel '{panel.name}' hidden");
            }
        }));
        
        activeTransitions.Add(transition);
        
        // Publish panel transition event
        EventBus.Publish(new PanelTransitionEvent(TransitionAction.Hide, panel.name, duration));
    }
    
    public void TogglePanel(string panelName, float duration = -1f)
    {
        if (registeredPanels.TryGetValue(panelName, out GameObject panel))
        {
            if (panel.activeInHierarchy)
            {
                HidePanel(panel, duration);
            }
            else
            {
                ShowPanel(panel, duration);
            }
        }
    }
    
    public void RegisterPanel(string panelName, GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning($"UIManager: Cannot register null panel '{panelName}'");
            return;
        }
        
        if (registeredPanels.ContainsKey(panelName))
        {
            Debug.LogWarning($"UIManager: Panel '{panelName}' already registered, overwriting");
        }
        
        registeredPanels[panelName] = panel;
        GetOrCreateCanvasGroup(panel); // Ensure CanvasGroup exists
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: Panel '{panelName}' registered");
        }
    }
    
    public void UnregisterPanel(string panelName)
    {
        if (registeredPanels.Remove(panelName))
        {
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Panel '{panelName}' unregistered");
            }
        }
    }
    
    // UI State Management
    public void SetUIMode(bool enabled)
    {
        if (isUIMode == enabled) return;
        
        InputMode previousMode = currentInputMode;
        isUIMode = enabled;
        currentInputMode = enabled ? InputMode.UI : InputMode.Game;
        
        // Notify InputManager about mode change
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.inputEnabled = !enabled; // Disable game input when UI is active
        }
        
        // Publish input mode change event
        EventBus.Publish(new InputModeChangedEvent(currentInputMode, previousMode));
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: UI mode {(enabled ? "enabled" : "disabled")}, Input mode: {currentInputMode}");
        }
    }
    
    public void SetUIState(UIState newState)
    {
        if (currentUIState == newState) return;
        
        UIState previousState = currentUIState;
        currentUIState = newState;
        
        // Update UI mode based on state
        bool shouldBeUIMode = newState != UIState.Game;
        SetUIMode(shouldBeUIMode);
        
        // Manage canvas visibility based on state
        UpdateCanvasVisibility();
        
        // Publish UI state change event
        EventBus.Publish(new UIStateChangedEvent(newState, shouldBeUIMode));
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: UI state changed from {previousState} to {newState}");
        }
    }
    
    private void UpdateCanvasVisibility()
    {
        switch (currentUIState)
        {
            case UIState.Game:
                if (menuCanvas != null) menuCanvas.gameObject.SetActive(false);
                if (overlayCanvas != null) overlayCanvas.gameObject.SetActive(false);
                break;
                
            case UIState.Settings:
                if (menuCanvas != null) menuCanvas.gameObject.SetActive(true);
                break;
                
            case UIState.Menu:
            case UIState.Inventory:
            case UIState.Dialogue:
                if (menuCanvas != null) menuCanvas.gameObject.SetActive(true);
                break;
                
            case UIState.Combat:
                if (overlayCanvas != null) overlayCanvas.gameObject.SetActive(true);
                break;
        }
    }
    
    // Settings menu shortcuts
    public void ShowSettingsMenu()
    {
        SetUIState(UIState.Settings);
        
        // Ensure settings panel exists
        if (settingsPanel == null)
        {
            InitializePanelControllers();
            RegisterDefaultPanels();
        }
        
        if (settingsPanel != null && settingsPanel.gameObject != null)
        {
            ShowPanel(settingsPanel.gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Showing Settings Panel - {settingsPanel.gameObject.name}");
            }
        }
        else
        {
            Debug.LogError("UIManager: Cannot show settings menu - SettingsPanel is null");
        }
    }
    
    public void HideSettingsMenu()
    {
        if (settingsPanel != null && settingsPanel.gameObject != null)
        {
            HidePanel(settingsPanel.gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log($"UIManager: Hiding Settings Panel - {settingsPanel.gameObject.name}");
            }
        }
        SetUIState(UIState.Game);
    }
    
    public void ToggleUIMode()
    {
        SetUIMode(!isUIMode);
    }
    
    // Helper methods
    private CanvasGroup GetOrCreateCanvasGroup(GameObject panel)
    {
        if (panelCanvasGroups.TryGetValue(panel, out CanvasGroup existingGroup))
        {
            return existingGroup;
        }
        
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        
        panelCanvasGroups[panel] = canvasGroup;
        return canvasGroup;
    }
    
    private void StopPanelTransitions(GameObject panel)
    {
        // Stop any active transitions for this panel
        for (int i = activeTransitions.Count - 1; i >= 0; i--)
        {
            if (activeTransitions[i] == null)
            {
                activeTransitions.RemoveAt(i);
            }
        }
    }
    
    private IEnumerator FadePanel(CanvasGroup canvasGroup, float targetAlpha, float duration, System.Action onComplete = null)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            // Smooth step for more natural easing
            t = t * t * (3f - 2f * t);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
    
    // Event subscriptions
    private void OnEnable()
    {
        EventBus.Subscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
        EventBus.Subscribe<PanelTransitionEvent>(OnPanelTransitionEvent);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
        EventBus.Unsubscribe<PanelTransitionEvent>(OnPanelTransitionEvent);
    }
    
    private void OnInteractionPromptEvent(InteractionPromptEvent evt)
    {
        // Handle interaction prompts through HUD controller
        if (hudController != null)
        {
            switch (evt.Action)
            {
                case InteractionAction.Show:
                    hudController.ShowInteractionPrompt(evt.PromptText);
                    break;
                case InteractionAction.Hide:
                    hudController.HideInteractionPrompt();
                    break;
                case InteractionAction.Update:
                    hudController.UpdateInteractionPrompt(evt.PromptText);
                    break;
            }
        }
    }
    
    private void OnPanelTransitionEvent(PanelTransitionEvent evt)
    {
        // Handle external panel transition requests
        switch (evt.Action)
        {
            case TransitionAction.Show:
                ShowPanel(evt.PanelName, evt.Duration);
                break;
            case TransitionAction.Hide:
                HidePanel(evt.PanelName, evt.Duration);
                break;
            case TransitionAction.Toggle:
                TogglePanel(evt.PanelName, evt.Duration);
                break;
        }
    }
    
    // Public getters
    public bool IsUIMode => isUIMode;
    public UIState CurrentUIState => currentUIState;
    public InputMode CurrentInputMode => currentInputMode;
    
    // Validation and debug methods
    public bool ValidateUISetup()
    {
        bool valid = true;
        
        if (hudCanvas == null)
        {
            Debug.LogError("UIManager: HUD Canvas is null");
            valid = false;
        }
        
        if (menuCanvas == null)
        {
            Debug.LogError("UIManager: Menu Canvas is null");
            valid = false;
        }
        
        if (overlayCanvas == null)
        {
            Debug.LogError("UIManager: Overlay Canvas is null");
            valid = false;
        }
        
        if (hudController == null)
        {
            Debug.LogWarning("UIManager: HUD Controller is null");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"UIManager: Validation {(valid ? "PASSED" : "FAILED")}");
            Debug.Log($"  - Registered panels: {registeredPanels.Count}");
            Debug.Log($"  - Active transitions: {activeTransitions.Count}");
            Debug.Log($"  - Current state: {currentUIState}");
        }
        
        return valid;
    }
}