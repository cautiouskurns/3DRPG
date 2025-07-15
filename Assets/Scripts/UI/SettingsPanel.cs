using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsPanel : MonoBehaviour
{
    [Header("Audio Controls")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider ambientVolumeSlider;
    
    [Header("Audio Labels")]
    public Text masterVolumeLabel;
    public Text sfxVolumeLabel;
    public Text musicVolumeLabel;
    public Text ambientVolumeLabel;
    
    [Header("Graphics Controls")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public Dropdown qualityDropdown;
    
    [Header("Action Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button resetButton;
    public Button closeButton;
    
    [Header("Panel Settings")]
    public bool autoCreateUI = true;
    public Vector2 panelSize = new Vector2(600, 500);
    public Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    
    [Header("Audio Test")]
    public AudioClip testSFXClip;
    public Button testSFXButton;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    // Private variables for settings management
    private SettingsManager settingsManager;
    private List<Vector2Int> availableResolutions;
    private Dictionary<string, object> pendingSettings = new Dictionary<string, object>();
    private bool hasUnsavedChanges = false;
    
    // Original values for cancel functionality
    private float originalMasterVolume;
    private float originalSFXVolume;
    private float originalMusicVolume;
    private float originalAmbientVolume;
    private bool originalFullscreen;
    private bool originalVSync;
    private Vector2Int originalResolution;
    private int originalQuality;
    
    void Start()
    {
        InitializeSettingsPanel();
    }
    
    private void InitializeSettingsPanel()
    {
        // Get SettingsManager instance
        settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            Debug.LogError("SettingsPanel: SettingsManager not found!");
            return;
        }
        
        // Auto-create UI if needed
        if (autoCreateUI)
        {
            CreateSettingsUI();
        }
        
        // Setup UI elements
        SetupUIElements();
        
        // Load current settings
        LoadCurrentSettings();
        
        // Store original values
        StoreOriginalSettings();
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: Settings panel initialized");
        }
    }
    
    private void CreateSettingsUI()
    {
        Canvas menuCanvas = FindMenuCanvas();
        if (menuCanvas == null)
        {
            Debug.LogError("SettingsPanel: No Menu Canvas found for auto-creation");
            return;
        }
        
        // Use this gameObject as the main panel instead of creating new one
        GameObject panelGO = this.gameObject;
        
        // Ensure it's parented to the menu canvas
        if (panelGO.transform.parent != menuCanvas.transform)
        {
            panelGO.transform.SetParent(menuCanvas.transform, false);
        }
        
        // Get or add RectTransform
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = panelGO.AddComponent<RectTransform>();
        }
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = panelSize;
        
        // Add background if not present
        Image panelImage = panelGO.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = panelGO.AddComponent<Image>();
        }
        panelImage.color = panelBackgroundColor;
        
        // Add CanvasGroup for fading if not present
        CanvasGroup canvasGroup = panelGO.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panelGO.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Create sections
        CreateAudioSection(panelGO);
        CreateGraphicsSection(panelGO);
        CreateButtonSection(panelGO);
        
        // Initially hide the panel
        panelGO.SetActive(false);
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: UI auto-created");
        }
    }
    
    private Canvas FindMenuCanvas()
    {
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null && uiManager.menuCanvas != null)
        {
            return uiManager.menuCanvas;
        }
        
        // Fallback: find canvas with medium sort order
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        return canvases.Where(c => c.sortingOrder >= 200 && c.sortingOrder < 300).FirstOrDefault();
    }
    
    private void CreateAudioSection(GameObject parent)
    {
        // Create audio section container
        GameObject audioSection = CreateSection(parent, "Audio Section", 0);
        
        // Create title
        CreateSectionTitle(audioSection, "Audio Settings");
        
        // Create volume sliders
        masterVolumeSlider = CreateVolumeSlider(audioSection, "Master Volume", 0, OnMasterVolumeChanged);
        sfxVolumeSlider = CreateVolumeSlider(audioSection, "SFX Volume", 1, OnSFXVolumeChanged);
        musicVolumeSlider = CreateVolumeSlider(audioSection, "Music Volume", 2, OnMusicVolumeChanged);
        ambientVolumeSlider = CreateVolumeSlider(audioSection, "Ambient Volume", 3, OnAmbientVolumeChanged);
        
        // Create test SFX button
        testSFXButton = CreateTestButton(audioSection, "Test SFX", OnTestSFXPressed);
    }
    
    private void CreateGraphicsSection(GameObject parent)
    {
        // Create graphics section container
        GameObject graphicsSection = CreateSection(parent, "Graphics Section", 1);
        
        // Create title
        CreateSectionTitle(graphicsSection, "Graphics Settings");
        
        // Create graphics controls
        resolutionDropdown = CreateDropdown(graphicsSection, "Resolution", 0, OnResolutionChanged);
        fullscreenToggle = CreateToggle(graphicsSection, "Fullscreen", 1, OnFullscreenChanged);
        vSyncToggle = CreateToggle(graphicsSection, "VSync", 2, OnVSyncChanged);
        qualityDropdown = CreateDropdown(graphicsSection, "Quality", 3, OnQualityChanged);
    }
    
    private void CreateButtonSection(GameObject parent)
    {
        // Create button section container
        GameObject buttonSection = CreateSection(parent, "Button Section", 2);
        
        // Create action buttons
        applyButton = CreateActionButton(buttonSection, "Apply", 0, OnApplyPressed, Color.green);
        cancelButton = CreateActionButton(buttonSection, "Cancel", 1, OnCancelPressed, Color.red);
        resetButton = CreateActionButton(buttonSection, "Reset", 2, OnResetPressed, Color.yellow);
        closeButton = CreateActionButton(buttonSection, "Close", 3, OnClosePressed, Color.gray);
    }
    
    private GameObject CreateSection(GameObject parent, string sectionName, int index)
    {
        GameObject section = new GameObject(sectionName);
        section.transform.SetParent(parent.transform, false);
        
        RectTransform sectionRect = section.AddComponent<RectTransform>();
        sectionRect.anchorMin = new Vector2(0, 1);
        sectionRect.anchorMax = new Vector2(1, 1);
        sectionRect.pivot = new Vector2(0.5f, 1);
        sectionRect.sizeDelta = new Vector2(-40, 150);
        sectionRect.anchoredPosition = new Vector2(0, -20 - (160 * index));
        
        return section;
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
    
    private Slider CreateVolumeSlider(GameObject parent, string labelText, int index, UnityEngine.Events.UnityAction<float> onValueChanged)
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
        labelRect.anchorMax = new Vector2(0.3f, 1);
        labelRect.sizeDelta = Vector2.zero;
        labelRect.anchoredPosition = Vector2.zero;
        
        Text label = labelGO.AddComponent<Text>();
        label.text = labelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 14;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        
        // Store label reference
        switch (index)
        {
            case 0: masterVolumeLabel = label; break;
            case 1: sfxVolumeLabel = label; break;
            case 2: musicVolumeLabel = label; break;
            case 3: ambientVolumeLabel = label; break;
        }
        
        // Create slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(sliderContainer.transform, false);
        
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.35f, 0);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.sizeDelta = Vector2.zero;
        sliderRect.anchoredPosition = Vector2.zero;
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.wholeNumbers = true;
        
        // Add background and fill (simplified)
        CreateSliderVisuals(sliderGO, slider);
        
        // Add event listener
        slider.onValueChanged.AddListener(onValueChanged);
        
        return slider;
    }
    
    private void CreateSliderVisuals(GameObject sliderGO, Slider slider)
    {
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderGO.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.white;
        
        slider.fillRect = fillRect;
    }
    
    private Toggle CreateToggle(GameObject parent, string labelText, int index, UnityEngine.Events.UnityAction<bool> onValueChanged)
    {
        GameObject toggleContainer = new GameObject($"{labelText} Container");
        toggleContainer.transform.SetParent(parent.transform, false);
        
        RectTransform containerRect = toggleContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.sizeDelta = new Vector2(0, 25);
        containerRect.anchoredPosition = new Vector2(0, -40 - (30 * index));
        
        // Create toggle
        GameObject toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(toggleContainer.transform, false);
        
        RectTransform toggleRect = toggleGO.AddComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0, 0);
        toggleRect.anchorMax = new Vector2(1, 1);
        toggleRect.sizeDelta = Vector2.zero;
        
        Toggle toggle = toggleGO.AddComponent<Toggle>();
        toggle.isOn = true;
        
        // Create toggle visuals (simplified)
        CreateToggleVisuals(toggleGO, toggle, labelText);
        
        // Add event listener
        toggle.onValueChanged.AddListener(onValueChanged);
        
        return toggle;
    }
    
    private void CreateToggleVisuals(GameObject toggleGO, Toggle toggle, string labelText)
    {
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(toggleGO.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.5f);
        bgRect.anchorMax = new Vector2(0, 0.5f);
        bgRect.pivot = new Vector2(0, 0.5f);
        bgRect.sizeDelta = new Vector2(20, 20);
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        
        RectTransform checkRect = checkmark.AddComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.sizeDelta = Vector2.zero;
        
        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = Color.green;
        
        // Label
        GameObject label = new GameObject("Label");
        label.transform.SetParent(toggleGO.transform, false);
        
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.sizeDelta = Vector2.zero;
        labelRect.anchoredPosition = new Vector2(30, 0);
        
        Text labelTextComponent = label.AddComponent<Text>();
        labelTextComponent.text = labelText;
        labelTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelTextComponent.fontSize = 14;
        labelTextComponent.color = Color.white;
        labelTextComponent.alignment = TextAnchor.MiddleLeft;
        
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
    }
    
    private Dropdown CreateDropdown(GameObject parent, string labelText, int index, UnityEngine.Events.UnityAction<int> onValueChanged)
    {
        GameObject dropdownContainer = new GameObject($"{labelText} Container");
        dropdownContainer.transform.SetParent(parent.transform, false);
        
        RectTransform containerRect = dropdownContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.sizeDelta = new Vector2(0, 25);
        containerRect.anchoredPosition = new Vector2(0, -40 - (30 * index));
        
        // Create label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(dropdownContainer.transform, false);
        
        RectTransform labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.3f, 1);
        labelRect.sizeDelta = Vector2.zero;
        
        Text label = labelGO.AddComponent<Text>();
        label.text = labelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 14;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        
        // Create dropdown (simplified)
        GameObject dropdownGO = new GameObject("Dropdown");
        dropdownGO.transform.SetParent(dropdownContainer.transform, false);
        
        RectTransform dropdownRect = dropdownGO.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0.35f, 0);
        dropdownRect.anchorMax = new Vector2(1, 1);
        dropdownRect.sizeDelta = Vector2.zero;
        
        Dropdown dropdown = dropdownGO.AddComponent<Dropdown>();
        
        // Create dropdown visuals (simplified)
        CreateDropdownVisuals(dropdownGO, dropdown);
        
        // Add event listener
        dropdown.onValueChanged.AddListener(onValueChanged);
        
        return dropdown;
    }
    
    private void CreateDropdownVisuals(GameObject dropdownGO, Dropdown dropdown)
    {
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(dropdownGO.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Label
        GameObject label = new GameObject("Label");
        label.transform.SetParent(dropdownGO.transform, false);
        
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;
        labelRect.offsetMin = new Vector2(10, 0);
        labelRect.offsetMax = new Vector2(-25, 0);
        
        Text labelText = label.AddComponent<Text>();
        labelText.text = "Select...";
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        dropdown.targetGraphic = bgImage;
        dropdown.captionText = labelText;
        
        // Add placeholder options
        dropdown.options.Add(new Dropdown.OptionData("Option 1"));
        dropdown.options.Add(new Dropdown.OptionData("Option 2"));
    }
    
    private Button CreateActionButton(GameObject parent, string buttonText, int index, UnityEngine.Events.UnityAction onClick, Color color)
    {
        GameObject buttonGO = new GameObject($"{buttonText} Button");
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(index * 0.25f, 0);
        buttonRect.anchorMax = new Vector2((index + 1) * 0.25f, 1);
        buttonRect.sizeDelta = new Vector2(-10, 0);
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Create button visuals
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = color;
        button.targetGraphic = buttonImage;
        
        // Create button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text text = textGO.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add event listener
        button.onClick.AddListener(onClick);
        
        return button;
    }
    
    private Button CreateTestButton(GameObject parent, string buttonText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonGO = new GameObject($"{buttonText} Button");
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.8f, 1);
        buttonRect.anchorMax = new Vector2(1f, 1);
        buttonRect.pivot = new Vector2(1, 1);
        buttonRect.sizeDelta = new Vector2(0, 25);
        buttonRect.anchoredPosition = new Vector2(0, -130);
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Create button visuals
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = Color.blue;
        button.targetGraphic = buttonImage;
        
        // Create button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text text = textGO.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 12;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add event listener
        button.onClick.AddListener(onClick);
        
        return button;
    }
    
    private void SetupUIElements()
    {
        // Setup resolution dropdown
        if (resolutionDropdown != null && settingsManager != null)
        {
            PopulateResolutionDropdown();
        }
        
        // Setup quality dropdown
        if (qualityDropdown != null && settingsManager != null)
        {
            PopulateQualityDropdown();
        }
        
        // Mark settings as unchanged initially
        hasUnsavedChanges = false;
        UpdateButtonStates();
    }
    
    private void PopulateResolutionDropdown()
    {
        availableResolutions = settingsManager.GetSupportedResolutions();
        resolutionDropdown.options.Clear();
        
        foreach (Vector2Int resolution in availableResolutions)
        {
            string optionText = $"{resolution.x} x {resolution.y}";
            resolutionDropdown.options.Add(new Dropdown.OptionData(optionText));
        }
        
        resolutionDropdown.RefreshShownValue();
    }
    
    private void PopulateQualityDropdown()
    {
        string[] qualityLevels = settingsManager.GetQualityLevels();
        qualityDropdown.options.Clear();
        
        foreach (string quality in qualityLevels)
        {
            qualityDropdown.options.Add(new Dropdown.OptionData(quality));
        }
        
        qualityDropdown.RefreshShownValue();
    }
    
    private void LoadCurrentSettings()
    {
        if (settingsManager == null) return;
        
        // Load audio settings
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = settingsManager.masterVolume;
            UpdateVolumeLabel(masterVolumeLabel, settingsManager.masterVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = settingsManager.sfxVolume;
            UpdateVolumeLabel(sfxVolumeLabel, settingsManager.sfxVolume);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = settingsManager.musicVolume;
            UpdateVolumeLabel(musicVolumeLabel, settingsManager.musicVolume);
        }
        
        if (ambientVolumeSlider != null)
        {
            ambientVolumeSlider.value = settingsManager.ambientVolume;
            UpdateVolumeLabel(ambientVolumeLabel, settingsManager.ambientVolume);
        }
        
        // Load graphics settings
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = settingsManager.isFullscreen;
        }
        
        if (vSyncToggle != null)
        {
            vSyncToggle.isOn = settingsManager.vSyncEnabled;
        }
        
        // Load resolution setting
        if (resolutionDropdown != null && availableResolutions != null)
        {
            Vector2Int currentRes = settingsManager.currentResolution;
            int index = availableResolutions.FindIndex(r => r.x == currentRes.x && r.y == currentRes.y);
            if (index >= 0)
            {
                resolutionDropdown.value = index;
            }
        }
        
        // Load quality setting
        if (qualityDropdown != null)
        {
            qualityDropdown.value = settingsManager.qualityLevel;
        }
    }
    
    private void StoreOriginalSettings()
    {
        if (settingsManager == null) return;
        
        originalMasterVolume = settingsManager.masterVolume;
        originalSFXVolume = settingsManager.sfxVolume;
        originalMusicVolume = settingsManager.musicVolume;
        originalAmbientVolume = settingsManager.ambientVolume;
        originalFullscreen = settingsManager.isFullscreen;
        originalVSync = settingsManager.vSyncEnabled;
        originalResolution = settingsManager.currentResolution;
        originalQuality = settingsManager.qualityLevel;
    }
    
    // Event handlers
    private void OnMasterVolumeChanged(float value)
    {
        UpdateVolumeLabel(masterVolumeLabel, value);
        pendingSettings["MasterVolume"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
        
        // Apply immediately for preview
        if (settingsManager != null)
        {
            settingsManager.SetMasterVolume(value);
        }
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        UpdateVolumeLabel(sfxVolumeLabel, value);
        pendingSettings["SFXVolume"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
        
        // Apply immediately for preview
        if (settingsManager != null)
        {
            settingsManager.SetSFXVolume(value);
        }
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        UpdateVolumeLabel(musicVolumeLabel, value);
        pendingSettings["MusicVolume"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
        
        // Apply immediately for preview
        if (settingsManager != null)
        {
            settingsManager.SetMusicVolume(value);
        }
    }
    
    private void OnAmbientVolumeChanged(float value)
    {
        UpdateVolumeLabel(ambientVolumeLabel, value);
        pendingSettings["AmbientVolume"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
        
        // Apply immediately for preview
        if (settingsManager != null)
        {
            settingsManager.SetAmbientVolume(value);
        }
    }
    
    private void OnResolutionChanged(int index)
    {
        if (availableResolutions != null && index >= 0 && index < availableResolutions.Count)
        {
            Vector2Int resolution = availableResolutions[index];
            pendingSettings["Resolution"] = resolution;
            hasUnsavedChanges = true;
            UpdateButtonStates();
        }
    }
    
    private void OnFullscreenChanged(bool value)
    {
        pendingSettings["Fullscreen"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
    }
    
    private void OnVSyncChanged(bool value)
    {
        pendingSettings["VSync"] = value;
        hasUnsavedChanges = true;
        UpdateButtonStates();
    }
    
    private void OnQualityChanged(int index)
    {
        pendingSettings["Quality"] = index;
        hasUnsavedChanges = true;
        UpdateButtonStates();
    }
    
    private void OnTestSFXPressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null && testSFXClip != null)
        {
            audioManager.PlaySFX(testSFXClip);
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.Log("SettingsPanel: Test SFX - No audio clip assigned or AudioManager not found");
            }
        }
    }
    
    private void OnApplyPressed()
    {
        ApplyPendingSettings();
        hasUnsavedChanges = false;
        UpdateButtonStates();
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: Settings applied");
        }
    }
    
    private void OnCancelPressed()
    {
        RevertToOriginalSettings();
        hasUnsavedChanges = false;
        UpdateButtonStates();
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: Settings cancelled");
        }
    }
    
    private void OnResetPressed()
    {
        if (settingsManager != null)
        {
            settingsManager.ResetToDefaults();
            LoadCurrentSettings();
            StoreOriginalSettings();
            pendingSettings.Clear();
            hasUnsavedChanges = false;
            UpdateButtonStates();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: Settings reset to defaults");
        }
    }
    
    private void OnClosePressed()
    {
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.HideSettingsMenu();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("SettingsPanel: Settings panel closed");
        }
    }
    
    // Helper methods
    private void UpdateVolumeLabel(Text label, float value)
    {
        if (label != null)
        {
            label.text = $"{label.text.Split(':')[0]}: {value:F0}%";
        }
    }
    
    private void UpdateButtonStates()
    {
        if (applyButton != null)
        {
            applyButton.interactable = hasUnsavedChanges;
        }
        
        if (cancelButton != null)
        {
            cancelButton.interactable = hasUnsavedChanges;
        }
    }
    
    private void ApplyPendingSettings()
    {
        if (settingsManager == null) return;
        
        foreach (var setting in pendingSettings)
        {
            switch (setting.Key)
            {
                case "MasterVolume":
                    settingsManager.SetMasterVolume((float)setting.Value);
                    break;
                case "SFXVolume":
                    settingsManager.SetSFXVolume((float)setting.Value);
                    break;
                case "MusicVolume":
                    settingsManager.SetMusicVolume((float)setting.Value);
                    break;
                case "AmbientVolume":
                    settingsManager.SetAmbientVolume((float)setting.Value);
                    break;
                case "Resolution":
                    Vector2Int resolution = (Vector2Int)setting.Value;
                    settingsManager.SetResolution(resolution);
                    break;
                case "Fullscreen":
                    settingsManager.SetFullscreen((bool)setting.Value);
                    break;
                case "VSync":
                    settingsManager.SetVSync((bool)setting.Value);
                    break;
                case "Quality":
                    settingsManager.SetQualityLevel((int)setting.Value);
                    break;
            }
        }
        
        pendingSettings.Clear();
        settingsManager.SaveSettings();
        StoreOriginalSettings();
    }
    
    private void RevertToOriginalSettings()
    {
        if (settingsManager == null) return;
        
        // Revert to original values
        settingsManager.SetMasterVolume(originalMasterVolume);
        settingsManager.SetSFXVolume(originalSFXVolume);
        settingsManager.SetMusicVolume(originalMusicVolume);
        settingsManager.SetAmbientVolume(originalAmbientVolume);
        settingsManager.SetFullscreen(originalFullscreen);
        settingsManager.SetVSync(originalVSync);
        settingsManager.SetResolution(originalResolution);
        settingsManager.SetQualityLevel(originalQuality);
        
        // Update UI to reflect reverted values
        LoadCurrentSettings();
        pendingSettings.Clear();
    }
    
    // Validation
    public bool ValidateSettingsPanelSetup()
    {
        bool valid = true;
        
        if (settingsManager == null)
        {
            Debug.LogError("SettingsPanel: SettingsManager not found");
            valid = false;
        }
        
        if (masterVolumeSlider == null || sfxVolumeSlider == null || musicVolumeSlider == null || ambientVolumeSlider == null)
        {
            Debug.LogWarning("SettingsPanel: Some audio sliders are missing");
        }
        
        if (resolutionDropdown == null || fullscreenToggle == null || vSyncToggle == null || qualityDropdown == null)
        {
            Debug.LogWarning("SettingsPanel: Some graphics controls are missing");
        }
        
        if (applyButton == null || cancelButton == null || resetButton == null || closeButton == null)
        {
            Debug.LogWarning("SettingsPanel: Some action buttons are missing");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"SettingsPanel: Validation {(valid ? "PASSED" : "FAILED")}");
        }
        
        return valid;
    }
}