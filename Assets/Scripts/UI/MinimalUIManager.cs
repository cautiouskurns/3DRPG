using UnityEngine;

/// <summary>
/// Minimal UIManager that doesn't crash Unity
/// Use this instead of the full UIManager until the crash is fixed
/// </summary>
public class MinimalUIManager : MonoBehaviour
{
    public static MinimalUIManager Instance { get; private set; }
    
    [Header("Minimal Settings")]
    public bool enableEscapeKey = true;
    public bool enableDebugLogs = true;
    
    [Header("Simple UI")]
    public GameObject simplePanel;
    
    private bool isSettingsOpen = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("MinimalUIManager: Minimal UI system started (crash-safe)");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        CreateSimplePanel();
    }
    
    void Update()
    {
        if (enableEscapeKey && Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }
    
    void HandleEscapeKey()
    {
        try
        {
            isSettingsOpen = !isSettingsOpen;
            
            if (simplePanel != null)
            {
                simplePanel.SetActive(isSettingsOpen);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"MinimalUIManager: Settings panel {(isSettingsOpen ? "opened" : "closed")}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MinimalUIManager: Error handling escape key: {e.Message}");
            enableEscapeKey = false; // Disable to prevent further crashes
        }
    }
    
    void CreateSimplePanel()
    {
        try
        {
            // Create a simple canvas
            GameObject canvasGO = new GameObject("Simple Canvas");
            canvasGO.transform.SetParent(transform);
            
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // High priority
            
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Create simple panel
            simplePanel = new GameObject("Simple Settings Panel");
            simplePanel.transform.SetParent(canvasGO.transform, false);
            
            RectTransform rect = simplePanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.one * 0.5f;
            rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = Vector2.one * 0.5f;
            rect.sizeDelta = new Vector2(400, 200);
            
            // Add background
            UnityEngine.UI.Image bg = simplePanel.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            // Add text
            GameObject textGO = new GameObject("Info Text");
            textGO.transform.SetParent(simplePanel.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            UnityEngine.UI.Text text = textGO.AddComponent<UnityEngine.UI.Text>();
            text.text = "MINIMAL SETTINGS PANEL\n\nThis is a crash-safe fallback.\nPress Escape to close.\n\nFix the main UIManager to restore\nfull functionality.";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            // Initially hide
            simplePanel.SetActive(false);
            
            if (enableDebugLogs)
            {
                Debug.Log("MinimalUIManager: Simple panel created successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MinimalUIManager: Failed to create simple panel: {e.Message}");
            simplePanel = null;
        }
    }
    
    public void ShowSettings()
    {
        if (simplePanel != null)
        {
            simplePanel.SetActive(true);
            isSettingsOpen = true;
        }
    }
    
    public void HideSettings()
    {
        if (simplePanel != null)
        {
            simplePanel.SetActive(false);
            isSettingsOpen = false;
        }
    }
    
    [ContextMenu("Test Settings Toggle")]
    public void TestSettingsToggle()
    {
        HandleEscapeKey();
    }
}