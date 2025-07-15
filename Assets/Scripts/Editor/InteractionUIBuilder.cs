using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class InteractionUIBuilder : EditorWindow
{
    [MenuItem("RPG Tools/Build Interaction UI")]
    public static void ShowWindow()
    {
        GetWindow<InteractionUIBuilder>("Interaction UI Builder");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Interaction UI Builder", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("This will create the interaction UI components in your existing canvases.", MessageType.Info);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Build Interaction Prompt UI", GUILayout.Height(30)))
        {
            BuildInteractionPromptUI();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Build Content Panel UI", GUILayout.Height(30)))
        {
            BuildContentPanelUI();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Build Complete Interaction UI", GUILayout.Height(40)))
        {
            BuildInteractionPromptUI();
            BuildContentPanelUI();
            Debug.Log("✅ Complete Interaction UI Built!");
        }
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Test UI Components"))
        {
            TestUIComponents();
        }
    }
    
    void BuildInteractionPromptUI()
    {
        // Find HUD Canvas
        Canvas hudCanvas = FindCanvasByName("HUD Canvas");
        if (hudCanvas == null)
        {
            Debug.LogError("❌ HUD Canvas not found! Please create UI using UIBuilderEditor first.");
            return;
        }
        
        // Remove existing prompt if any
        Transform existingPrompt = hudCanvas.transform.Find("InteractionPrompt");
        if (existingPrompt != null)
        {
            DestroyImmediate(existingPrompt.gameObject);
        }
        
        // Create main prompt GameObject
        GameObject promptGO = new GameObject("InteractionPrompt");
        promptGO.transform.SetParent(hudCanvas.transform, false);
        
        // Add RectTransform
        RectTransform promptRect = promptGO.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.8f);
        promptRect.anchorMax = new Vector2(0.5f, 0.8f);
        promptRect.sizeDelta = new Vector2(300, 60);
        promptRect.anchoredPosition = Vector2.zero;
        
        // Add CanvasGroup for fading
        CanvasGroup canvasGroup = promptGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Create background
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(promptGO.transform, false);
        
        RectTransform bgRect = backgroundGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Image background = backgroundGO.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f);
        background.sprite = CreateRoundedRectSprite();
        
        // Create text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(promptGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, -10);
        textRect.anchoredPosition = Vector2.zero;
        
        Text promptText = textGO.AddComponent<Text>();
        promptText.text = "Press E to interact";
        promptText.fontSize = 16;
        promptText.alignment = TextAnchor.MiddleCenter;
        promptText.color = Color.white;
        promptText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Add InteractionPrompt component
        InteractionPrompt interactionPrompt = promptGO.AddComponent<InteractionPrompt>();
        interactionPrompt.promptPanel = promptGO;
        interactionPrompt.promptText = promptText;
        interactionPrompt.promptBackground = background;
        interactionPrompt.promptCanvasGroup = canvasGroup;
        interactionPrompt.useScreenPosition = true;
        interactionPrompt.followPlayer = false;
        
        // Set promptGO inactive initially
        promptGO.SetActive(false);
        
        Debug.Log("✅ InteractionPrompt UI created in HUD Canvas");
    }
    
    void BuildContentPanelUI()
    {
        // Find Menu Canvas
        Canvas menuCanvas = FindCanvasByName("Menu Canvas");
        if (menuCanvas == null)
        {
            Debug.LogError("❌ Menu Canvas not found! Please create UI using UIBuilderEditor first.");
            Debug.Log("Available canvases:");
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                Debug.Log($"  - {canvas.name}");
            }
            return;
        }
        
        Debug.Log($"✅ Found Menu Canvas: {menuCanvas.name}");
        
        // Remove existing panel if any
        Transform existingPanel = menuCanvas.transform.Find("InteractionContentPanel");
        if (existingPanel != null)
        {
            DestroyImmediate(existingPanel.gameObject);
        }
        
        // Create main panel GameObject
        GameObject panelGO = new GameObject("InteractionContentPanel");
        panelGO.transform.SetParent(menuCanvas.transform, false);
        
        // Add RectTransform - full screen
        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        // Add CanvasGroup for fading
        CanvasGroup canvasGroup = panelGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Create background overlay
        GameObject overlayGO = new GameObject("Overlay");
        overlayGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform overlayRect = overlayGO.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        
        Image overlay = overlayGO.AddComponent<Image>();
        overlay.color = new Color(0, 0, 0, 0.5f);
        
        // Create content container
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.2f, 0.2f);
        contentRect.anchorMax = new Vector2(0.8f, 0.8f);
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        Image contentBg = contentGO.AddComponent<Image>();
        contentBg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        contentBg.sprite = CreateRoundedRectSprite();
        
        // Create title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.8f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.sizeDelta = new Vector2(-40, -20);
        titleRect.anchoredPosition = Vector2.zero;
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Interaction Title";
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Create description
        GameObject descGO = new GameObject("Description");
        descGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform descRect = descGO.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0f, 0.4f);
        descRect.anchorMax = new Vector2(1f, 0.8f);
        descRect.sizeDelta = new Vector2(-40, -20);
        descRect.anchoredPosition = Vector2.zero;
        
        Text descriptionText = descGO.AddComponent<Text>();
        descriptionText.text = "Description text goes here...";
        descriptionText.fontSize = 16;
        descriptionText.alignment = TextAnchor.UpperLeft;
        descriptionText.color = Color.white;
        descriptionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Create lore section
        GameObject loreGO = new GameObject("Lore");
        loreGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform loreRect = loreGO.AddComponent<RectTransform>();
        loreRect.anchorMin = new Vector2(0f, 0.1f);
        loreRect.anchorMax = new Vector2(1f, 0.4f);
        loreRect.sizeDelta = new Vector2(-40, -20);
        loreRect.anchoredPosition = Vector2.zero;
        
        Text loreText = loreGO.AddComponent<Text>();
        loreText.text = "Historical lore goes here...";
        loreText.fontSize = 14;
        loreText.fontStyle = FontStyle.Italic;
        loreText.alignment = TextAnchor.UpperLeft;
        loreText.color = new Color(0.8f, 0.8f, 1f);
        loreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Create category
        GameObject categoryGO = new GameObject("Category");
        categoryGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform categoryRect = categoryGO.AddComponent<RectTransform>();
        categoryRect.anchorMin = new Vector2(0f, 0f);
        categoryRect.anchorMax = new Vector2(0.5f, 0.1f);
        categoryRect.sizeDelta = new Vector2(-20, -10);
        categoryRect.anchoredPosition = Vector2.zero;
        
        Text categoryText = categoryGO.AddComponent<Text>();
        categoryText.text = "[Category]";
        categoryText.fontSize = 12;
        categoryText.alignment = TextAnchor.MiddleLeft;
        categoryText.color = Color.yellow;
        categoryText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Create close button
        GameObject closeButtonGO = new GameObject("CloseButton");
        closeButtonGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform closeRect = closeButtonGO.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.8f, 0f);
        closeRect.anchorMax = new Vector2(1f, 0.1f);
        closeRect.sizeDelta = new Vector2(-20, -10);
        closeRect.anchoredPosition = Vector2.zero;
        
        Image closeButtonImg = closeButtonGO.AddComponent<Image>();
        closeButtonImg.color = new Color(0.8f, 0.2f, 0.2f);
        
        Button closeButton = closeButtonGO.AddComponent<Button>();
        
        GameObject closeTextGO = new GameObject("Text");
        closeTextGO.transform.SetParent(closeButtonGO.transform, false);
        
        RectTransform closeTextRect = closeTextGO.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        closeTextRect.anchoredPosition = Vector2.zero;
        
        Text closeButtonText = closeTextGO.AddComponent<Text>();
        closeButtonText.text = "Close";
        closeButtonText.fontSize = 12;
        closeButtonText.alignment = TextAnchor.MiddleCenter;
        closeButtonText.color = Color.white;
        closeButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Add InteractionContentPanel component
        InteractionContentPanel contentPanel = panelGO.AddComponent<InteractionContentPanel>();
        contentPanel.contentPanel = panelGO;
        contentPanel.titleText = titleText;
        contentPanel.descriptionText = descriptionText;
        contentPanel.loreText = loreText;
        contentPanel.categoryText = categoryText;
        contentPanel.closeButton = closeButton;
        
        // Set panel inactive initially
        panelGO.SetActive(false);
        
        Debug.Log("✅ InteractionContentPanel UI created in Menu Canvas");
    }
    
    Sprite CreateRoundedRectSprite()
    {
        // Create a simple white texture for UI backgrounds
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
    
    Canvas FindCanvasByName(string name)
    {
        GameObject canvasGO = GameObject.Find(name);
        if (canvasGO != null)
        {
            return canvasGO.GetComponent<Canvas>();
        }
        return null;
    }
    
    void TestUIComponents()
    {
        if (Application.isPlaying)
        {
            // Test InteractionPrompt
            if (InteractionPrompt.Instance != null)
            {
                InteractionPrompt.Instance.ShowPromptNew("Test interaction prompt!", InteractionType.General);
                Debug.Log("✅ InteractionPrompt test triggered");
            }
            else
            {
                Debug.LogWarning("❌ InteractionPrompt instance not found");
            }
            
            // Test ContentPanel  
            if (InteractionContentPanel.Instance != null)
            {
                InteractionContent testContent = new InteractionContent
                {
                    title = "Test Content Panel",
                    description = "This is a test of the interaction content panel system. If you can see this, the UI is working correctly!",
                    loreText = "This panel was created by the InteractionUIBuilder tool to test the content display system.",
                    category = "Test",
                    contentDisplayTime = 10f
                };
                
                InteractionContentPanel.Instance.ShowContent(testContent);
                Debug.Log("✅ InteractionContentPanel test triggered");
            }
            else
            {
                Debug.LogWarning("❌ InteractionContentPanel instance not found");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Enter Play mode to test UI components");
        }
    }
}