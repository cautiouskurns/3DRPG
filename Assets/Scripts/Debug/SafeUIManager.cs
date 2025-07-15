using UnityEngine;

/// <summary>
/// Safe fallback UIManager that prevents crashes
/// Add this to scene and disable other UIManager components temporarily
/// </summary>
public class SafeUIManager : MonoBehaviour
{
    public static SafeUIManager Instance { get; private set; }
    
    [Header("Safe Mode Settings")]
    public bool enableDebugLogs = true;
    public bool disableEscapeKey = false;
    
    private bool escapePressed = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (enableDebugLogs)
            {
                Debug.Log("SafeUIManager: Safe mode activated - preventing UI crashes");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // Safe escape key handling
        if (Input.GetKeyDown(KeyCode.Escape) && !disableEscapeKey)
        {
            HandleEscapeKeySafely();
        }
    }
    
    private void HandleEscapeKeySafely()
    {
        try
        {
            escapePressed = !escapePressed;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SafeUIManager: Escape pressed - State: {(escapePressed ? "Settings" : "Game")}");
                Debug.Log("SafeUIManager: This is a safe fallback - no actual UI will show");
                Debug.Log("SafeUIManager: Disable this component once UIManager is fixed");
            }
            
            // Don't actually do anything - just log to prevent crashes
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SafeUIManager: Even safe escape handling failed! {e.Message}");
            
            // Disable escape key to prevent further crashes
            disableEscapeKey = true;
            Debug.LogWarning("SafeUIManager: Escape key disabled to prevent crashes");
        }
    }
    
    public void DisableEscapeKey()
    {
        disableEscapeKey = true;
        Debug.Log("SafeUIManager: Escape key manually disabled");
    }
    
    public void EnableEscapeKey()
    {
        disableEscapeKey = false;
        Debug.Log("SafeUIManager: Escape key re-enabled");
    }
}