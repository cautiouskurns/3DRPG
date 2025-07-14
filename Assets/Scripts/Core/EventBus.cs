using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> eventHandlers = new Dictionary<Type, List<Delegate>>();
    
    /// <summary>
    /// Subscribe to an event type
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="handler">Event handler function</param>
    public static void Subscribe<T>(System.Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = new List<Delegate>();
        }
        
        eventHandlers[eventType].Add(handler);
        
        Debug.Log($"EventBus: Subscribed to {eventType.Name}");
    }
    
    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="handler">Event handler function</param>
    public static void Unsubscribe<T>(System.Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Remove(handler);
            
            // Clean up empty handler lists
            if (eventHandlers[eventType].Count == 0)
            {
                eventHandlers.Remove(eventType);
            }
            
            Debug.Log($"EventBus: Unsubscribed from {eventType.Name}");
        }
    }
    
    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="gameEvent">Event instance</param>
    public static void Publish<T>(T gameEvent) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in eventHandlers[eventType])
            {
                try
                {
                    ((System.Action<T>)handler).Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError($"EventBus: Error handling {eventType.Name}: {e.Message}");
                }
            }
            
            Debug.Log($"EventBus: Published {eventType.Name} to {eventHandlers[eventType].Count} subscribers");
        }
    }
    
    /// <summary>
    /// Clear all event subscriptions (useful for cleanup)
    /// </summary>
    public static void Clear()
    {
        eventHandlers.Clear();
        Debug.Log("EventBus: All subscriptions cleared");
    }
    
    /// <summary>
    /// Get number of subscribers for a specific event type
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <returns>Number of subscribers</returns>
    public static int GetSubscriberCount<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        return eventHandlers.ContainsKey(eventType) ? eventHandlers[eventType].Count : 0;
    }
}