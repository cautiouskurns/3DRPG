using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Editor tool to build complete UI hierarchy in edit mode
/// This prevents runtime crashes and allows visual design
/// </summary>
public class UIBuilderEditor : EditorWindow
{
    [Header("UI Builder Settings")]
    public bool enableDebugLogs = true;
    public bool replaceExistingUI = true;
    
    [Header("Canvas Settings")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    public int hudSortOrder = 100;
    public int menuSortOrder = 200;
    public int overlaySortOrder = 300;
    
    [Header("Settings Panel Design")]
    public Vector2 panelSize = new Vector2(600, 500);
    public Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    
    [Header("HUD Design")]
    public Vector2 hudBarSize = new Vector2(300, 20);
    public Vector2 hudSpacing = new Vector2(10, 30);
    public Vector2 hudOffset = new Vector2(20, 20);
    
    private GameObject uiRoot;
    private Canvas hudCanvas;
    private Canvas menuCanvas;
    private Canvas overlayCanvas;
    
    [MenuItem("Tools/UI Builder")]
    public static void ShowWindow()
    {
        UIBuilderEditor window = GetWindow<UIBuilderEditor>();
        window.titleContent = new GUIContent("UI Builder");
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Label("UI Builder - Edit Mode UI Creation", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        enableDebugLogs = EditorGUILayout.Toggle("Enable Debug Logs", enableDebugLogs);
        replaceExistingUI = EditorGUILayout.Toggle("Replace Existing UI", replaceExistingUI);
        
        GUILayout.Space(10);
        GUILayout.Label("Canvas Settings", EditorStyles.boldLabel);
        referenceResolution = EditorGUILayout.Vector2Field("Reference Resolution", referenceResolution);
        hudSortOrder = EditorGUILayout.IntField("HUD Sort Order", hudSortOrder);
        menuSortOrder = EditorGUILayout.IntField("Menu Sort Order", menuSortOrder);
        overlaySortOrder = EditorGUILayout.IntField("Overlay Sort Order", overlaySortOrder);
        
        GUILayout.Space(10);
        GUILayout.Label("Design Settings", EditorStyles.boldLabel);
        panelSize = EditorGUILayout.Vector2Field("Settings Panel Size", panelSize);
        panelBackgroundColor = EditorGUILayout.ColorField("Panel Background", panelBackgroundColor);
        
        hudBarSize = EditorGUILayout.Vector2Field("HUD Bar Size", hudBarSize);
        hudSpacing = EditorGUILayout.Vector2Field("HUD Spacing", hudSpacing);
        hudOffset = EditorGUILayout.Vector2Field("HUD Offset", hudOffset);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Build Complete UI System", GUILayout.Height(40)))
        {
            BuildCompleteUISystem();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Build Canvas Hierarchy Only", GUILayout.Height(30)))
        {
            BuildCanvasHierarchy();
        }
        
        if (GUILayout.Button("Build Settings Panel Only", GUILayout.Height(30)))
        {
            BuildSettingsPanel();
        }
        
        if (GUILayout.Button("Build HUD Only", GUILayout.Height(30)))
        {
            BuildHUD();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Clean Up UI", GUILayout.Height(30)))
        {
            CleanUpExistingUI();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        GUILayout.Label("1. Click 'Build Complete UI System' to create everything", EditorStyles.wordWrappedLabel);
        GUILayout.Label("2. Modify components in Inspector as needed", EditorStyles.wordWrappedLabel);
        GUILayout.Label("3. Test in Play mode - no runtime creation needed!", EditorStyles.wordWrappedLabel);
    }
    
    void BuildCompleteUISystem()
    {
        if (enableDebugLogs)
        {
            Debug.Log("=== BUILDING COMPLETE UI SYSTEM IN EDIT MODE ===");
        }
        
        if (replaceExistingUI)
        {
            CleanUpExistingUI();
        }
        
        // Step 1: Create UI Root
        CreateUIRoot();
        
        // Step 2: Build Canvas Hierarchy
        BuildCanvasHierarchy();
        
        // Step 3: Create EventSystem if needed
        CreateEventSystem();
        
        // Step 4: Build HUD
        BuildHUD();
        
        // Step 5: Build Settings Panel
        BuildSettingsPanel();
        
        // Step 6: Add Manager Components
        AddManagerComponents();
        
        // Step 7: Configure Components
        ConfigureComponents();
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Complete UI System built successfully in edit mode!");
            Debug.Log("Check the hierarchy - everything should be visible and configurable");
        }
        
        // Mark scene as dirty to save changes
        EditorUtility.SetDirty(uiRoot);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
    
    void CreateUIRoot()
    {
        uiRoot = GameObject.Find("UI System");
        if (uiRoot == null)
        {
            uiRoot = new GameObject("UI System");
        }
        
        // Add UIManager component if not present
        UIManager uiManager = uiRoot.GetComponent<UIManager>();
        if (uiManager == null)
        {
            uiManager = uiRoot.AddComponent<UIManager>();
        }
        
        // Disable auto-creation since we're doing it in edit mode
        uiManager.enableDebugLogs = enableDebugLogs;
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ UI Root created: " + uiRoot.name);
        }
    }
    
    void BuildCanvasHierarchy()
    {
        // HUD Canvas
        hudCanvas = CreateCanvas("HUD Canvas", hudSortOrder, uiRoot.transform);
        
        // Menu Canvas  
        menuCanvas = CreateCanvas("Menu Canvas", menuSortOrder, uiRoot.transform);
        menuCanvas.gameObject.SetActive(false); // Start hidden
        
        // Overlay Canvas
        overlayCanvas = CreateCanvas("Overlay Canvas", overlaySortOrder, uiRoot.transform);
        overlayCanvas.gameObject.SetActive(false); // Start hidden
        
        // Configure UIManager references
        UIManager uiManager = uiRoot.GetComponent<UIManager>();
        if (uiManager != null)
        {
            uiManager.hudCanvas = hudCanvas;
            uiManager.menuCanvas = menuCanvas;
            uiManager.overlayCanvas = overlayCanvas;
            uiManager.hudSortOrder = hudSortOrder;
            uiManager.menuSortOrder = menuSortOrder;
            uiManager.overlaySortOrder = overlaySortOrder;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Canvas hierarchy created with sort orders: HUD(" + hudSortOrder + "), Menu(" + menuSortOrder + "), Overlay(" + overlaySortOrder + ")");
        }
    }
    
    Canvas CreateCanvas(string name, int sortOrder, Transform parent)
    {
        GameObject canvasGO = new GameObject(name);
        canvasGO.transform.SetParent(parent, false);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        return canvas;
    }
    
    void CreateEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            if (enableDebugLogs)
            {
                Debug.Log("✅ EventSystem created");
            }
        }
    }
    
    void BuildHUD()
    {
        if (hudCanvas == null) return;
        
        // Create HUD Container
        GameObject hudContainer = new GameObject("HUD Container");
        hudContainer.transform.SetParent(hudCanvas.transform, false);
        
        RectTransform containerRect = hudContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 1);
        containerRect.anchoredPosition = hudOffset;
        
        // Create Health Bar
        CreateStatBar(hudContainer, "Health Bar", 0, Color.green, new Color(0.2f, 0.1f, 0.1f, 0.8f));
        
        // Create MP Bar
        CreateStatBar(hudContainer, "MP Bar", 1, Color.blue, new Color(0.1f, 0.1f, 0.2f, 0.8f));
        
        // Create XP Bar
        CreateStatBar(hudContainer, "XP Bar", 2, Color.yellow, new Color(0.2f, 0.2f, 0.1f, 0.8f));
        
        // Create Level Text
        CreateLevelText(hudContainer);
        
        // Create Interaction Prompt
        CreateInteractionPrompt(hudCanvas);
        
        // Add HUDController component and configure it
        HUDController hudController = hudContainer.GetComponent<HUDController>();
        if (hudController == null)
        {
            hudController = hudContainer.AddComponent<HUDController>();
        }
        hudController.autoCreateUI = false; // We've created it manually
        
        // Assign references directly
        hudController.healthBar = hudContainer.transform.Find("Health Bar")?.GetComponent<Slider>();
        hudController.mpBar = hudContainer.transform.Find("MP Bar")?.GetComponent<Slider>();
        hudController.xpBar = hudContainer.transform.Find("XP Bar")?.GetComponent<Slider>();
        hudController.levelText = hudContainer.transform.Find("Level Text")?.GetComponent<Text>();
        hudController.interactionPromptText = hudCanvas.transform.Find("Interaction Prompt")?.GetComponent<Text>();
        
        // Link to UIManager
        UIManager uiManager = uiRoot.GetComponent<UIManager>();
        if (uiManager != null)
        {
            uiManager.hudController = hudController;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ HUD created with health, MP, XP bars and level text");
        }
    }
    
    Slider CreateStatBar(GameObject parent, string name, int index, Color fillColor, Color backgroundColor)
    {
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent.transform, false);
        
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.sizeDelta = hudBarSize;
        sliderRect.anchoredPosition = new Vector2(0, -(hudBarSize.y + hudSpacing.y) * index);
        
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
        
        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Sliced;
        
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        
        return slider;
    }
    
