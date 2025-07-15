using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick fix script to ensure Settings Panel is created properly with all sections
/// </summary>
public class SettingsPanelFix : MonoBehaviour
{
    [Header("Settings")]
    public bool autoFixOnStart = true;
    public bool enableDebugLogs = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            Invoke("FixSettingsPanel", 0.5f); // Small delay to let other systems initialize
        }
    }
    
    void Update()
    {
        // Fix hotkey
        if (Input.GetKeyDown(KeyCode.F8))
        {
            FixSettingsPanel();
        }
    }
    
    [ContextMenu("Fix Settings Panel")]
    public void FixSettingsPanel()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== FIXING SETTINGS PANEL ===");
        }
        
        // Get UIManager
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found!");
            return;
        }
        
        // Ensure Menu Canvas exists
        if (uiManager.menuCanvas == null)
        {
            Debug.LogError("Menu Canvas not found!");
            return;
        }
        
        // Remove existing broken settings panel if any
        SettingsPanel[] existingPanels = FindObjectsByType<SettingsPanel>(FindObjectsSortMode.None);
        foreach (SettingsPanel panel in existingPanels)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"Destroying existing SettingsPanel: {panel.gameObject.name}");
            }
            DestroyImmediate(panel.gameObject);
        }
        
        // Create new Settings Panel GameObject
        GameObject settingsPanelGO = new GameObject("Settings Panel");
        settingsPanelGO.transform.SetParent(uiManager.menuCanvas.transform, false);
        
        // Set up RectTransform
        RectTransform rect = settingsPanelGO.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.one * 0.5f;
        rect.anchorMax = Vector2.one * 0.5f;
        rect.pivot = Vector2.one * 0.5f;
        rect.sizeDelta = new Vector2(600, 500);
        rect.anchoredPosition = Vector2.zero;
        
        // Add visual components
        Image background = settingsPanelGO.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        CanvasGroup canvasGroup = settingsPanelGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Add SettingsPanel component with autoCreateUI disabled
        SettingsPanel settingsPanel = settingsPanelGO.AddComponent<SettingsPanel>();
        settingsPanel.autoCreateUI = false; // Disable auto-creation since we're handling it
        
        // Create sections manually
        CreateAudioSection(settingsPanelGO, settingsPanel);
        CreateGraphicsSection(settingsPanelGO, settingsPanel);
        CreateButtonSection(settingsPanelGO, settingsPanel);
        
        // Register with UIManager
        uiManager.settingsPanel = settingsPanel;
        uiManager.RegisterPanel("Settings", settingsPanelGO);
        uiManager.RegisterPanel("Settings Panel", settingsPanelGO);
        
        // Initially hide the panel
        settingsPanelGO.SetActive(false);
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Settings Panel fixed and registered successfully!");
            Debug.Log("Press Escape to test the Settings Panel");
        }
    }
    
    private void CreateAudioSection(GameObject parent, SettingsPanel panel)
    {
        // Create Audio Section
        GameObject audioSection = new GameObject("Audio Section");
        audioSection.transform.SetParent(parent.transform, false);
        
        RectTransform audioRect = audioSection.AddComponent<RectTransform>();
        audioRect.anchorMin = new Vector2(0, 1);
        audioRect.anchorMax = new Vector2(1, 1);
        audioRect.pivot = new Vector2(0.5f, 1);
        audioRect.sizeDelta = new Vector2(-40, 150);
        audioRect.anchoredPosition = new Vector2(0, -20);
        
        // Create Audio Title
        CreateSectionTitle(audioSection, "Audio Settings");
        
        // Create Volume Sliders
        panel.masterVolumeSlider = CreateVolumeSlider(audioSection, "Master Volume", 0);
        panel.sfxVolumeSlider = CreateVolumeSlider(audioSection, "SFX Volume", 1);
        panel.musicVolumeSlider = CreateVolumeSlider(audioSection, "Music Volume", 2);
        panel.ambientVolumeSlider = CreateVolumeSlider(audioSection, "Ambient Volume", 3);
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Audio Section created with 4 volume sliders");
        }
    }
    
    private void CreateGraphicsSection(GameObject parent, SettingsPanel panel)
    {
        // Create Graphics Section
        GameObject graphicsSection = new GameObject("Graphics Section");
        graphicsSection.transform.SetParent(parent.transform, false);
        
        RectTransform graphicsRect = graphicsSection.AddComponent<RectTransform>();
        graphicsRect.anchorMin = new Vector2(0, 1);
        graphicsRect.anchorMax = new Vector2(1, 1);
        graphicsRect.pivot = new Vector2(0.5f, 1);
        graphicsRect.sizeDelta = new Vector2(-40, 150);
        graphicsRect.anchoredPosition = new Vector2(0, -180);
        
        // Create Graphics Title
        CreateSectionTitle(graphicsSection, "Graphics Settings");
        
        // Create Graphics Controls (simplified)
        CreateSimpleText(graphicsSection, "Graphics controls placeholder", 1);
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Graphics Section created");
        }
    }
    
    private void CreateButtonSection(GameObject parent, SettingsPanel panel)
    {
        // Create Button Section
        GameObject buttonSection = new GameObject("Button Section");
        buttonSection.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonSection.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(1, 0);
        buttonRect.pivot = new Vector2(0.5f, 0);
        buttonRect.sizeDelta = new Vector2(-40, 50);
        buttonRect.anchoredPosition = new Vector2(0, 20);
        
        // Create Close Button
        panel.closeButton = CreateCloseButton(buttonSection);
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Button Section created with Close button");
        }
    }
    
    private void CreateSectionTitle(GameObject parent, string title)
    {
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(parent.transform, false);
        
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = Vector2.zero;
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = title;
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 18;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
    }
    
    private Slider CreateVolumeSlider(GameObject parent, string labelText, int index)
    {
        GameObject sliderContainer = new GameObject($"{labelText} Container");
        sliderContainer.transform.SetParent(parent.transform, false);
        
        RectTransform containerRect = sliderContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.sizeDelta = new Vector2(0, 25);
        containerRect.anchoredPosition = new Vector2(0, -40 - (30 * index));
        
        // Create label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(sliderContainer.transform, false);
        
        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.4f, 1);
        labelRect.sizeDelta = Vector2.zero;
        
        Text label = labelGO.AddComponent<Text>();
        label.text = labelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 14;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        
        // Create simple slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(sliderContainer.transform, false);
        
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.45f, 0);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.sizeDelta = Vector2.zero;
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.wholeNumbers = true;
        
        // Simple background
        Image sliderBg = sliderGO.AddComponent<Image>();
        sliderBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        return slider;
    }
    
    private void CreateSimpleText(GameObject parent, string text, int index)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.sizeDelta = new Vector2(0, 25);
        textRect.anchoredPosition = new Vector2(0, -40 - (30 * index));
        
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleLeft;
    }
    
    private Button CreateCloseButton(GameObject parent)
    {
        GameObject buttonGO = new GameObject("Close Button");
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.8f, 0);
        buttonRect.anchorMax = new Vector2(1f, 1);
        buttonRect.sizeDelta = Vector2.zero;
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Button background
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = Color.gray;
        button.targetGraphic = buttonImage;
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text text = textGO.AddComponent<Text>();
        text.text = "Close";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add close functionality
        button.onClick.AddListener(() => {
            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.HideSettingsMenu();
            }
        });
        
        return button;
    }
}