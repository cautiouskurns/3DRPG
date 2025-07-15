using UnityEngine;

/// <summary>
/// Base interface for all game events
/// </summary>
public interface IGameEvent
{
    float Timestamp { get; }
}

/// <summary>
/// Base class for game events with automatic timestamp
/// </summary>
public abstract class GameEvent : IGameEvent
{
    public float Timestamp { get; private set; }
    
    protected GameEvent()
    {
        Timestamp = Time.time;
    }
}

/// <summary>
/// Event fired when player moves
/// </summary>
public class PlayerMovedEvent : GameEvent
{
    public Vector3 NewPosition { get; }
    public Vector3 PreviousPosition { get; }
    public Vector2 MovementInput { get; }
    
    public PlayerMovedEvent(Vector3 newPosition, Vector3 previousPosition, Vector2 movementInput)
    {
        NewPosition = newPosition;
        PreviousPosition = previousPosition;
        MovementInput = movementInput;
    }
}

/// <summary>
/// Event fired when game state changes
/// </summary>
public class GameStateChangedEvent : GameEvent
{
    public GameState PreviousState { get; }
    public GameState NewState { get; }
    
    public GameStateChangedEvent(GameState previousState, GameState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}

/// <summary>
/// Event fired when input is enabled/disabled
/// </summary>
public class InputStateChangedEvent : GameEvent
{
    public bool InputEnabled { get; }
    
    public InputStateChangedEvent(bool inputEnabled)
    {
        InputEnabled = inputEnabled;
    }
}

/// <summary>
/// Event fired when systems are initialized
/// </summary>
public class SystemsInitializedEvent : GameEvent
{
    public string SystemName { get; }
    
    public SystemsInitializedEvent(string systemName)
    {
        SystemName = systemName;
    }
}

/// <summary>
/// Event fired when game object is interacted with
/// </summary>
public class GameObjectInteractedEvent : GameEvent
{
    public GameObject InteractedObject { get; }
    public string ObjectName { get; }
    public Vector3 Position { get; }
    
    public GameObjectInteractedEvent(GameObject obj, string objectName)
    {
        InteractedObject = obj;
        ObjectName = objectName;
        Position = obj != null ? obj.transform.position : Vector3.zero;
    }
}