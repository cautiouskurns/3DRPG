# Scene Transition Setup Instructions - Task 1.2.B

## Unity Scene Configuration

### 1. Create Scene Transition Manager

In your main village scene, create the transition manager:

```
Scene Hierarchy:
├── Scene Transition Manager (empty GameObject)
│   └── SimpleSceneTransition script
├── Interaction Prompt Manager (empty GameObject)  
│   └── InteractionPrompt script
├── Scene Transition Validator (empty GameObject)
│   └── SceneTransitionValidator script
└── Existing village objects...
```

### 2. Setup Door Triggers on Buildings

For each of the 6 buildings in your village, add door trigger components:

**Town Hall Door:**
- Add empty GameObject: "TownHall_Door"
- Position: At building entrance (0, 1, 18) - slightly in front of building
- Add DoorTrigger script with settings:
  - Target Scene Name: "TownHall_Interior"
  - Spawn Point Name: "PlayerSpawn"
  - Door Name: "Town Hall"
  - Interaction Range: 2.5

**Shop Door:**
- GameObject: "Shop_Door" at (15, 1, 3)
- Target Scene: "Shop_Interior"
- Door Name: "Shop"

**Inn Door:**
- GameObject: "Inn_Door" at (-15, 1, 3)
- Target Scene: "Inn_Interior"  
- Door Name: "Inn"

**Blacksmith Door:**
- GameObject: "Blacksmith_Door" at (15, 1, -13)
- Target Scene: "Blacksmith_Interior"
- Door Name: "Blacksmith"

**Chapel Door:**
- GameObject: "Chapel_Door" at (-15, 1, 13)
- Target Scene: "Chapel_Interior"
- Door Name: "Chapel"

**House Door:**
- GameObject: "House_Door" at (0, 1, -18)
- Target Scene: "House_Interior"
- Door Name: "House"

### 3. Create Interior Scenes

Create 6 new scenes in `Assets/Scenes/Interiors/`:

#### TownHall_Interior.unity
```
Scene Contents:
├── Main Camera (copy from village scene, with CameraController)
├── Directional Light
├── Ground Plane (scaled 10x10, positioned at Y=0)
├── PlayerSpawn (empty GameObject at 0, 1, -3)
├── ExitDoor (empty GameObject at 0, 1, 3)
│   └── DoorTrigger script:
│       - Target Scene: "SampleScene" (or your main village scene name)
│       - Spawn Point: "TownHall_ExitSpawn"
│       - Door Name: "Town Hall Exit"
├── Player (copy from village scene with PlayerController)
├── Game Manager (copy from village scene)
├── Input Manager (copy from village scene)
└── Scene Transition Manager (copy with SimpleSceneTransition)
```

#### Shop_Interior.unity
```
Similar setup with:
├── PlayerSpawn at (2, 1, -2)
├── ExitDoor at (2, 1, 2) -> Target: "SampleScene", Spawn: "Shop_ExitSpawn"
```

#### Inn_Interior.unity
```
Similar setup with:
├── PlayerSpawn at (-1, 1, -3)
├── ExitDoor at (-1, 1, 3) -> Target: "SampleScene", Spawn: "Inn_ExitSpawn"
```

#### Blacksmith_Interior.unity
```
Similar setup with:
├── PlayerSpawn at (0, 1, -2)
├── ExitDoor at (0, 1, 2) -> Target: "SampleScene", Spawn: "Blacksmith_ExitSpawn"
```

#### Chapel_Interior.unity
```
Similar setup with:
├── PlayerSpawn at (0, 1, -4)
├── ExitDoor at (0, 1, 4) -> Target: "SampleScene", Spawn: "Chapel_ExitSpawn"
```

#### House_Interior.unity
```
Similar setup with:
├── PlayerSpawn at (0, 1, -2)
├── ExitDoor at (0, 1, 2) -> Target: "SampleScene", Spawn: "House_ExitSpawn"
```

### 4. Add Exit Spawn Points to Village Scene

In your main village scene, add spawn points for returning from interiors:

```
Exit Spawn Points:
├── TownHall_ExitSpawn at (0, 1, 16) - in front of Town Hall
├── Shop_ExitSpawn at (15, 1, 1) - in front of Shop  
├── Inn_ExitSpawn at (-15, 1, 1) - in front of Inn
├── Blacksmith_ExitSpawn at (15, 1, -11) - in front of Blacksmith
├── Chapel_ExitSpawn at (-15, 1, 11) - in front of Chapel
└── House_ExitSpawn at (0, 1, -16) - in front of House
```

