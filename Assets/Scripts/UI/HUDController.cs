using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Stat Bars")]
    public Slider healthBar;
    public Slider mpBar;
    public Slider xpBar;
    
    [Header("Display Elements")]
    public Text levelText;
    public Text interactionPromptText;
    
    [Header("Bar Colors")]
    public Color healthColor = Color.green;
    public Color mpColor = Color.blue;
    public Color xpColor = Color.yellow;
    public Color healthBackgroundColor = new Color(0.2f, 0.1f, 0.1f, 0.8f);
    public Color mpBackgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.8f);
    public Color xpBackgroundColor = new Color(0.2f, 0.2f, 0.1f, 0.8f);
    
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Interaction Prompt Settings")]
    public float promptFadeSpeed = 3f;
    public Vector2 promptOffset = new Vector2(0, 100);
    
    [Header("Auto-Creation Settings")]
    public bool autoCreateUI = true;
    public Vector2 hudSize = new Vector2(300, 20);
    public Vector2 hudSpacing = new Vector2(10, 30);
    public Vector2 hudOffset = new Vector2(20, 20);
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    // Current stat values for animation
    private float currentHealthPercent = 1f;
    private float currentMPPercent = 1f;
    private float currentXPPercent = 0f;
    private int currentLevel = 1;
    
    // Animation coroutines
    private Coroutine healthAnimation;
    private Coroutine mpAnimation;
    private Coroutine xpAnimation;
    private Coroutine promptAnimation;
    
    // Interaction prompt state
    private bool isPromptVisible = false;
    private string currentPromptText = "";
    
    void Start()
    {
        InitializeHUD();
    }
    
    private void InitializeHUD()
    {
        if (autoCreateUI)
        {
            CreateHUDElements();
        }
        else
        {
            // Find pre-built UI elements
            FindPreBuiltUIElements();
        }
        
        SetupHUDElements();
        InitializeStatBars();
        
        if (enableDebugLogs)
        {
            Debug.Log("HUDController: HUD system initialized");
        }
    }
    
    private void CreateHUDElements()
    {
        Canvas hudCanvas = FindHUDCanvas();
        if (hudCanvas == null)
        {
            Debug.LogError("HUDController: No HUD Canvas found for auto-creation");
            return;
        }
        
        // Create main HUD container
        GameObject hudContainer = new GameObject("HUD Container");
        hudContainer.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform containerRect = hudContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.anchoredPosition = hudOffset;
        
        // Create stat bars
        healthBar = CreateStatBar(hudContainer, "Health Bar", 0, healthColor, healthBackgroundColor);
        mpBar = CreateStatBar(hudContainer, "MP Bar", 1, mpColor, mpBackgroundColor);
        xpBar = CreateStatBar(hudContainer, "XP Bar", 2, xpColor, xpBackgroundColor);
        
        // Create level text
        levelText = CreateLevelText(hudContainer);
        
        // Create interaction prompt
        interactionPromptText = CreateInteractionPrompt(hudCanvas);
        
        if (enableDebugLogs)
        {
            Debug.Log("HUDController: UI elements auto-created");
        }
    }
    
    private Canvas FindHUDCanvas()
    {
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null && uiManager.hudCanvas != null)
        {
            return uiManager.hudCanvas;
        }
        
        // Fallback: find canvas with lowest sort order
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Canvas hudCanvas = null;
        int lowestOrder = int.MaxValue;
        
        foreach (Canvas canvas in canvases)
        {
            if (canvas.sortingOrder < lowestOrder)
            {
                lowestOrder = canvas.sortingOrder;
                hudCanvas = canvas;
            }
        }
        
        return hudCanvas;
    }
    
    private void FindPreBuiltUIElements()
    {
        // Find stat bars by name
        healthBar = FindSliderByName("Health Bar");
        mpBar = FindSliderByName("MP Bar");
        xpBar = FindSliderByName("XP Bar");
        
        // Find level text
        levelText = FindTextByName("Level Text");
        
        // Find interaction prompt
        interactionPromptText = FindTextByName("Interaction Prompt");
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Found pre-built elements - Health: {healthBar != null}, MP: {mpBar != null}, XP: {xpBar != null}, Level: {levelText != null}, Prompt: {interactionPromptText != null}");
        }
    }
    
    private Slider FindSliderByName(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found != null)
        {
            return found.GetComponent<Slider>();
        }
        
        // Try finding in children if direct find fails
        Slider[] allSliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        foreach (Slider slider in allSliders)
        {
            if (slider.gameObject.name == name)
            {
                return slider;
            }
        }
        
        return null;
    }
    
    private Text FindTextByName(string name)
    {
        GameObject found = GameObject.Find(name);
        if (found != null)
        {
            return found.GetComponent<Text>();
        }
        
        // Try finding in children if direct find fails
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            if (text.gameObject.name == name)
            {
                return text;
            }
        }
        
        return null;
    }
    
    private Slider CreateStatBar(GameObject parent, string name, int index, Color fillColor, Color backgroundColor)
    {
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent.transform, false);
        
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.sizeDelta = hudSize;
        sliderRect.anchoredPosition = new Vector2(0, -(hudSize.y + hudSpacing.y) * index);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = index == 2 ? 0f : 1f; // XP starts at 0, health/MP at full
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderGO.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = backgroundColor;
        bgImage.type = Image.Type.Sliced;
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.anchoredPosition = Vector2.zero;
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Sliced;
        
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        
        return slider;
    }
    
    private Text CreateLevelText(GameObject parent)
    {
        GameObject textGO = new GameObject("Level Text");
        textGO.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        textRect.anchoredPosition = new Vector2(hudSize.x + 20, -hudSize.y / 2);
        
        Text text = textGO.AddComponent<Text>();
        text.text = "Level 1";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        
        // Add outline for better readability
        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);
        
        return text;
    }
    
    private Text CreateInteractionPrompt(Canvas canvas)
    {
        GameObject promptGO = new GameObject("Interaction Prompt");
        promptGO.transform.SetParent(canvas.transform, false);
        
        RectTransform promptRect = promptGO.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.sizeDelta = new Vector2(400, 50);
        promptRect.anchoredPosition = promptOffset;
        
        Text text = promptGO.AddComponent<Text>();
        text.text = "";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add outline for better readability
        Outline outline = promptGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, 2);
        
        // Add CanvasGroup for fading
        CanvasGroup canvasGroup = promptGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        
        promptGO.SetActive(false);
        
        return text;
    }
    
    private void SetupHUDElements()
    {
        // Validate required elements
        if (healthBar == null || mpBar == null || xpBar == null)
        {
            Debug.LogWarning("HUDController: Some stat bars are missing");
        }
        
        if (levelText == null)
        {
            Debug.LogWarning("HUDController: Level text is missing");
        }
        
        if (interactionPromptText == null)
        {
            Debug.LogWarning("HUDController: Interaction prompt text is missing");
        }
        
        // Set initial values
        if (healthBar != null) healthBar.value = currentHealthPercent;
        if (mpBar != null) mpBar.value = currentMPPercent;
        if (xpBar != null) xpBar.value = currentXPPercent;
        if (levelText != null) levelText.text = $"Level {currentLevel}";
    }
    
    private void InitializeStatBars()
    {
        // Apply colors to existing bars
        ApplyBarColors();
        
        // Set initial values from character system (placeholder values)
        UpdateHealth(100f, 100f);
        UpdateMP(50f, 100f);
        UpdateXP(0f, 100f);
        UpdateLevel(1);
    }
    
    private void ApplyBarColors()
    {
        if (healthBar != null && healthBar.fillRect != null)
        {
            Image fillImage = healthBar.fillRect.GetComponent<Image>();
            if (fillImage != null) fillImage.color = healthColor;
        }
        
        if (mpBar != null && mpBar.fillRect != null)
        {
            Image fillImage = mpBar.fillRect.GetComponent<Image>();
            if (fillImage != null) fillImage.color = mpColor;
        }
        
        if (xpBar != null && xpBar.fillRect != null)
        {
            Image fillImage = xpBar.fillRect.GetComponent<Image>();
            if (fillImage != null) fillImage.color = xpColor;
        }
    }
    
    // Public stat update methods
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar == null) return;
        
        float targetPercent = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        targetPercent = Mathf.Clamp01(targetPercent);
        
        if (healthAnimation != null)
        {
            StopCoroutine(healthAnimation);
        }
        
        healthAnimation = StartCoroutine(AnimateBar(healthBar, targetPercent, () => {
            currentHealthPercent = targetPercent;
        }));
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Health updated - {currentHealth}/{maxHealth} ({targetPercent:P1})");
        }
    }
    
    public void UpdateMP(float currentMP, float maxMP)
    {
        if (mpBar == null) return;
        
        float targetPercent = maxMP > 0 ? currentMP / maxMP : 0f;
        targetPercent = Mathf.Clamp01(targetPercent);
        
        if (mpAnimation != null)
        {
            StopCoroutine(mpAnimation);
        }
        
        mpAnimation = StartCoroutine(AnimateBar(mpBar, targetPercent, () => {
            currentMPPercent = targetPercent;
        }));
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: MP updated - {currentMP}/{maxMP} ({targetPercent:P1})");
        }
    }
    
    public void UpdateXP(float currentXP, float xpToNext)
    {
        if (xpBar == null) return;
        
        float targetPercent = xpToNext > 0 ? currentXP / xpToNext : 0f;
        targetPercent = Mathf.Clamp01(targetPercent);
        
        if (xpAnimation != null)
        {
            StopCoroutine(xpAnimation);
        }
        
        xpAnimation = StartCoroutine(AnimateBar(xpBar, targetPercent, () => {
            currentXPPercent = targetPercent;
        }));
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: XP updated - {currentXP}/{xpToNext} ({targetPercent:P1})");
        }
    }
    
    public void UpdateLevel(int level)
    {
        if (levelText == null) return;
        
        currentLevel = level;
        levelText.text = $"Level {level}";
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Level updated to {level}");
        }
    }
    
    // Interaction prompt methods
    public void ShowInteractionPrompt(string message)
    {
        if (interactionPromptText == null) return;
        
        currentPromptText = message;
        interactionPromptText.text = message;
        
        if (!isPromptVisible)
        {
            isPromptVisible = true;
            
            if (promptAnimation != null)
            {
                StopCoroutine(promptAnimation);
            }
            
            promptAnimation = StartCoroutine(FadePrompt(true));
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Interaction prompt shown - '{message}'");
        }
    }
    
    public void HideInteractionPrompt()
    {
        if (interactionPromptText == null) return;
        
        if (isPromptVisible)
        {
            isPromptVisible = false;
            
            if (promptAnimation != null)
            {
                StopCoroutine(promptAnimation);
            }
            
            promptAnimation = StartCoroutine(FadePrompt(false));
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("HUDController: Interaction prompt hidden");
        }
    }
    
    public void UpdateInteractionPrompt(string message)
    {
        if (interactionPromptText == null) return;
        
        currentPromptText = message;
        interactionPromptText.text = message;
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Interaction prompt updated - '{message}'");
        }
    }
    
    // Animation coroutines
    private IEnumerator AnimateBar(Slider bar, float targetValue, System.Action onComplete = null)
    {
        if (bar == null) yield break;
        
        float startValue = bar.value;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            
            // Apply animation curve
            float curveValue = animationCurve.Evaluate(t);
            bar.value = Mathf.Lerp(startValue, targetValue, curveValue);
            
            yield return null;
        }
        
        bar.value = targetValue;
        onComplete?.Invoke();
    }
    
    private IEnumerator FadePrompt(bool fadeIn)
    {
        if (interactionPromptText == null) yield break;
        
        CanvasGroup canvasGroup = interactionPromptText.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        
        if (fadeIn)
        {
            interactionPromptText.gameObject.SetActive(true);
        }
        
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;
        float duration = 1f / promptFadeSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        
        if (!fadeIn)
        {
            interactionPromptText.gameObject.SetActive(false);
            currentPromptText = "";
        }
    }
    
    // EventBus integration
    private void OnEnable()
    {
        EventBus.Subscribe<CharacterStatsUpdatedEvent>(OnCharacterStatsUpdated);
        EventBus.Subscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<CharacterStatsUpdatedEvent>(OnCharacterStatsUpdated);
        EventBus.Unsubscribe<InteractionPromptEvent>(OnInteractionPromptEvent);
    }
    
    private void OnCharacterStatsUpdated(CharacterStatsUpdatedEvent evt)
    {
        UpdateHealth(evt.CurrentHealth, evt.MaxHealth);
        UpdateMP(evt.CurrentMP, evt.MaxMP);
        UpdateXP(evt.CurrentXP, evt.XPToNext);
        UpdateLevel(evt.Level);
    }
    
    private void OnInteractionPromptEvent(InteractionPromptEvent evt)
    {
        switch (evt.Action)
        {
            case InteractionAction.Show:
                ShowInteractionPrompt(evt.PromptText);
                break;
            case InteractionAction.Hide:
                HideInteractionPrompt();
                break;
            case InteractionAction.Update:
                UpdateInteractionPrompt(evt.PromptText);
                break;
        }
    }
    
    // Test methods for demonstration
    [ContextMenu("Test Health Damage")]
    public void TestHealthDamage()
    {
        UpdateHealth(currentHealthPercent * 100f - 25f, 100f);
    }
    
    [ContextMenu("Test MP Usage")]
    public void TestMPUsage()
    {
        UpdateMP(currentMPPercent * 100f - 20f, 100f);
    }
    
    [ContextMenu("Test XP Gain")]
    public void TestXPGain()
    {
        float newXP = (currentXPPercent * 100f + 30f) % 100f;
        if (newXP < currentXPPercent * 100f)
        {
            UpdateLevel(currentLevel + 1);
        }
        UpdateXP(newXP, 100f);
    }
    
    [ContextMenu("Test Interaction Prompt")]
    public void TestInteractionPrompt()
    {
        if (isPromptVisible)
        {
            HideInteractionPrompt();
        }
        else
        {
            ShowInteractionPrompt("Press E to test interaction");
        }
    }
    
    // Validation methods
    public bool ValidateHUDSetup()
    {
        bool valid = true;
        
        if (healthBar == null)
        {
            Debug.LogError("HUDController: Health bar is null");
            valid = false;
        }
        
        if (mpBar == null)
        {
            Debug.LogError("HUDController: MP bar is null");
            valid = false;
        }
        
        if (xpBar == null)
        {
            Debug.LogError("HUDController: XP bar is null");
            valid = false;
        }
        
        if (levelText == null)
        {
            Debug.LogWarning("HUDController: Level text is null");
        }
        
        if (interactionPromptText == null)
        {
            Debug.LogWarning("HUDController: Interaction prompt text is null");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"HUDController: Validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
}