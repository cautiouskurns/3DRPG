using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public GameState currentState = GameState.Exploration;
    
    [Header("Core Systems")]
    public bool systemsInitialized = false;
    
    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Initialize core systems
        InitializeCoreSystem();
    }
    
    public void ChangeGameState(GameState newState)
    {
        if (currentState != newState)
        {
            GameState previousState = currentState;
            currentState = newState;
            
            Debug.Log($"Game State changed from {previousState} to {newState}");
            
            // Broadcast state change event using EventBus
            EventBus.Publish(new GameStateChangedEvent(previousState, newState));
            
            // Keep the old event for backwards compatibility
            OnGameStateChanged?.Invoke(previousState, newState);
        }
    }
    
    public void InitializeCoreSystem()
    {
        // Hook for future systems initialization
        // For now, just mark systems as initialized
        systemsInitialized = true;
        Debug.Log("Core systems initialized");
        
        // Broadcast systems initialized event
        EventBus.Publish(new SystemsInitializedEvent("GameManager"));
    }
    
    public bool AreSystemsReady()
    {
        return systemsInitialized;
    }
    
    // Event for other systems to subscribe to state changes
    public System.Action<GameState, GameState> OnGameStateChanged;
}

public enum GameState
{
    MainMenu,
    Exploration, 
    Combat,
    Dialogue,
    Inventory,
    Paused
}