    void CreateLevelText(GameObject parent)
    {
        GameObject textGO = new GameObject("Level Text");
        textGO.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        textRect.anchoredPosition = new Vector2(hudBarSize.x + 20, -hudBarSize.y / 2);
        
        Text text = textGO.AddComponent<Text>();
        text.text = "Level 1";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        
        // Add outline
        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);
    }
    
    void CreateInteractionPrompt(Canvas canvas)
    {
        GameObject promptGO = new GameObject("Interaction Prompt");
        promptGO.transform.SetParent(canvas.transform, false);
        
        RectTransform promptRect = promptGO.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.sizeDelta = new Vector2(400, 50);
        promptRect.anchoredPosition = new Vector2(0, 100);
        
        Text text = promptGO.AddComponent<Text>();
        text.text = "";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add outline
        Outline outline = promptGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, 2);
        
        // Add CanvasGroup for fading
        CanvasGroup canvasGroup = promptGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        
        promptGO.SetActive(false);
    }
    
    void BuildSettingsPanel()
    {
        if (menuCanvas == null) return;
        
        // Create Settings Panel
        GameObject settingsPanelGO = new GameObject("Settings Panel");
        settingsPanelGO.transform.SetParent(menuCanvas.transform, false);
        
        RectTransform panelRect = settingsPanelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = panelSize;
        
        // Add background
        Image panelImage = settingsPanelGO.AddComponent<Image>();
        panelImage.color = panelBackgroundColor;
        
        // Add CanvasGroup for fading
        CanvasGroup canvasGroup = settingsPanelGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Create sections
        CreateAudioSection(settingsPanelGO);
        CreateGraphicsSection(settingsPanelGO);
        CreateButtonSection(settingsPanelGO);
        
        // Add SettingsPanel component
        SettingsPanel settingsPanel = settingsPanelGO.AddComponent<SettingsPanel>();
        settingsPanel.autoCreateUI = false; // We've created it manually
        settingsPanel.panelSize = panelSize;
        settingsPanel.panelBackgroundColor = panelBackgroundColor;
        
        // Link to UIManager
        UIManager uiManager = uiRoot.GetComponent<UIManager>();
        if (uiManager != null)
        {
            uiManager.settingsPanel = settingsPanel;
        }
        
        // Initially hide the panel
        settingsPanelGO.SetActive(false);
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Settings Panel created with Audio, Graphics, and Button sections");
        }
    }
    
    void CreateAudioSection(GameObject parent)
    {
        GameObject audioSection = CreateSection(parent, "Audio Section", 0);
        
        CreateSectionTitle(audioSection, "Audio Settings");
        
        CreateVolumeSlider(audioSection, "Master Volume", 0);
        CreateVolumeSlider(audioSection, "SFX Volume", 1);
        CreateVolumeSlider(audioSection, "Music Volume", 2);
        CreateVolumeSlider(audioSection, "Ambient Volume", 3);
        
        CreateTestButton(audioSection, "Test SFX");
    }
    
    void CreateGraphicsSection(GameObject parent)
    {
        GameObject graphicsSection = CreateSection(parent, "Graphics Section", 1);
        
        CreateSectionTitle(graphicsSection, "Graphics Settings");
        
        CreateDropdown(graphicsSection, "Resolution", 0);
        CreateToggle(graphicsSection, "Fullscreen", 1);
        CreateToggle(graphicsSection, "VSync", 2);
        CreateDropdown(graphicsSection, "Quality", 3);
    }
    
    void CreateButtonSection(GameObject parent)
    {
        GameObject buttonSection = CreateSection(parent, "Button Section", 2);
        
        CreateActionButton(buttonSection, "Apply", 0, Color.green);
        CreateActionButton(buttonSection, "Cancel", 1, Color.red);
        CreateActionButton(buttonSection, "Reset", 2, Color.yellow);
        CreateActionButton(buttonSection, "Close", 3, Color.gray);
    }
    
    GameObject CreateSection(GameObject parent, string sectionName, int index)
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
    
    void CreateSectionTitle(GameObject parent, string title)
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
    
    void CreateVolumeSlider(GameObject parent, string labelText, int index)
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
        
        Text label = labelGO.AddComponent<Text>();
        label.text = labelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 14;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        
        // Create slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(sliderContainer.transform, false);
        
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.35f, 0);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.sizeDelta = Vector2.zero;
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.wholeNumbers = true;
        
        CreateSliderVisuals(sliderGO, slider);
    }
    
    void CreateSliderVisuals(GameObject sliderGO, Slider slider)
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
    
    void CreateToggle(GameObject parent, string labelText, int index)
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
        
        CreateToggleVisuals(toggleGO, toggle, labelText);
    }
    
    void CreateToggleVisuals(GameObject toggleGO, Toggle toggle, string labelText)
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
    
    void CreateDropdown(GameObject parent, string labelText, int index)
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
        
        // Create dropdown
        GameObject dropdownGO = new GameObject("Dropdown");
        dropdownGO.transform.SetParent(dropdownContainer.transform, false);
        
        RectTransform dropdownRect = dropdownGO.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0.35f, 0);
        dropdownRect.anchorMax = new Vector2(1, 1);
        dropdownRect.sizeDelta = Vector2.zero;
        
        Dropdown dropdown = dropdownGO.AddComponent<Dropdown>();
        
        CreateDropdownVisuals(dropdownGO, dropdown);
    }
    
    void CreateDropdownVisuals(GameObject dropdownGO, Dropdown dropdown)
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
    
    void CreateTestButton(GameObject parent, string buttonText)
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
    }
    
    void CreateActionButton(GameObject parent, string buttonText, int index, Color color)
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
    }
    
    void AddManagerComponents()
    {
        // Add SettingsManager if not present
        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager == null)
        {
            GameObject settingsManagerGO = new GameObject("SettingsManager");
            settingsManager = settingsManagerGO.AddComponent<SettingsManager>();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Manager components configured");
        }
    }
    
    void ConfigureComponents()
    {
        UIManager uiManager = uiRoot.GetComponent<UIManager>();
        if (uiManager != null)
        {
            // Disable auto-creation since we've done it manually
            uiManager.enableDebugLogs = enableDebugLogs;
            
            // Wire up references if they're missing
            if (uiManager.hudCanvas == null) uiManager.hudCanvas = hudCanvas;
            if (uiManager.menuCanvas == null) uiManager.menuCanvas = menuCanvas;
            if (uiManager.overlayCanvas == null) uiManager.overlayCanvas = overlayCanvas;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Components configured and linked");
        }
    }
    
    void CleanUpExistingUI()
    {
        // Remove existing UI components
        UIManager[] existingUIManagers = FindObjectsOfType<UIManager>();
        foreach (UIManager manager in existingUIManagers)
        {
            if (enableDebugLogs)
            {
                Debug.Log("Removing existing UIManager: " + manager.gameObject.name);
            }
            DestroyImmediate(manager.gameObject);
        }
        
        // Remove existing canvases with our names
        Canvas[] existingCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in existingCanvases)
        {
            if (canvas.name.Contains("HUD Canvas") || canvas.name.Contains("Menu Canvas") || canvas.name.Contains("Overlay Canvas"))
            {
                if (enableDebugLogs)
                {
                    Debug.Log("Removing existing canvas: " + canvas.gameObject.name);
                }
                DestroyImmediate(canvas.gameObject);
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("✅ Existing UI cleaned up");
        }
    }
}