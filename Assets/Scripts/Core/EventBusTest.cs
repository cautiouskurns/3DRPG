using UnityEngine;

public class EventBusTest : MonoBehaviour
{
    void Start()
    {
        // Subscribe to game events
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Subscribe<SystemsInitializedEvent>(OnSystemsInitialized);
        EventBus.Subscribe<InputStateChangedEvent>(OnInputStateChanged);
        
        Debug.Log("EventBusTest: Subscribed to all events");
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Unsubscribe<SystemsInitializedEvent>(OnSystemsInitialized);
        EventBus.Unsubscribe<InputStateChangedEvent>(OnInputStateChanged);
    }
    
    void Update()
    {
        // Test EventBus with key presses
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Test game state change
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeGameState(GameState.Combat);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Test input enable/disable
            if (InputManager.Instance != null)
            {
                InputManager.Instance.EnableInput(!InputManager.Instance.inputEnabled);
            }
        }
    }
    
    private void OnGameStateChanged(GameStateChangedEvent gameEvent)
    {
        Debug.Log($"EventBusTest: Game state changed from {gameEvent.PreviousState} to {gameEvent.NewState} at {gameEvent.Timestamp}");
    }
    
    private void OnSystemsInitialized(SystemsInitializedEvent gameEvent)
    {
        Debug.Log($"EventBusTest: System {gameEvent.SystemName} initialized at {gameEvent.Timestamp}");
    }
    
    private void OnInputStateChanged(InputStateChangedEvent gameEvent)
    {
        Debug.Log($"EventBusTest: Input {(gameEvent.InputEnabled ? "enabled" : "disabled")} at {gameEvent.Timestamp}");
    }
}