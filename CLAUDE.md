# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a 3D RPG Unity project using Unity 6000.2.0b1 with Universal Render Pipeline (URP). The project is in early development stages with basic setup complete.

## Key Unity Dependencies

- **Unity Input System**: Version 1.14.0 - Modern input handling system
- **Universal Render Pipeline**: Version 17.2.0 - Unity's modern rendering pipeline
- **AI Navigation**: Version 2.0.7 - NavMesh and pathfinding
- **Visual Scripting**: Version 1.9.6 - Node-based scripting system
- **Timeline**: Version 1.8.7 - Cinematic and animation sequencing
- **Test Framework**: Version 1.5.1 - Unit testing framework

## Project Structure

```
Assets/
├── Audio/          # Audio assets and sound effects
├── Materials/      # Material assets for rendering
├── Prefabs/        # Reusable game objects
├── Scenes/         # Unity scene files
├── Scripts/        # C# scripts and code
├── Settings/       # URP and rendering settings
└── UI/            # User interface assets
```

## Input System Configuration

The project uses Unity's Input System with predefined action maps:

### Player Actions
- **Move**: WASD/Arrow keys, Gamepad left stick
- **Look**: Mouse delta, Gamepad right stick  
- **Attack**: Left mouse button, Gamepad X button, Enter key
- **Jump**: Spacebar, Gamepad A button
- **Sprint**: Left Shift, Gamepad left stick press
- **Crouch**: C key, Gamepad B button
- **Interact**: E key, Gamepad Y button (Hold interaction)
- **Previous/Next**: 1/2 keys, Gamepad D-pad

### UI Actions
Standard UI navigation with mouse, keyboard, and gamepad support.

## Development Commands

Unity projects are typically developed through the Unity Editor rather than command line. Key development workflows:

### Opening the Project
- Open Unity Hub
- Click "Add" and select the project folder
- Open with Unity 6000.2.0b1 or compatible version

### Building the Project
- Use Unity Editor: File > Build Settings
- Select target platform and click "Build"
- Build output goes to a folder you specify

### Testing
- Use Unity Test Runner: Window > General > Test Runner
- Create tests in `Assets/Scripts/` with `[Test]` and `[UnityTest]` attributes
- Tests require Unity Test Framework package (already included)

## Architecture Notes

### Rendering Pipeline
The project uses Universal Render Pipeline with separate configurations for:
- **PC**: High-quality settings for desktop platforms
- **Mobile**: Optimized settings for mobile devices
- Volume profiles for post-processing effects

### Input Handling
Modern Unity Input System implementation:
- Actions defined in `Assets/InputSystem_Actions.inputactions`
- Support for multiple input devices (Keyboard/Mouse, Gamepad, Touch, XR)
- Composite bindings for complex input (e.g., WASD movement)

### Code Organization
- Main scripts in `Assets/Scripts/`
- Follow Unity C# naming conventions
- Use MonoBehaviour for Unity components
- Implement Unity lifecycle methods (Start, Update, etc.)

## Common Patterns

### Unity Script Structure
```csharp
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    void Start()
    {
        // Initialization
    }
    
    void Update()
    {
        // Per-frame updates
    }
}
```

### Input System Usage
Reference the InputSystem_Actions asset and use generated C# class or PlayerInput component for input handling.

## Development Guidelines

- Use Unity's component-based architecture
- Leverage prefabs for reusable game objects
- Follow Unity's naming conventions for scripts and assets
- Use Unity's built-in physics and collision systems
- Implement proper object pooling for performance-critical objects
- Use Unity's Profiler for performance analysis
- Test on target platforms regularly

# Generic RPG Foundation - Episode 1

## Project Overview
3D low-poly JRPG built in Unity 2022.3 LTS using Built-in Render Pipeline. Follows "generic skeleton first" philosophy - build standard RPG systems first, add unique mechanics later.

**Always-Playable Development:** Every milestone produces a complete, demonstrable game experience.

## Technical Specifications
- **Platform:** PC Windows primary, 30+ FPS stable target
- **Hardware:** 5-year-old mid-range PCs, integrated graphics support
- **Control:** Mouse + Keyboard primary
- **Art Style:** Low-poly 3D with Unity Asset Store foundation