**For each spawn point:**
- Create empty GameObject with spawn point name
- Tag: "SpawnPoint"
- Position as specified above

### 5. Configure Unity Build Settings

Add all scenes to Build Settings (File → Build Settings):

```
Scenes in Build:
0. Village (your main village scene)
1. TownHall_Interior
2. Shop_Interior  
3. Inn_Interior
4. Blacksmith_Interior
5. Chapel_Interior
6. House_Interior
```

### 6. Create Unity Tags and Layers

**Tags needed:**
- "SpawnPoint" - for spawn point GameObjects
- "Player" - should already exist on player GameObject

**Layers (optional but recommended):**
- "Interaction" - for door trigger colliders

### 7. Setup Interaction Prompt UI

The InteractionPrompt script will auto-create UI, but for better control:

**Option A - Auto Creation (Recommended):**
- InteractionPrompt script will automatically create world-space canvas
- Set "Auto Create UI" to true
- Assign fallback font if needed

**Option B - Manual Setup:**
- Create Canvas GameObject (World Space)
- Add Text component for prompt display
- Assign to InteractionPrompt script fields

### 8. Component Configuration

**SimpleSceneTransition:**
- Transition Delay: 0.1f
- Preserve Player State: true
- Enable Debug Logs: true (for testing)

**DoorTrigger (on each door):**
- Interaction Range: 2.5f
- Interaction Key: E
- Enable Debug Logs: true (for testing)
- Auto-configure trigger collider: true

**InteractionPrompt:**
- Auto Create UI: true
- Default Prompt Text: "Press E to enter"
- Fade In Speed: 5f
- Fade Out Speed: 10f

### 9. Testing and Validation

**Initial Testing:**
1. Play the village scene
2. Press T to run scene transition validation
3. Check console for validation results
4. Fix any failed tests before proceeding

**Manual Testing:**
1. Walk up to each building door
2. Verify "Press E to enter" prompt appears
3. Press E to enter building
4. Check player spawns at correct position inside
5. Walk to exit door and press E
6. Verify return to village at correct spawn point

**Validation Hotkeys:**
- **T**: Run full scene transition validation
- **Y**: Test door trigger detection
- **U**: Validate core systems integration
- **V**: Run village validation (from previous task)

### 10. Troubleshooting

**Prompt not showing:**
- Check InteractionPrompt script is in scene
- Verify Canvas creation or assignment
- Check door trigger collider setup

**Scene not loading:**
- Verify scene names match exactly in DoorTrigger
- Check scenes are added to Build Settings
- Verify SimpleSceneTransition manager exists

**Player not spawning correctly:**
- Check spawn point GameObjects exist and are named correctly
- Verify spawn points have "SpawnPoint" tag
- Check spawn point positions are above ground

**Core systems not working:**
- Ensure GameManager, InputManager exist in each scene
- Verify DontDestroyOnLoad is working for managers
- Check camera and player are properly configured

**Return transitions not working:**
- Check exact scene name matches in DoorTrigger (case-sensitive)
- Use `SimpleSceneTransition.GetVillageSceneName()` to find correct name
- Verify main village scene is first in Build Settings
- Check spawn points exist with correct names in village scene
- Enable debug logs in SimpleSceneTransition for detailed error info

### 11. Performance Optimization

**For better performance:**
- Keep interior scenes simple (no complex geometry)
- Use basic materials for interior ground planes
- Minimize object count in interior scenes
- Consider using simple lighting setups

### 12. Integration Notes

**Works with existing systems:**
- **Village Layout**: Door triggers are separate from building placement
- **Ground Setup**: Interior scenes have independent ground planes
- **Camera System**: CameraController automatically follows player in new scenes
- **Input System**: All interaction uses existing InputManager E key input
- **Game State**: GameManager state is preserved across scene transitions

**Extensible for future tasks:**
- Interior scenes ready for furniture/decoration placement
- Door system ready for locked doors and key requirements
- Player state system ready for inventory and equipment data
- Transition system ready for loading screens and effects

This setup provides a solid foundation for scene transitions while maintaining integration with all existing core systems from Milestone 1.1 and the village environment from Task 1.2.A.