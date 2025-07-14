# Village Setup Instructions - Task 1.2.A

## Unity Scene Configuration

### 1. Create Village GameObjects Hierarchy

In your Unity scene, create the following GameObject hierarchy:

```
Scene
├── Village Systems
│   ├── Village Layout Manager
│   ├── Village Ground Manager  
│   ├── Village Collision Manager
│   └── Village Validator
├── Player (existing)
├── Main Camera (existing)
└── Game Manager (existing)
```

### 2. Attach Scripts to GameObjects

**Village Layout Manager:**
- Add Component: `BasicVillageLayout`
- Leave all prefab fields empty (will use placeholder buildings)
- Set `buildOnStart` to `true`

**Village Ground Manager:**
- Add Component: `BasicGroundSetup`
- Leave all material fields empty (will auto-create)
- Set `setupOnStart` to `true`
- Set `createMaterialsIfMissing` to `true`

**Village Collision Manager:**
- Add Component: `BasicCollisionSetup`
- Set `setupOnStart` to `true`
- Set `buildingTag` to "Building"
- Set `propTag` to "Prop"
- Set `villageSize` to (60, 60)
- Set `boundaryHeight` to 2

**Village Validator:**
- Add Component: `VillageValidationTest`
- Set `enableDebugLogging` to `true`

### 3. Configure Unity Tags

Create the following tags in Unity (Window → Tags and Layers):
- `Building`
- `Prop`
- `Ground`
- `Boundary`

### 4. Player Setup

Ensure your Player GameObject has:
- Position: (0, 1, 0) to spawn in village center
- The existing `PlayerController` script
- Capsule Collider (Height: 2, Radius: 0.5)
- Rigidbody with constraints on rotation X and Z

### 5. Camera Setup

Ensure your Main Camera has:
- Position: (0, 20, -15) for isometric view
- Rotation: (30, 0, 0)
- Projection: Orthographic
- Size: 10
- The existing `CameraController` script targeting the Player

### 6. Running the Village System

1. **Start the Scene**: The village will automatically build when you play
2. **Validation**: Press `V` to validate all systems
3. **Rebuild**: Press `R` to rebuild the village
4. **Movement Test**: Use WASD to move around and test collision

### 7. Expected Results

After setup, you should see:
- 6 placeholder buildings positioned logically around the village
- Ground planes with different materials (grass, stone paths, dirt)
- Invisible collision boundaries preventing player from leaving village
- Decorative props scattered around buildings
- Console logs showing successful validation

### 8. Validation Tests

The system will automatically test:
- ✅ 6+ buildings with collision
- ✅ Ground textures and materials
- ✅ Village boundaries (4 invisible walls)
- ✅ Player integration and movement
- ✅ Performance (30+ FPS target)

### 9. Integration with Core Systems

The village system works with your existing Milestone 1.1 systems:
- **GameManager**: Village loads after game initialization
- **InputManager**: Player movement works within village boundaries
- **EventBus**: Village systems can publish/subscribe to events
- **PlayerController**: Movement at 5 units/second with collision
- **CameraController**: Smooth following within village area

### 10. Troubleshooting

**No buildings appear:**
- Check that `buildOnStart` is true on BasicVillageLayout
- Verify the script is attached to a GameObject in the scene

**No ground textures:**
- Ensure `createMaterialsIfMissing` is true on BasicGroundSetup
- Check that the Ground tag exists in Unity

**Player falls through ground:**
- Verify Player has a Rigidbody and Collider
- Check that ground planes have colliders (they should auto-generate)

**Validation fails:**
- Run the MilestoneValidator (Press O) to ensure core systems work
- Check Unity Console for specific error messages
- Press V to see detailed village validation results

This completes the Basic Village Layout & Assets implementation as specified in Task 1.2.A.