using UnityEngine;
using UnityEditor;

public class MasterInteractionSetup : EditorWindow
{
    [MenuItem("RPG Tools/⭐ Master Interaction Setup")]
    public static void ShowWindow()
    {
        GetWindow<MasterInteractionSetup>("Master Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("⭐ Master Interaction Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("This will set up the COMPLETE interaction system in the correct order:\n\n1. Base UI System (Canvases)\n2. Interaction UI Components\n3. Interaction Logic System\n4. Village Interactables", MessageType.Info);
        GUILayout.Space(15);
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🚀 SETUP EVERYTHING", GUILayout.Height(50)))
        {
            SetupEverything();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(20);
        
        GUILayout.Label("Manual Steps (if needed):", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. Create Base UI System"))
        {
            CreateBaseUISystem();
        }
        
        if (GUILayout.Button("2. Create Interaction UI"))
        {
            CreateInteractionUI();
        }
        
        if (GUILayout.Button("3. Setup Interaction Logic"))
        {
            SetupInteractionLogic();
        }
        
        if (GUILayout.Button("4. Setup Village Objects"))
        {
            SetupVillageObjects();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🧪 Test Everything"))
        {
            TestEverything();
        }
    }
    
    void SetupEverything()
    {
        Debug.Log("🚀 === MASTER INTERACTION SETUP STARTED ===");
        
        // Step 1: Base UI System
        Debug.Log("📋 Step 1: Creating Base UI System...");
        CreateBaseUISystem();
        
        // Step 2: Interaction UI
        Debug.Log("🎮 Step 2: Creating Interaction UI...");
        CreateInteractionUI();
        
        // Step 3: Interaction Logic
        Debug.Log("⚙️ Step 3: Setting up Interaction Logic...");
        SetupInteractionLogic();
        
        // Step 4: Village Objects
        Debug.Log("🏘️ Step 4: Setting up Village Objects...");
        SetupVillageObjects();
        
        Debug.Log("✅ === MASTER SETUP COMPLETED ===");
        Debug.Log("🎉 Ready to test! Press Play and walk near village objects, then press E!");
        
        EditorUtility.DisplayDialog("Setup Complete!", 
            "Master Interaction Setup completed!\n\n" +
            "✅ Base UI System created\n" +
            "✅ Interaction UI built\n" +
            "✅ Interaction logic setup\n" +
            "✅ Village objects configured\n\n" +
            "Press Play and test interactions!", "Awesome!");
        
        Close();
    }
    
    void CreateBaseUISystem()
    {
        // Use UIBuilderEditor if available
        var uiBuilderWindow = GetWindow<UIBuilderEditor>();
        if (uiBuilderWindow != null)
        {
            // Call the build method
            var method = typeof(UIBuilderEditor).GetMethod("BuildCompleteUISystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(uiBuilderWindow, null);
                uiBuilderWindow.Close();
            }
        }
        else
        {
            Debug.LogWarning("⚠️ UIBuilderEditor not found, creating basic canvases manually...");
            CreateBasicCanvases();
        }
    }
    
    void CreateBasicCanvases()
    {
        // Create HUD Canvas
        if (GameObject.Find("HUD Canvas") == null)
        {
            GameObject hudCanvasGO = new GameObject("HUD Canvas");
            Canvas hudCanvas = hudCanvasGO.AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hudCanvas.sortingOrder = 0;
            hudCanvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            hudCanvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Debug.Log("✅ Created HUD Canvas");
        }
        
        // Create Menu Canvas
        if (GameObject.Find("Menu Canvas") == null)
        {
            GameObject menuCanvasGO = new GameObject("Menu Canvas");
            Canvas menuCanvas = menuCanvasGO.AddComponent<Canvas>();
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuCanvas.sortingOrder = 10;
            menuCanvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            menuCanvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Debug.Log("✅ Created Menu Canvas");
        }
        
        // Create Overlay Canvas
        if (GameObject.Find("Overlay Canvas") == null)
        {
            GameObject overlayCanvasGO = new GameObject("Overlay Canvas");
            Canvas overlayCanvas = overlayCanvasGO.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 20;
            overlayCanvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            overlayCanvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Debug.Log("✅ Created Overlay Canvas");
        }
    }
    
    void CreateInteractionUI()
    {
        // Create the UI directly instead of using reflection
        BuildInteractionPromptUIDirect();
        BuildContentPanelUIDirect();
    }
    
    void SetupInteractionLogic()
    {
        var setupWindow = GetWindow<InteractionSystemSetup>();
        if (setupWindow != null)
        {
            // Call setup methods
            var setupMethod = typeof(InteractionSystemSetup).GetMethod("SetupCompleteSystem", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (setupMethod != null)
            {
                setupMethod.Invoke(setupWindow, null);
            }
            
            setupWindow.Close();
        }
    }
    
    void SetupVillageObjects()
    {
        // Find or create VillageInteractables
        GameObject villageGO = GameObject.Find("Village");
        if (villageGO == null)
        {
            villageGO = new GameObject("VillageInteractables");
        }
        
        VillageInteractables villageInteractables = villageGO.GetComponent<VillageInteractables>();
        if (villageInteractables == null)
        {
            villageInteractables = villageGO.AddComponent<VillageInteractables>();
        }
        
        villageInteractables.autoCreateInteractables = true;
        villageInteractables.useExistingObjects = true;
        villageInteractables.enableDebugLogs = true;
        
        Debug.Log("✅ Village interactables setup completed");
    }
    
    void TestEverything()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("⚠️ Enter Play mode to test the interaction system");
            return;
        }
        
        // List all canvases
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"🖼️ Found {canvases.Length} canvases:");
        foreach (Canvas canvas in canvases)
        {
            string status = canvas.gameObject.activeInHierarchy ? "ACTIVE" : "INACTIVE";
            Debug.Log($"  - {canvas.name}: {status}");
        }
        
        // Check interaction components
        InteractionSystem interactionSystem = FindFirstObjectByType<InteractionSystem>();
        Debug.Log($"🎯 InteractionSystem: {(interactionSystem != null ? "FOUND" : "MISSING")}");
        
        InteractionPrompt prompt = FindFirstObjectByType<InteractionPrompt>();
        Debug.Log($"💬 InteractionPrompt: {(prompt != null ? "FOUND" : "MISSING")}");
        
        InteractionContentPanel panel = FindFirstObjectByType<InteractionContentPanel>();
        Debug.Log($"📝 InteractionContentPanel: {(panel != null ? "FOUND" : "MISSING")}");
        
        InteractableObject[] interactables = FindObjectsOfType<InteractableObject>();
        Debug.Log($"🎮 Found {interactables.Length} interactable objects");
        
        // Test interaction if possible
        if (panel != null)
        {
            InteractionContent testContent = new InteractionContent
            {
                title = "🧪 Master Setup Test",
                description = "If you can see this message, the complete interaction system is working perfectly! Walk near village objects and press E to interact with them.",
                loreText = "The ancient Master Setup ritual was successful, bringing life to the interactive elements of the Kingdom of Aethermoor.",
                category = "System Test",
                contentDisplayTime = 10f
            };
            
            panel.ShowContent(testContent);
            Debug.Log("🧪 Test content panel displayed!");
        }
    }
    
    void BuildInteractionPromptUIDirect()
    {
        // Find HUD Canvas
        Canvas hudCanvas = FindCanvasByName("HUD Canvas");
        if (hudCanvas == null)
        {
            Debug.LogError("❌ HUD Canvas not found for InteractionPrompt!");
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
        
        UnityEngine.UI.Image background = backgroundGO.AddComponent<UnityEngine.UI.Image>();
        background.color = new Color(0, 0, 0, 0.7f);
        
        // Create text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(promptGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, -10);
        textRect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Text promptText = textGO.AddComponent<UnityEngine.UI.Text>();
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
    
    void BuildContentPanelUIDirect()
    {
        // Find Menu Canvas
        Canvas menuCanvas = FindCanvasByName("Menu Canvas");
        if (menuCanvas == null)
        {
            Debug.LogError("❌ Menu Canvas not found for InteractionContentPanel!");
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
        
        UnityEngine.UI.Image overlay = overlayGO.AddComponent<UnityEngine.UI.Image>();
        overlay.color = new Color(0, 0, 0, 0.5f);
        
        // Create content container
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.2f, 0.2f);
        contentRect.anchorMax = new Vector2(0.8f, 0.8f);
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image contentBg = contentGO.AddComponent<UnityEngine.UI.Image>();
        contentBg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        
        // Create title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(contentGO.transform, false);
        
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.8f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.sizeDelta = new Vector2(-40, -20);
        titleRect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Text titleText = titleGO.AddComponent<UnityEngine.UI.Text>();
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
        
        UnityEngine.UI.Text descriptionText = descGO.AddComponent<UnityEngine.UI.Text>();
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
        
        UnityEngine.UI.Text loreText = loreGO.AddComponent<UnityEngine.UI.Text>();
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
        
        UnityEngine.UI.Text categoryText = categoryGO.AddComponent<UnityEngine.UI.Text>();
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
        
        UnityEngine.UI.Image closeButtonImg = closeButtonGO.AddComponent<UnityEngine.UI.Image>();
        closeButtonImg.color = new Color(0.8f, 0.2f, 0.2f);
        
        UnityEngine.UI.Button closeButton = closeButtonGO.AddComponent<UnityEngine.UI.Button>();
        
        GameObject closeTextGO = new GameObject("Text");
        closeTextGO.transform.SetParent(closeButtonGO.transform, false);
        
        RectTransform closeTextRect = closeTextGO.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        closeTextRect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Text closeButtonText = closeTextGO.AddComponent<UnityEngine.UI.Text>();
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
    
    Canvas FindCanvasByName(string name)
    {
        GameObject canvasGO = GameObject.Find(name);
        if (canvasGO != null)
        {
            return canvasGO.GetComponent<Canvas>();
        }
        return null;
    }
}