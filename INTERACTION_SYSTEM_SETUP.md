# Interaction System Setup Instructions

## Quick Setup (Recommended)

1. **Open Setup Tool**: In Unity, go to menu `RPG Tools > Setup Interaction System`
2. **Click "Setup Complete System"** - This will automatically create all required components
3. **Press Play** and test by walking near village objects and pressing E

## Manual Setup (If needed)

### Prerequisites
✅ Player object with `PlayerController` component  
✅ Village objects created by `BasicVillageLayout` component  
✅ UI canvases (HUD, Menu, Overlay) from `UIBuilderEditor`  

### Step 1: Core Interaction System
1. Create empty GameObject named "InteractionSystem"
2. Add `InteractionSystem` component
3. Set `Player Transform` to your player object
4. Set `Interaction Radius` to 1.5
5. Enable `Enable Debug Logs` and `Enable Debug Visuals`

### Step 2: Village Interactables
1. Find your "Village" GameObject (or create new "VillageInteractables")
2. Add `VillageInteractables` component
3. Enable `Auto Create Interactables`
4. Enable `Use Existing Objects`
5. Enable `Enable Debug Logs`

### Step 3: Audio System (Optional)
1. Create empty GameObject named "InteractionAudioManager"
2. Add `InteractionAudioManager` component
3. Enable `Use Spatial Audio`
4. Enable `Enable Debug Logs`

### Step 4: UI Components
1. In Overlay Canvas, create child GameObject "InteractionPrompt"
2. Add `InteractionPrompt` component
3. In Menu Canvas, create child GameObject "InteractionContentPanel"
4. Add `InteractionContentPanel` component

### Step 5: Validation Tool (Optional)
1. Create empty GameObject named "InteractionSystemValidator"
2. Add `InteractionSystemValidator` component
3. Enable `Validate On Start`
4. Set `Validation Hotkey` to F9

## Testing the System

### Basic Test
1. **Press Play**
2. **Walk near village buildings/objects** (within 1.5 units)
3. **Look for yellow highlighting** on objects
4. **Press E** to interact and see content panel

### Debug Tools
- **F9**: Run full system validation
- **Scene view**: See interaction radius as cyan wireframe sphere
- **Console**: Debug logs showing interactions

### Expected Interactions
- **6 Buildings**: Town Hall, Shop, Inn, Blacksmith, Chapel, House
- **Village Well**: Mystical spring with lore
- **Notice Board**: Village information
- **Props**: Barrels, Crates, Fences, Rocks with lore
- **Monuments**: Statue, Ancient Runestone, Memorial Plaque

## Troubleshooting

### Nothing happens when pressing E
1. Check Player has `PlayerController` component
2. Verify `InteractionSystem` has player reference assigned
3. Make sure objects have `InteractableObject` components
4. Check interaction radius (default 1.5 units)

### No highlighting on objects
1. Objects need `Renderer` component for highlighting
2. Check `Enable Highlighting` is true on `InteractableObject`
3. Verify objects have materials that can be changed

### No village objects to interact with
1. Run `BasicVillageLayout` to generate village first
2. Make sure `VillageInteractables.autoCreateInteractables` is true
3. Check console for setup logs

### Content panel doesn't show
1. Verify `InteractionContentPanel` exists in Menu Canvas
2. Check `StaticUIManager` is working
3. Look for UI setup errors in console

## Advanced Configuration

### Custom Interaction Audio
1. Create `InteractionAudioSet` ScriptableObject
2. Assign audio clips for different interaction types
3. Assign to `InteractionAudioManager.audioSet`

### Custom Highlighting
1. Create custom highlight materials
2. Assign to `InteractableObject.highlightMaterial`
3. Or assign to `InteractionSystem.highlightMaterial` for global default

### Performance Optimization
- Reduce `InteractionSystem.interactionRadius` for better performance
- Disable `enableDebugVisuals` in production
- Set `enableDebugLogs` to false in production

## System Architecture

```
InteractionSystem (Singleton)
├── Proximity Detection (1.5 unit radius)
├── Object Highlighting (Material swapping)
├── Input Handling (E key via InputManager)
└── Event Broadcasting (via EventBus)

InteractableObject (Base Class)
├── BuildingInteractable (6 buildings)
├── PropInteractable (8+ props)
└── LoreInteractable (3+ monuments)

UI System
├── InteractionPrompt (Shows "Press E" hints)
└── InteractionContentPanel (Shows lore/descriptions)

Audio System
├── InteractionAudioManager (Type-specific sounds)
└── AudioManager Integration (Spatial audio)
```

## Files Created
- `InteractionSystem.cs` - Core interaction logic
- `InteractableObject.cs` - Base interactable class + variants
- `VillageInteractables.cs` - Auto-setup for 15+ objects
- `InteractionContentPanel.cs` - Content display UI
- `InteractionPrompt.cs` - Enhanced interaction prompts
- `InteractionAudioManager.cs` - Audio feedback system
- `InteractionSystemValidator.cs` - Testing/validation tool
- `InteractionSystemSetup.cs` - Scene setup tool

Ready to explore the Kingdom of Aethermoor! 🏰✨