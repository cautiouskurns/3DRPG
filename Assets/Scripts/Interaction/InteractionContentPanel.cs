using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionContentPanel : MonoBehaviour
{
    public static InteractionContentPanel Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject contentPanel;
    public Text titleText;
    public Text descriptionText;
    public Text loreText;
    public Text categoryText;
    public Button closeButton;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.2f;
    public bool autoHideAfterTime = true;
    public float autoHideDelay = 5f;
    
    [Header("Layout")]
    public bool showCategoryLabel = true;
    public bool showLoreSection = true;
    public string loreSectionTitle = "Historical Note:";
    
    private CanvasGroup panelCanvasGroup;
    private Coroutine currentAnimation;
    private Coroutine autoHideCoroutine;
    private InteractionContent currentContent;
    private bool isVisible = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeContentPanel();
        }
        else
        {
            Debug.LogWarning("InteractionContentPanel: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetupUIReferences();
        SetupEventListeners();
        HideImmediate();
    }
    
    void InitializeContentPanel()
    {
        if (contentPanel == null)
        {
            contentPanel = gameObject;
        }
        
        panelCanvasGroup = contentPanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = contentPanel.AddComponent<CanvasGroup>();
        }
    }
    
    void SetupUIReferences()
    {
        if (titleText == null)
        {
            titleText = transform.Find("Content/Title")?.GetComponent<Text>();
        }
        
        if (descriptionText == null)
        {
            descriptionText = transform.Find("Content/Description")?.GetComponent<Text>();
        }
        
        if (loreText == null)
        {
            loreText = transform.Find("Content/Lore")?.GetComponent<Text>();
        }
        
        if (categoryText == null)
        {
            categoryText = transform.Find("Content/Category")?.GetComponent<Text>();
        }
        
        if (closeButton == null)
        {
            closeButton = transform.Find("CloseButton")?.GetComponent<Button>();
        }
        
        ValidateUISetup();
    }
    
    void SetupEventListeners()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideContent);
        }
        
        EventBus.Subscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
    }
    
    void OnInteractionPromptEvent(InteractionPromptEvent eventData)
    {
        if (eventData.Action == InteractionAction.Hide && isVisible)
        {
            HideContent();
        }
    }
    
    public void ShowContent(InteractionContent content)
    {
        if (content == null || !content.IsValid())
        {
            Debug.LogWarning("InteractionContentPanel: Invalid content provided");
            return;
        }
        
        currentContent = content;
        UpdateContentText();
        ShowPanel();
        
        if (autoHideAfterTime && content.contentDisplayTime > 0f)
        {
            if (autoHideCoroutine != null)
            {
                StopCoroutine(autoHideCoroutine);
            }
            
            // Only start coroutine if GameObject is active
            if (gameObject.activeInHierarchy)
            {
                autoHideCoroutine = StartCoroutine(AutoHideAfterDelay(content.contentDisplayTime));
            }
        }
    }
    
    void UpdateContentText()
    {
        if (currentContent == null) return;
        
        if (titleText != null)
        {
            titleText.text = currentContent.title;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = currentContent.description;
        }
        
        if (loreText != null && showLoreSection)
        {
            if (!string.IsNullOrEmpty(currentContent.loreText))
            {
                string loreDisplay = showLoreSection && !string.IsNullOrEmpty(loreSectionTitle) 
                    ? $"{loreSectionTitle}\n{currentContent.loreText}"
                    : currentContent.loreText;
                
                loreText.text = loreDisplay;
                loreText.gameObject.SetActive(true);
            }
            else
            {
                loreText.gameObject.SetActive(false);
            }
        }
        
        if (categoryText != null && showCategoryLabel)
        {
            if (!string.IsNullOrEmpty(currentContent.category))
            {
                categoryText.text = $"[{currentContent.category}]";
                categoryText.gameObject.SetActive(true);
            }
            else
            {
                categoryText.gameObject.SetActive(false);
            }
        }
    }
    
    void ShowPanel()
    {
        if (isVisible) return;
        
        isVisible = true;
        
        // Ensure GameObject is active before starting coroutines
        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
        }
        
        // Make sure the parent canvas is also active
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && !parentCanvas.gameObject.activeInHierarchy)
        {
            parentCanvas.gameObject.SetActive(true);
            Debug.Log($"InteractionContentPanel: Activated parent canvas {parentCanvas.name}");
        }
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(FadeInPanel());
        
        if (InputManager.Instance != null)
        {
            InputManager.Instance.inputEnabled = false;
        }
        
        EventBus.Publish(new UIStateChangedEvent(UIState.Dialogue, true, "InteractionContent"));
    }
    
    public void HideContent()
    {
        if (!isVisible) return;
        
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(FadeOutPanel());
        
        if (InputManager.Instance != null)
        {
            InputManager.Instance.inputEnabled = true;
        }
        
        EventBus.Publish(new UIStateChangedEvent(UIState.Game, false));
    }
    
    void HideImmediate()
    {
        isVisible = false;
        contentPanel.SetActive(false);
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }
    
    IEnumerator FadeInPanel()
    {
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = true;
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            
            t = t * t * (3f - 2f * t);
            
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        currentAnimation = null;
    }
    
    IEnumerator FadeOutPanel()
    {
        float startAlpha = panelCanvasGroup.alpha;
        panelCanvasGroup.interactable = false;
        
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeOutDuration;
            
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }
        
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.blocksRaycasts = false;
        contentPanel.SetActive(false);
        isVisible = false;
        currentAnimation = null;
    }
    
    IEnumerator AutoHideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideContent();
        autoHideCoroutine = null;
    }
    
    void ValidateUISetup()
    {
        bool valid = true;
        
        if (titleText == null)
        {
            Debug.LogWarning("InteractionContentPanel: Title text not assigned");
        }
        
        if (descriptionText == null)
        {
            Debug.LogWarning("InteractionContentPanel: Description text not assigned");
            valid = false;
        }
        
        if (panelCanvasGroup == null)
        {
            Debug.LogError("InteractionContentPanel: CanvasGroup component missing");
            valid = false;
        }
        
        if (!valid)
        {
            Debug.LogError("InteractionContentPanel: UI setup incomplete - content display may not work properly");
        }
    }
    
    void Update()
    {
        if (isVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            HideContent();
        }
    }
    
    [ContextMenu("Test Show Content")]
    public void TestShowContentContext()
    {
        InteractionContent testContent = new InteractionContent
        {
            title = "Test Interaction",
            description = "This is a test interaction to verify the content panel is working correctly.",
            loreText = "Long ago, this was used for testing purposes by ancient developers.",
            category = "Debug",
            contentDisplayTime = 10f
        };
        
        ShowContent(testContent);
    }
    
    [ContextMenu("Test Hide Content")]
    public void TestHideContentContext()
    {
        HideContent();
    }
    
    void OnDestroy()
    {
        EventBus.Unsubscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideContent);
        }
    }
}