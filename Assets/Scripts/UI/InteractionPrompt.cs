using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [Header("Prompt Settings")]
    public Canvas promptCanvas;
    public Text promptText;
    public string defaultPromptText = "Press E to enter";
    
    [Header("Visual Settings")]
    public float fadeInSpeed = 5f;
    public float fadeOutSpeed = 10f;
    public bool useScreenSpace = true;
    public Vector2 screenOffset = new Vector2(0, 100); // Pixels above center
    
    [Header("Auto-Creation")]
    public bool autoCreateUI = true;
    public Font fallbackFont;
    
    private bool isVisible = false;
    private CanvasGroup canvasGroup;
    private Camera playerCamera;
    private Vector3 promptOffset = Vector3.zero;
    
    void Start()
    {
        // Initialize prompt UI
        SetupPromptUI();
        
        // Start hidden
        HidePrompt();
        
        Debug.Log("InteractionPrompt: Initialized");
    }
    
    void Update()
    {
        // Handle fade animations
        if (canvasGroup != null)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            float speed = isVisible ? fadeInSpeed : fadeOutSpeed;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
        }
        
        // Update prompt position if using world space
        if (!useScreenSpace)
        {
            UpdatePromptPosition();
        }
    }
    
    public void ShowPrompt(string customText = "")
    {
        if (!gameObject.activeInHierarchy) return;
        
        isVisible = true;
        
        // Set prompt text
        string textToShow = string.IsNullOrEmpty(customText) ? defaultPromptText : customText;
        SetPromptText(textToShow);
        
        // Enable canvas
        if (promptCanvas != null)
        {
            promptCanvas.gameObject.SetActive(true);
        }
        
        Debug.Log($"InteractionPrompt: Showing '{textToShow}'");
    }
    
    public void HidePrompt()
    {
        isVisible = false;
        
        // Disable canvas after fade out
        if (canvasGroup != null && canvasGroup.alpha <= 0.1f)
        {
            if (promptCanvas != null)
            {
                promptCanvas.gameObject.SetActive(false);
            }
        }
        
        Debug.Log("InteractionPrompt: Hidden");
    }
    
    public void SetPromptText(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }
    }
    
    private void SetupPromptUI()
    {
        // Get camera reference
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
        }
        
        // Setup canvas if not assigned
        if (promptCanvas == null && autoCreateUI)
        {
            CreatePromptUI();
        }
        
        // Setup canvas group for fading
        if (promptCanvas != null)
        {
            canvasGroup = promptCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = promptCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            
            // Configure canvas settings based on mode
            if (useScreenSpace)
            {
                promptCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                promptCanvas.sortingOrder = 100; // Ensure it renders on top
            }
            else
            {
                promptCanvas.renderMode = RenderMode.WorldSpace;
                promptCanvas.worldCamera = playerCamera;
                promptCanvas.sortingOrder = 100;
            }
        }
        
        // Validate setup
        if (promptText == null)
        {
            Debug.LogWarning("InteractionPrompt: No prompt text component assigned!");
        }
    }
    
    private void CreatePromptUI()
    {
        // Create canvas GameObject
        GameObject canvasObject = new GameObject("InteractionPromptCanvas");
        
        // Add Canvas component
        promptCanvas = canvasObject.AddComponent<Canvas>();
        
        // Add CanvasScaler
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Add GraphicRaycaster (required for UI)
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Configure based on screen space or world space
        RectTransform canvasRect = promptCanvas.GetComponent<RectTransform>();
        
        if (useScreenSpace)
        {
            // Screen space setup
            promptCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRect.sizeDelta = new Vector2(1920, 1080);
            canvasRect.localScale = Vector3.one;
            canvasRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            // World space setup
            canvasObject.transform.SetParent(transform);
            canvasObject.transform.localPosition = Vector3.up * 2f;
            promptCanvas.renderMode = RenderMode.WorldSpace;
            canvasRect.sizeDelta = new Vector2(200, 50);
            canvasRect.localScale = Vector3.one * 0.01f; // Scale down for world space
        }
        
        // Create text GameObject
        GameObject textObject = new GameObject("PromptText");
        textObject.transform.SetParent(canvasObject.transform, false);
        
        // Add Text component
        promptText = textObject.AddComponent<Text>();
        promptText.text = defaultPromptText;
        promptText.fontSize = 24;
        promptText.alignment = TextAnchor.MiddleCenter;
        promptText.color = Color.white;
        
        // Set font
        if (fallbackFont != null)
        {
            promptText.font = fallbackFont;
        }
        else
        {
            // Try to find default font
            Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (defaultFont != null)
            {
                promptText.font = defaultFont;
            }
        }
        
        // Setup text rect transform
        RectTransform textRect = promptText.GetComponent<RectTransform>();
        
        if (useScreenSpace)
        {
            // Center screen with offset
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(400, 60);
            textRect.anchoredPosition = screenOffset;
        }
        else
        {
            // Fill parent for world space
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
        }
        
        // Add background (optional)
        CreatePromptBackground(canvasObject);
        
        Debug.Log("InteractionPrompt: Auto-created UI components");
    }
    
    private void CreatePromptBackground(GameObject parent)
    {
        // Create background GameObject
        GameObject backgroundObject = new GameObject("PromptBackground");
        backgroundObject.transform.SetParent(parent.transform, false);
        
        // Add Image component for background
        Image background = backgroundObject.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
        
        // Setup background rect transform (behind text)
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
        backgroundRect.anchoredPosition = Vector2.zero;
        
        // Move background behind text
        backgroundObject.transform.SetSiblingIndex(0);
    }
    
    private void UpdatePromptPosition()
    {
        if (promptCanvas == null || playerCamera == null) return;
        
        // Make prompt face the camera
        Vector3 directionToCamera = playerCamera.transform.position - promptCanvas.transform.position;
        directionToCamera.y = 0; // Keep prompt upright
        
        if (directionToCamera != Vector3.zero)
        {
            promptCanvas.transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }
    
    // Public method to set prompt position relative to parent
    public void SetPromptOffset(Vector3 offset)
    {
        promptOffset = offset;
        if (promptCanvas != null)
        {
            promptCanvas.transform.localPosition = promptOffset;
        }
    }
    
    // Public method to check if prompt is currently visible
    public bool IsVisible()
    {
        return isVisible && (canvasGroup == null || canvasGroup.alpha > 0.1f);
    }
    
    // Public method to force immediate show/hide (no fade)
    public void SetVisibleImmediate(bool visible)
    {
        isVisible = visible;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
        }
        
        if (promptCanvas != null)
        {
            promptCanvas.gameObject.SetActive(visible);
        }
    }
    
    // Public method to update prompt appearance
    public void UpdatePromptAppearance(Color textColor, Color backgroundColor)
    {
        if (promptText != null)
        {
            promptText.color = textColor;
        }
        
        // Find background image
        Image background = promptCanvas.GetComponentInChildren<Image>();
        if (background != null)
        {
            background.color = backgroundColor;
        }
    }
}