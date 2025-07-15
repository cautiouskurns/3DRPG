# Task 1.2.C: Environmental Systems - Unity Setup Instructions

## Overview
Complete environmental system implementation with lighting management, audio foundation, atmospheric effects, and validation tools.

## Scripts Created
1. **BasicLightingManager.cs** - Lighting management with exterior/interior presets
2. **AudioManager.cs** - Complete audio foundation with hook methods
3. **BasicAtmosphereManager.cs** - Atmospheric effects (chimney smoke, particles)
4. **VillageEnvironmentValidator.cs** - Comprehensive validation and performance monitoring

## Unity Setup Steps

### 1. Basic Lighting Manager Setup
1. Create empty GameObject named "BasicLightingManager"
2. Add `BasicLightingManager.cs` script
3. Configure in Inspector:
   - **Exterior Lighting**: Default values are optimal
   - **Interior Lighting**: Default values are optimal
   - **Performance**: Max Realtime Lights = 4, Enable Shadows = true
   - **Debug**: Enable Debug Logs = true

### 2. Audio Manager Setup
1. Create empty GameObject named "AudioManager"
2. Add `AudioManager.cs` script
3. Configure in Inspector:
   - **Volume Settings**: Master=1.0, Music=0.7, SFX=1.0, Ambient=0.6, UI=1.0
   - **Audio Settings**: Mute on Focus Loss = true, Enable Spatial Audio = true
   - **Debug**: Enable Debug Logs = true
4. AudioSources will be created automatically at runtime

### 3. Basic Atmosphere Manager Setup
1. **Create Atmospheric Tag** (Optional but recommended):
   - Go to Edit > Project Settings > Tags and Layers
   - Under "Tags", click "+" and add "Atmospheric"
   - This helps organize atmospheric GameObjects (script works without it)

2. Create empty GameObject named "BasicAtmosphereManager" in Village scene
3. Add `BasicAtmosphereManager.cs` script
4. Configure in Inspector:
   - **Atmospheric Effects**: Enable Atmospheric Effects = true
   - **Effect Settings**: Max Active Particles = 50, Smoke Emission Rate = 10
   - **Performance**: Optimize For Performance = true
   - **Debug**: Enable Debug Logs = true, Show Gizmos = true
5. Smoke spawn points will be created automatically at runtime

### 4. Village Environment Validator Setup
1. Create empty GameObject named "VillageEnvironmentValidator"
2. Add `VillageEnvironmentValidator.cs` script
3. Configure in Inspector:
   - **Validation Settings**: Performance Check Duration = 10s, Target FPS = 30
   - **Test Scenes**: Add all interior scene names:
     - "TownHall_Interior"
     - "Shop_Interior" 
     - "Inn_Interior"
     - "Blacksmith_Interior"
     - "Chapel_Interior"
     - "House_Interior"
   - **Debug**: Enable Debug Logs = true

### 5. Scene Integration
The lighting system automatically integrates with SimpleSceneTransition:
- Exterior lighting applied for Village scene
- Interior lighting applied for all *_Interior scenes
- Lighting changes happen automatically during scene transitions

## Validation Hotkeys
- **P**: Run complete village environment validation
- **L**: Validate lighting system and consistency
- **M**: Check current performance and FPS monitoring
- **V**: Run previous village validation (compatibility)
- **T**: Run scene transition validation

## Expected Results

### Village Exterior (Village.unity)
- Soft blue ambient lighting (outdoor daylight feel)
- Warm sunlight directional light at 30° angle
- Chimney smoke effects on 3 buildings (TownHall, Blacksmith, Inn)
- 30+ FPS stable performance

### Interior Scenes (All *_Interior.unity)
- Warm white ambient lighting (indoor feel)
- Softer directional light (window light simulation)
- No atmospheric effects (interiors are clean)
- Consistent lighting feel across all interiors

### Audio Foundation
- Complete AudioSource setup (Music, SFX, Ambient, UI)
- Volume control system functional
- Spatial audio configuration ready
- Hook methods ready for future audio content

### Performance
- Maximum 3-4 real-time lights per scene
- Maximum 50 active particles total
- Stable 30+ FPS throughout village experience
- No memory leaks or performance degradation

## Testing Instructions

### Quick Validation
1. Start in Village scene
2. Press **P** to run full validation
3. Verify all systems show as ready/functional
4. Press **M** to check current performance

### Complete Village Tour
1. Walk around village exterior
2. Enter each building (6 interiors total)
3. Exit back to village
4. Monitor lighting transitions (should be automatic)
5. Check atmospheric effects are visible and subtle
6. Monitor FPS during extended exploration

### Performance Monitoring
1. Press **M** to start 10-second performance test
2. Move around during test for realistic conditions
3. Check results show 30+ FPS average
4. If performance issues, check:
   - Too many real-time lights (should be ≤4)
   - Atmospheric effects enabled with high particle count
   - Other performance-heavy systems running

## Troubleshooting

### Lighting Issues
- If lighting doesn't change during transitions: Check BasicLightingManager is in scene
- If lighting too dark/bright: Adjust ambient/directional intensity values
- If shadows causing performance issues: Disable shadows in BasicLightingManager

### Atmospheric Effects Issues
- If no smoke visible: Check BasicAtmosphereManager enabled and spawn points created
- If performance drops: Reduce Max Active Particles or disable atmospheric effects
- If smoke looks wrong: Check particle system materials and settings

### Audio Issues
- If AudioManager errors: Check all AudioSource components were created
- If spatial audio not working: Verify Enable Spatial Audio is checked
- If volume not working: Check master volume and individual volume sliders

### Performance Issues
- Target 30+ FPS - if below, check:
  - Real-time light count (max 4 recommended)
  - Particle count (max 50 recommended)
  - Shadow quality settings
  - Other scene complexity

## Integration Notes
- **SimpleSceneTransition**: Automatically applies lighting during scene changes
- **GameManager**: All managers follow singleton pattern
- **EventBus**: Lighting and audio managers can respond to game events
- **Existing Systems**: No changes needed to player, camera, or input systems

## Success Criteria Checklist
- [ ] BasicLightingManager singleton functional in scene
- [ ] AudioManager singleton with all AudioSources created
- [ ] BasicAtmosphereManager with visible chimney smoke
- [ ] VillageEnvironmentValidator with working hotkeys
- [ ] Lighting automatically changes during scene transitions
- [ ] Consistent lighting across Village + 6 interior scenes
- [ ] Stable 30+ FPS performance throughout village
- [ ] All existing systems (movement, camera, transitions) still functional
- [ ] No regressions in previous village functionality

## Next Steps
This environmental system provides the foundation for:
- Future audio content integration (Milestone 9)
- Lighting expansion to other environments (forest, dungeons)
- Enhanced atmospheric effects
- Performance monitoring and optimization
- NPC placement and interaction systems