## Current Development State
- **Active Milestone:** 1.1 Core Architecture
- **Target:** GameManager, InputManager, PlayerController, CameraController, EventBus
- **Game State:** Player moves in bounded 3D village space with isometric camera

## Architecture Principles
- **Singletons for managers:** GameManager, InputManager, AudioManager, UIManager
- **Component-based player systems:** PlayerController, CharacterStats, etc.
- **Event-driven communication:** EventBus for system communication
- **Simple maintainable code:** Clarity over complexity during foundation phase
- **Performance first:** 30+ FPS stable, optimize for minimum hardware

## Project Structure
Assets/
├── Scripts/
│   ├── Core/           # GameManager, InputManager, EventBus
│   ├── Player/         # PlayerController, CharacterSystem
│   ├── Combat/         # CombatManager, TurnController (future)
│   ├── UI/             # UIManager, HUDController (future)
│   └── Utilities/      # Helper scripts, extensions
├── Prefabs/
│   ├── Characters/     # Player, NPCs, Enemies
│   ├── Environment/    # Buildings, props, interactables
│   └── UI/             # Interface prefabs
├── Scenes/
│   ├── TestVillage     # Current development scene
│   └── Millhaven       # Main village (future)
├── Materials/          # 3D materials, shaders
├── Audio/              # Music, SFX (future milestones)
└── UI/                 # Interface graphics, fonts

## Core System Architecture
GameManager (Central Hub)
├── InputManager → PlayerController → EventBus
├── CameraController → Player following
└── (Future: CombatSystem, InventorySystem, UIManager, AudioManager)

## Current Systems Status
✅ **GameManager:** Singleton, game state management, DontDestroyOnLoad  
✅ **InputManager:** WASD/mouse input, C# Action events, enable/disable  
✅ **PlayerController:** Physics movement, 5 units/second, smooth acceleration  
✅ **CameraController:** Isometric follow camera, smooth tracking  
✅ **EventBus:** Generic event system, PlayerMovedEvent, GameStateChangedEvent  

## Implementation Guidelines

### ALWAYS IMPLEMENT:
- Singleton pattern for managers using DontDestroyOnLoad
- Event-driven communication via EventBus
- Physics-based movement using Rigidbody
- Component-based architecture
- Performance considerations (object pooling, efficient updates)

### NEVER IMPLEMENT (until specified milestone):
- Audio systems (Milestone 9)
- UI beyond basic debug (Milestone 2+)  
- Save/load systems (Milestone 2+)
- Combat mechanics (Milestone 3+)
- Inventory systems (Milestone 5+)
- Dialogue systems (Milestone 7+)

## Code Standards
- **Naming:** PascalCase classes, camelCase variables, descriptive names
- **Comments:** XML docs for public methods, inline for complex logic
- **Performance:** Cache component references, minimize GetComponent calls
- **Events:** Use C# Actions for simple events, EventBus for complex communication
- **Serialization:** [Header] attributes for Inspector organization

## Current Milestone 1.1 Goals
**End State:** Player character moves smoothly in bounded 3D space with proper isometric camera. Core systems (GameManager, InputManager, EventBus) initialized and ready for extension.

**Success Criteria:**
- Player moves in 4 directions at exactly 5 units/second
- Camera maintains isometric view and follows smoothly  
- All core systems initialize without errors
- Event system functional between components
- 30+ FPS stable performance
- Foundation ready for Milestone 1.2 (Village Environment)

## Asset Acquisition Strategy
- **Unity Asset Store first:** Buildings, characters, environments
- **Custom only when necessary:** Unique game-specific elements
- **Performance budget:** 1000-3000 tris characters, 200-1000 tris props
- **Texture resolution:** 512x512 characters, 256x256 props, 1024x1024 environments

## Testing Requirements
- **Every commit builds and runs** without console errors
- **Performance testing:** Profiler confirms 30+ FPS target
- **Integration testing:** All systems work together
- **Milestone validation:** Specific success criteria met before proceeding

---

When implementing new features:
1. Check current milestone requirements first
2. Use existing architecture patterns  
3. Follow naming conventions and project structure
4. Test integration with existing systems
5. Optimize for target performance
6. Document any architectural decisions