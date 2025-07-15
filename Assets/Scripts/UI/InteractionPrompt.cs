using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject promptPanel;
    public Text promptText;
    public Image promptBackground;
    public CanvasGroup promptCanvasGroup;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.2f;
    public float fadeOutDuration = 0.15f;
    public Vector2 showScale = Vector2.one;
    public Vector2 hideScale = new Vector2(0.8f, 0.8f);
    
    [Header("Positioning")]
    public bool followPlayer = true;
    public Vector3 worldOffset = new Vector3(0, 2, 0);
    public bool useScreenPosition = false;
    public Vector2 screenPosition = new Vector2(0.5f, 0.8f);
    
    [Header("Visual Style")]
    public Color defaultTextColor = Color.white;
    public Color highlightTextColor = Color.yellow;
    public Color backgroundColor = new Color(0, 0, 0, 0.7f);
    
    [Header("Legacy Compatibility")]
    public Canvas promptCanvas;
    public string defaultPromptText = "Press E to enter";
    public float fadeInSpeed = 5f;
    public float fadeOutSpeed = 10f;
    public bool useScreenSpace = true;
    public Vector2 screenOffset = new Vector2(0, 100);
    public bool autoCreateUI = true;
    public Font fallbackFont;
    
    private Transform playerTransform;
    private Camera playerCamera;
    private Coroutine currentAnimation;
    private bool isVisible = false;
    private string currentPromptText = "";
    private CanvasGroup canvasGroup;
    private Vector3 promptOffset = Vector3.zero;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePrompt();
        }
        else
        {
            Debug.LogWarning("InteractionPrompt: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetupReferences();
        SubscribeToEvents();
        HidePromptImmediate();
        
        // Legacy compatibility
        SetupPromptUI();
        HidePrompt();
        
        Debug.Log("InteractionPrompt: Initialized");
    }
    
    void Update()
    {
        // New system: Update world position following
        if (isVisible && followPlayer && !useScreenPosition)
        {
            UpdateWorldPosition();
        }
        
        // Legacy system: Handle fade animations
        if (canvasGroup != null)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            float speed = isVisible ? fadeInSpeed : fadeOutSpeed;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
        }
        
        // Legacy system: Update prompt position if using world space
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
    
    // New interaction system methods
    void InitializePrompt()
    {
        if (promptPanel == null)
        {
            promptPanel = gameObject;
        }
        
        if (promptCanvasGroup == null)
        {
            promptCanvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
            {
                promptCanvasGroup = promptPanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    void SetupReferences()
    {
        FindPlayerReferences();
        FindUIComponents();
        ApplyDefaultStyling();
    }
    
    void FindPlayerReferences()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                PlayerController controller = FindFirstObjectByType<PlayerController>();
                if (controller != null)
                {
                    playerTransform = controller.transform;
                }
            }
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindFirstObjectByType<Camera>();
            }
        }
    }
    
    void FindUIComponents()
    {
        if (promptText == null)
        {
            promptText = GetComponentInChildren<Text>();
        }
        
        if (promptBackground == null)
        {
            promptBackground = GetComponent<Image>();
        }
    }
    
    void ApplyDefaultStyling()
    {
        if (promptText != null)
        {
            promptText.color = defaultTextColor;
            promptText.text = "";
        }
        
        if (promptBackground != null)
        {
            promptBackground.color = backgroundColor;
        }
    }
    
    void SubscribeToEvents()
    {
        EventBus.Subscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
    }
    
    void OnInteractionPromptEvent(InteractionPromptEvent eventData)
    {
        switch (eventData.Action)
        {
            case InteractionAction.Show:
                ShowPromptNew(eventData.PromptText, eventData.Type);
                break;
            case InteractionAction.Hide:
                HidePromptNew();
                break;
            case InteractionAction.Update:
                UpdatePromptNew(eventData.PromptText, eventData.Type);
                break;
        }
    }
    
    public void ShowPromptNew(string text, InteractionType type = InteractionType.General)
    {
        if (string.IsNullOrEmpty(text))
        {
            HidePromptNew();
            return;
        }
        
        currentPromptText = text;
        
        if (promptText != null)
        {
            promptText.text = text;
            promptText.color = GetColorForInteractionType(type);
        }
        
        if (!isVisible)
        {
            isVisible = true;
            promptPanel.SetActive(true);
            
            if (useScreenPosition)
            {
                SetScreenPosition();
            }
            else
            {
                UpdateWorldPosition();
            }
            
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }
            currentAnimation = StartCoroutine(AnimateShow());
        }
    }
    
    public void UpdatePromptNew(string text, InteractionType type = InteractionType.General)
    {
        if (isVisible && promptText != null)
        {
            currentPromptText = text;
            promptText.text = text;
            promptText.color = GetColorForInteractionType(type);
        }
        else
        {
            ShowPromptNew(text, type);
        }
    }
    
    public void HidePromptNew()
    {
        if (!isVisible) return;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateHide());
    }
    
    void HidePromptImmediate()
    {
        isVisible = false;
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
        }
        transform.localScale = hideScale;
        currentPromptText = "";
    }
    
    Color GetColorForInteractionType(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Door:
                return new Color(0.8f, 0.8f, 1f);
            case InteractionType.NPC:
                return new Color(1f, 1f, 0.8f);
            case InteractionType.Item:
                return new Color(0.8f, 1f, 0.8f);
            case InteractionType.Combat:
                return new Color(1f, 0.8f, 0.8f);
            case InteractionType.Dialogue:
                return highlightTextColor;
            default:
                return defaultTextColor;
        }
    }
    
    void UpdateWorldPosition()
    {
        if (playerTransform == null || playerCamera == null) return;
        
        Vector3 worldPosition = playerTransform.position + worldOffset;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);
        
        if (screenPosition.z > 0)
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.position = screenPosition;
            }
        }
    }
    
    void SetScreenPosition()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform promptRect = transform as RectTransform;
        
        if (canvasRect != null && promptRect != null)
        {
            Vector2 canvasSize = canvasRect.rect.size;
            Vector2 targetPosition = new Vector2(
                canvasSize.x * screenPosition.x,
                canvasSize.y * screenPosition.y
            );
            
            promptRect.anchoredPosition = targetPosition - canvasSize * 0.5f;
        }
    }
    
    IEnumerator AnimateShow()
    {
        if (promptCanvasGroup == null) yield break;
        
        promptCanvasGroup.alpha = 0f;
        transform.localScale = hideScale;
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            
            t = t * t * (3f - 2f * t);
            
            promptCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            transform.localScale = Vector3.Lerp(hideScale, showScale, t);
            
            yield return null;
        }
        
        promptCanvasGroup.alpha = 1f;
        transform.localScale = showScale;
        currentAnimation = null;
    }
    
    IEnumerator AnimateHide()
    {
        if (promptCanvasGroup == null) yield break;
        
        float startAlpha = promptCanvasGroup.alpha;
        Vector3 startScale = transform.localScale;
        
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeOutDuration;
            
            promptCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            transform.localScale = Vector3.Lerp(startScale, hideScale, t);
            
            yield return null;
        }
        
        HidePromptImmediate();
        currentAnimation = null;
    }
    
    void OnDestroy()
    {
        EventBus.Unsubscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
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