using UnityEngine;

[System.Serializable]
public class PlayerStateData
{
    [Header("Position Data")]
    public Vector3 savedPosition;
    public Vector3 savedRotation;
    
    [Header("Game State")]
    public GameState gameState;
    public bool inputEnabled;
    
    [Header("Movement State")]
    public bool wasMoving;
    public Vector2 lastMovementInput;
    
    [Header("Scene Data")]
    public string previousSceneName;
    public float transitionTimestamp;
    
    // Default constructor
    public PlayerStateData()
    {
        savedPosition = Vector3.zero;
        savedRotation = Vector3.zero;
        gameState = GameState.Exploration;
        inputEnabled = true;
        wasMoving = false;
        lastMovementInput = Vector2.zero;
        previousSceneName = "";
        transitionTimestamp = 0f; // Will be set when actually used
    }
    
    // Constructor that saves current player state
    public PlayerStateData(PlayerController player)
    {
        if (player != null)
        {
            // Save position and rotation
            savedPosition = player.transform.position;
            savedRotation = player.transform.rotation.eulerAngles;
            
            // Save movement state
            wasMoving = player.IsMoving();
            lastMovementInput = player.GetMovementInput();
        }
        else
        {
            savedPosition = Vector3.zero;
            savedRotation = Vector3.zero;
            wasMoving = false;
            lastMovementInput = Vector2.zero;
        }
        
        // Save game manager state
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameState = gameManager.currentState;
        }
        else
        {
            gameState = GameState.Exploration;
        }
        
        // Save input manager state
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputEnabled = inputManager.inputEnabled;
        }
        else
        {
            inputEnabled = true;
        }
        
        // Save scene information
        previousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        transitionTimestamp = 0f; // Will be set by transition manager when needed
        
        Debug.Log($"PlayerStateData saved: Position={savedPosition}, Scene={previousSceneName}, GameState={gameState}");
    }
    
    // Apply saved state to player at new position
    public void ApplyToPlayer(PlayerController player, Vector3 newPosition)
    {
        if (player == null)
        {
            Debug.LogError("PlayerStateData.ApplyToPlayer: Player is null!");
            return;
        }
        
        // Set new position (override saved position)
        player.transform.position = newPosition;
        
        // Also set Rigidbody position if it exists to prevent physics conflicts
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.position = newPosition;
            playerRb.linearVelocity = Vector3.zero; // Reset velocity to prevent movement
        }
        
        // Restore rotation (keep player facing same direction)
        player.transform.rotation = Quaternion.Euler(savedRotation);
        
        if (Application.isPlaying)
        {
            Debug.Log($"PlayerStateData: Applied position {newPosition} to player at {player.transform.position}");
        }
        
        // Restore input state
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.inputEnabled = inputEnabled;
        }
        
        // Restore game state
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && gameManager.currentState != gameState)
        {
            gameManager.ChangeGameState(gameState);
        }
        
        // Reset movement state (player should start stationary in new scene)
        // We don't restore movement input to avoid player continuing to move after transition
        
        Debug.Log($"PlayerStateData applied: Position={newPosition}, Rotation={savedRotation}, InputEnabled={inputEnabled}");
    }
    
    // Apply saved state to player at saved position (for returning to same scene)
    public void ApplyToPlayer(PlayerController player)
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning($"PlayerStateData: Using SAVED position {savedPosition} instead of spawn point - this might be wrong!");
        }
        ApplyToPlayer(player, savedPosition);
    }
    
    // Get summary string for debugging
    public string GetSummary()
    {
        return $"Position: {savedPosition:F1}, Scene: {previousSceneName}, GameState: {gameState}, InputEnabled: {inputEnabled}";
    }
    
    // Check if state data is valid
    public bool IsValid()
    {
        // Basic validation checks
        bool positionValid = !float.IsNaN(savedPosition.x) && !float.IsNaN(savedPosition.y) && !float.IsNaN(savedPosition.z);
        bool rotationValid = !float.IsNaN(savedRotation.x) && !float.IsNaN(savedRotation.y) && !float.IsNaN(savedRotation.z);
        bool sceneValid = !string.IsNullOrEmpty(previousSceneName);
        
        return positionValid && rotationValid && sceneValid;
    }
    
    // Create a copy of this state data
    public PlayerStateData Copy()
    {
        PlayerStateData copy = new PlayerStateData();
        copy.savedPosition = savedPosition;
        copy.savedRotation = savedRotation;
        copy.gameState = gameState;
        copy.inputEnabled = inputEnabled;
        copy.wasMoving = wasMoving;
        copy.lastMovementInput = lastMovementInput;
        copy.previousSceneName = previousSceneName;
        copy.transitionTimestamp = transitionTimestamp;
        
        return copy;
    }
    
    // Set transition timestamp (called when transition actually starts)
    public void SetTransitionTimestamp()
    {
        transitionTimestamp = Time.time;
    }
}