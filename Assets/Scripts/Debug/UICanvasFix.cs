using UnityEngine;

[System.Serializable]
public class UICanvasFix : MonoBehaviour
{
    [Header("Canvas Management")]
    public bool ensureMenuCanvasActive = true;
    public bool ensureOverlayCanvasActive = true;
    public bool enableDebugLogs = true;
    
    [Header("Hotkey")]
    public KeyCode fixCanvasesHotkey = KeyCode.F8;
    
    void Start()
    {
        if (ensureMenuCanvasActive || ensureOverlayCanvasActive)
        {
            Invoke(nameof(FixCanvases), 0.5f); // Small delay to ensure other systems initialize first
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(fixCanvasesHotkey))
        {
            FixCanvases();
        }
    }
    
    [ContextMenu("Fix Canvases")]
    public void FixCanvases()
    {
        if (enableDebugLogs)
        {
            Debug.Log("UICanvasFix: Checking and fixing canvas states...");
        }
        
        // Fix Menu Canvas
        if (ensureMenuCanvasActive)
        {
            GameObject menuCanvas = GameObject.Find("Menu Canvas");
            if (menuCanvas != null)
            {
                if (!menuCanvas.activeInHierarchy)
                {
                    menuCanvas.SetActive(true);
                    if (enableDebugLogs)
                    {
                        Debug.Log("✅ UICanvasFix: Activated Menu Canvas");
                    }
                }
                else if (enableDebugLogs)
                {
                    Debug.Log("✅ UICanvasFix: Menu Canvas already active");
                }
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning("⚠️ UICanvasFix: Menu Canvas not found");
            }
        }
        
        // Fix Overlay Canvas
        if (ensureOverlayCanvasActive)
        {
            GameObject overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas != null)
            {
                if (!overlayCanvas.activeInHierarchy)
                {
                    overlayCanvas.SetActive(true);
                    if (enableDebugLogs)
                    {
                        Debug.Log("✅ UICanvasFix: Activated Overlay Canvas");
                    }
                }
                else if (enableDebugLogs)
                {
                    Debug.Log("✅ UICanvasFix: Overlay Canvas already active");
                }
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning("⚠️ UICanvasFix: Overlay Canvas not found");
            }
        }
        
        // Validate interaction UI components
        ValidateInteractionUI();
        
        if (enableDebugLogs)
        {
            Debug.Log("UICanvasFix: Canvas check completed");
        }
    }
    
    void ValidateInteractionUI()
    {
        // Check InteractionPrompt
        InteractionPrompt prompt = FindFirstObjectByType<InteractionPrompt>();
        if (prompt != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"✅ UICanvasFix: InteractionPrompt found on {prompt.gameObject.name}");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("⚠️ UICanvasFix: InteractionPrompt not found");
        }
        
        // Check InteractionContentPanel
        InteractionContentPanel contentPanel = FindFirstObjectByType<InteractionContentPanel>();
        if (contentPanel != null)
        {
            Canvas parentCanvas = contentPanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                if (!parentCanvas.gameObject.activeInHierarchy)
                {
                    parentCanvas.gameObject.SetActive(true);
                    if (enableDebugLogs)
                    {
                        Debug.Log($"✅ UICanvasFix: Activated parent canvas {parentCanvas.name} for InteractionContentPanel");
                    }
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"✅ UICanvasFix: InteractionContentPanel found in {parentCanvas.name}");
                }
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("⚠️ UICanvasFix: InteractionContentPanel not found");
        }
    }
    
    [ContextMenu("List All Canvases")]
    public void ListAllCanvases()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        Debug.Log($"=== CANVAS LIST ({allCanvases.Length} found) ===");
        
        foreach (Canvas canvas in allCanvases)
        {
            string status = canvas.gameObject.activeInHierarchy ? "ACTIVE" : "INACTIVE";
            Debug.Log($"  - {canvas.name}: {status} (Order: {canvas.sortingOrder})");
        }
    }
    
    [ContextMenu("Test Interaction Content Panel")]
    public void TestInteractionContentPanel()
    {
        if (Application.isPlaying)
        {
            InteractionContent testContent = new InteractionContent
            {
                title = "UI Canvas Fix Test",
                description = "This is a test to verify that the InteractionContentPanel can display content properly after fixing canvas issues.",
                loreText = "The ancient UICanvasFix spell was cast to ensure all canvases remain active and ready for interaction.",
                category = "Debug",
                contentDisplayTime = 8f
            };
            
            if (InteractionContentPanel.Instance != null)
            {
                InteractionContentPanel.Instance.ShowContent(testContent);
                Debug.Log("✅ UICanvasFix: Test content panel triggered");
            }
            else
            {
                Debug.LogWarning("❌ UICanvasFix: InteractionContentPanel instance not found");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ UICanvasFix: Enter Play mode to test content panel");
        }
    }
}