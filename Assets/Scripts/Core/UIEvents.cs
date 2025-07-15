using UnityEngine;

// UI-specific EventBus events for the RPG system
public interface IUIEvent : IGameEvent
{
    // Marker interface for UI events
}

// Character stat update events for HUD display
public class CharacterStatsUpdatedEvent : IUIEvent
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
    public float CurrentMP { get; private set; }
    public float MaxMP { get; private set; }
    public float CurrentXP { get; private set; }
    public float XPToNext { get; private set; }
    public int Level { get; private set; }
    public float Timestamp { get; private set; }
    
    public CharacterStatsUpdatedEvent(float currentHealth, float maxHealth, 
                                     float currentMP, float maxMP,
                                     float currentXP, float xpToNext, int level)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        CurrentMP = currentMP;
        MaxMP = maxMP;
        CurrentXP = currentXP;
        XPToNext = xpToNext;
        Level = level;
        Timestamp = Time.time;
    }
    
    // Helper methods for UI calculations
    public float GetHealthPercentage()
    {
        return MaxHealth > 0 ? CurrentHealth / MaxHealth : 0f;
    }
    
    public float GetMPPercentage()
    {
        return MaxMP > 0 ? CurrentMP / MaxMP : 0f;
    }
    
    public float GetXPPercentage()
    {
        return XPToNext > 0 ? CurrentXP / XPToNext : 0f;
    }
}

// Settings change events for system integration
public class SettingsChangedEvent : IUIEvent
{
    public SettingsType Type { get; private set; }
    public float Value { get; private set; }
    public bool BoolValue { get; private set; }
    public string StringValue { get; private set; }
    public float Timestamp { get; private set; }
    
    // Audio settings constructor
    public SettingsChangedEvent(SettingsType type, float value)
    {
        Type = type;
        Value = value;
        BoolValue = false;
        StringValue = "";
        Timestamp = Time.time;
    }
    
    // Boolean settings constructor
    public SettingsChangedEvent(SettingsType type, bool boolValue)
    {
        Type = type;
        Value = 0f;
        BoolValue = boolValue;
        StringValue = "";
        Timestamp = Time.time;
    }
    
    // String settings constructor
    public SettingsChangedEvent(SettingsType type, string stringValue)
    {
        Type = type;
        Value = 0f;
        BoolValue = false;
        StringValue = stringValue;
        Timestamp = Time.time;
    }
}

public enum SettingsType
{
    MasterVolume,
    SFXVolume,
    MusicVolume,
    AmbientVolume,
    Fullscreen,
    VSync,
    Resolution,
    GraphicsQuality
}

// UI state change events for input routing
public class UIStateChangedEvent : IUIEvent
{
    public UIState State { get; private set; }
    public bool IsUIMode { get; private set; }
    public string PanelName { get; private set; }
    public float Timestamp { get; private set; }
    
    public UIStateChangedEvent(UIState state, bool isUIMode, string panelName = "")
    {
        State = state;
        IsUIMode = isUIMode;
        PanelName = panelName;
        Timestamp = Time.time;
    }
}

public enum UIState
{
    Game,
    Menu,
    Settings,
    Inventory,
    Dialogue,
    Combat
}

// Interaction prompt events for dynamic prompts
public class InteractionPromptEvent : IUIEvent
{
    public InteractionAction Action { get; private set; }
    public string PromptText { get; private set; }
    public float Duration { get; private set; }
    public InteractionType Type { get; private set; }
    public float Timestamp { get; private set; }
    
    public InteractionPromptEvent(InteractionAction action, string promptText = "", 
                                 float duration = 0f, InteractionType type = InteractionType.General)
    {
        Action = action;
        PromptText = promptText;
        Duration = duration;
        Type = type;
        Timestamp = Time.time;
    }
}

public enum InteractionAction
{
    Show,
    Hide,
    Update
}

public enum InteractionType
{
    General,
    Door,
    NPC,
    Item,
    Combat,
    Dialogue
}

// Panel transition events for smooth UI flow
public class PanelTransitionEvent : IUIEvent
{
    public TransitionAction Action { get; private set; }
    public string PanelName { get; private set; }
    public float Duration { get; private set; }
    public bool Immediate { get; private set; }
    public float Timestamp { get; private set; }
    
    public PanelTransitionEvent(TransitionAction action, string panelName, 
                               float duration = 0.3f, bool immediate = false)
    {
        Action = action;
        PanelName = panelName;
        Duration = duration;
        Immediate = immediate;
        Timestamp = Time.time;
    }
}

public enum TransitionAction
{
    Show,
    Hide,
    Toggle
}

// Notification events for user feedback
public class NotificationEvent : IUIEvent
{
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public float Duration { get; private set; }
    public bool RequireConfirmation { get; private set; }
    public float Timestamp { get; private set; }
    
    public NotificationEvent(string message, NotificationType type = NotificationType.Info, 
                           float duration = 3f, bool requireConfirmation = false)
    {
        Message = message;
        Type = type;
        Duration = duration;
        RequireConfirmation = requireConfirmation;
        Timestamp = Time.time;
    }
}

public enum NotificationType
{
    Info,
    Warning,
    Error,
    Success
}

// Input mode change events
public class InputModeChangedEvent : IUIEvent
{
    public InputMode Mode { get; private set; }
    public InputMode PreviousMode { get; private set; }
    public float Timestamp { get; private set; }
    
    public InputModeChangedEvent(InputMode mode, InputMode previousMode)
    {
        Mode = mode;
        PreviousMode = previousMode;
        Timestamp = Time.time;
    }
}

public enum InputMode
{
    Game,
    UI,
    Disabled
}