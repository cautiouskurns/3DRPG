using UnityEngine;
using UnityEditor;

public class InteractionSystemSetup : EditorWindow
{
    private bool setupInteractionSystem = true;
    private bool setupVillageInteractables = true;
    private bool setupAudioManager = true;
    private bool setupValidator = true;
    private bool setupUIComponents = true;
    
    [MenuItem("RPG Tools/Setup Interaction System")]
    public static void ShowWindow()
    {
        GetWindow<InteractionSystemSetup>("Interaction System Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Interaction System Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool will set up the complete interaction system in your scene.", EditorStyles.helpBox);
        GUILayout.Space(10);
        
        setupInteractionSystem = EditorGUILayout.Toggle("Setup InteractionSystem", setupInteractionSystem);
        setupVillageInteractables = EditorGUILayout.Toggle("Setup Village Interactables", setupVillageInteractables);
        setupAudioManager = EditorGUILayout.Toggle("Setup Audio Manager", setupAudioManager);
        setupValidator = EditorGUILayout.Toggle("Setup Validator", setupValidator);
        setupUIComponents = EditorGUILayout.Toggle("Setup UI Components", setupUIComponents);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Setup Complete System", GUILayout.Height(30)))
        {
            SetupCompleteSystem();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Village Only", GUILayout.Height(25)))
        {
            SetupVillageOnly();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("Make sure you have a Player object with PlayerController and village objects created by BasicVillageLayout before running setup.", MessageType.Info);
    }
    
    void SetupCompleteSystem()
    {
        Debug.Log("=== INTERACTION SYSTEM SETUP STARTED ===");
        
        if (setupInteractionSystem)
        {
            SetupInteractionSystemComponent();
        }
        
        if (setupVillageInteractables)
        {
            SetupVillageInteractablesComponent();
        }
        
        if (setupAudioManager)
        {
            SetupInteractionAudioManager();
        }
        
        if (setupValidator)
        {
            SetupSystemValidator();
        }
        
        if (setupUIComponents)
        {
            SetupUIInteractionComponents();
        }
        
        Debug.Log("=== INTERACTION SYSTEM SETUP COMPLETED ===");
        Debug.Log("Press Play and walk near village objects, then press E to interact!");
        
        Close();
    }
    
    void SetupVillageOnly()
    {
        SetupVillageInteractablesComponent();
        Debug.Log("Village interactables setup completed!");
    }
    
    void SetupInteractionSystemComponent()
    {
        GameObject interactionSystemGO = GameObject.Find("InteractionSystem");
        if (interactionSystemGO == null)
        {
            interactionSystemGO = new GameObject("InteractionSystem");
        }
        
        InteractionSystem interactionSystem = interactionSystemGO.GetComponent<InteractionSystem>();
        if (interactionSystem == null)
        {
            interactionSystem = interactionSystemGO.AddComponent<InteractionSystem>();
        }
        
        // Find and assign player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller != null)
            {
                player = controller.gameObject;
                player.tag = "Player";
            }
        }
        
        if (player != null)
        {
            interactionSystem.playerTransform = player.transform;
            Debug.Log("✓ InteractionSystem setup with player reference");
        }
        else
        {
            Debug.LogWarning("⚠ Player not found - please assign manually");
        }
        
        interactionSystem.interactionRadius = 1.5f;
        interactionSystem.enableDebugLogs = true;
        interactionSystem.enableDebugVisuals = true;
    }
    
    void SetupVillageInteractablesComponent()
    {
        GameObject villageGO = GameObject.Find("Village");
        if (villageGO == null)
        {
            villageGO = GameObject.Find("VillageInteractables");
            if (villageGO == null)
            {
                villageGO = new GameObject("VillageInteractables");
            }
        }
        
        VillageInteractables villageInteractables = villageGO.GetComponent<VillageInteractables>();
        if (villageInteractables == null)
        {
            villageInteractables = villageGO.AddComponent<VillageInteractables>();
        }
        
        villageInteractables.autoCreateInteractables = true;
        villageInteractables.useExistingObjects = true;
        villageInteractables.enableDebugLogs = true;
        
        // Trigger setup if in play mode
        if (Application.isPlaying)
        {
            villageInteractables.SetupVillageInteractables();
        }
        
        Debug.Log("✓ VillageInteractables component added");
    }
    
    void SetupInteractionAudioManager()
    {
        GameObject audioManagerGO = GameObject.Find("InteractionAudioManager");
        if (audioManagerGO == null)
        {
            audioManagerGO = new GameObject("InteractionAudioManager");
        }
        
        InteractionAudioManager audioManager = audioManagerGO.GetComponent<InteractionAudioManager>();
        if (audioManager == null)
        {
            audioManager = audioManagerGO.AddComponent<InteractionAudioManager>();
        }
        
        audioManager.useSpatialAudio = true;
        audioManager.enableDebugLogs = true;
        
        Debug.Log("✓ InteractionAudioManager setup");
    }
    
    void SetupSystemValidator()
    {
        GameObject validatorGO = GameObject.Find("InteractionSystemValidator");
        if (validatorGO == null)
        {
            validatorGO = new GameObject("InteractionSystemValidator");
        }
        
        InteractionSystemValidator validator = validatorGO.GetComponent<InteractionSystemValidator>();
        if (validator == null)
        {
            validator = validatorGO.AddComponent<InteractionSystemValidator>();
        }
        
        validator.validateOnStart = true;
        validator.enableDetailedLogs = true;
        validator.validationHotkey = KeyCode.F9;
        
        Debug.Log("✓ InteractionSystemValidator setup - Press F9 to validate");
    }
    
    void SetupUIInteractionComponents()
    {
        // Setup InteractionPrompt
        Canvas overlayCanvas = FindCanvasByName("Overlay Canvas");
        if (overlayCanvas != null)
        {
            GameObject promptGO = GameObject.Find("InteractionPrompt");
            if (promptGO == null)
            {
                promptGO = new GameObject("InteractionPrompt");
                promptGO.transform.SetParent(overlayCanvas.transform, false);
                
                InteractionPrompt prompt = promptGO.AddComponent<InteractionPrompt>();
                prompt.useScreenPosition = true;
                prompt.screenPosition = new Vector2(0.5f, 0.8f);
                
                Debug.Log("✓ InteractionPrompt created in Overlay Canvas");
            }
        }
        
        // Setup InteractionContentPanel
        Canvas menuCanvas = FindCanvasByName("Menu Canvas");
        if (menuCanvas != null)
        {
            GameObject contentPanelGO = GameObject.Find("InteractionContentPanel");
            if (contentPanelGO == null)
            {
                contentPanelGO = new GameObject("InteractionContentPanel");
                contentPanelGO.transform.SetParent(menuCanvas.transform, false);
                
                InteractionContentPanel contentPanel = contentPanelGO.AddComponent<InteractionContentPanel>();
                
                Debug.Log("✓ InteractionContentPanel created in Menu Canvas");
            }
        }
        
        Debug.Log("✓ UI components setup completed");
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