# Unity AI Material Creation Prompts

Use these prompts with Unity's AI Material Creator to generate high-quality materials for your village system.

## Ground Materials

### 1. Grass Material
```
Create a realistic grass material for a medieval village ground. The grass should be vibrant green with natural variation, suitable for a fantasy RPG setting. Include subtle dirt patches and worn areas. The material should tile seamlessly and work well in orthographic/isometric camera view. Color: medium green with earthy undertones.
```

### 2. Stone Path Material
```
Create a medieval cobblestone path material with weathered gray stones. The stones should be irregularly shaped with moss growing between cracks. Include subtle color variation from light to dark gray. The material should look well-traveled and ancient, suitable for connecting village buildings. Make it tileable for long pathways.
```

### 3. Dirt Material
```
Create a packed dirt material for a blacksmith work area. The dirt should be brown with reddish clay undertones, showing signs of heavy use and foot traffic. Include small stones and debris scattered throughout. The surface should appear compacted and slightly rough. Color: medium to dark brown with clay accents.
```

### 4. Cobblestone Material
```
Create a formal cobblestone material for a village town square. Use refined, uniform stones in light gray color with clean mortar lines. The stones should be well-maintained and polished from foot traffic. Include subtle reflections and a slightly smoother finish than regular stone paths. Color: light gray with subtle blue undertones.
```

## Building Materials

### 5. Town Hall Material
```
Create a prestigious sandstone material for a town hall building. Use warm tan/beige colors with subtle texture variations. The stone should appear well-crafted and official, with clean lines and a slight weathered patina. Include carved details and a semi-matte finish. Color: warm tan/sandy beige.
```

### 6. Shop Material
```
Create a weathered wood material for a merchant shop building. Use medium brown wood planks with visible grain patterns and age marks. Include some color variation between planks and subtle wear from weather exposure. The wood should look sturdy but lived-in. Color: medium brown with darker grain lines.
```

### 7. Inn Material
```
Create a rustic timber material for a cozy inn building. Use light brown wood with warm undertones, showing signs of care and maintenance. Include subtle color variations and a slightly polished finish from regular cleaning. The material should feel welcoming and homey. Color: light brown with golden highlights.
```

### 8. Blacksmith Material
```
Create a dark, soot-stained wood material for a blacksmith workshop. Use very dark brown/charcoal colors with burn marks and soot stains. The wood should appear heavily weathered from heat and smoke exposure. Include some metallic staining and rough texture. Color: dark brown to charcoal black.
```

### 9. Chapel Material
```
Create a pristine limestone material for a religious chapel building. Use very light gray/off-white colors with smooth, clean surfaces. The stone should appear well-maintained and sacred, with subtle carved details. Include a slight luminous quality and clean finish. Color: light gray/off-white.
```

### 10. House Material
```
Create a modest timber and plaster material for a residential house. Combine light wood beams with cream-colored plaster sections. The material should show gentle aging but good maintenance. Include subtle color variations and a lived-in appearance. Colors: light wood brown and cream plaster.
```

## Prop Materials

### 11. Barrel Material
```
Create a wooden barrel material with metal bands. Use medium brown oak wood with visible grain and age rings. Include dark metal hoops with rust stains and wear marks. The wood should appear sturdy but weathered from outdoor storage. Colors: medium brown wood with dark metal accents.
```

### 12. Crate Material
```
Create a rough wooden crate material using pine or similar light wood. Include visible wood planks, nail marks, and shipping wear. The wood should appear functional and utilitarian with some scratches and dents. Color: light brown pine wood with darker stain marks.
```

### 13. Fence Material
```
Create a simple wooden fence material using basic timber. The wood should be medium brown with natural grain patterns and weathering from outdoor exposure. Include some splitting and age marks but maintain structural integrity. Color: medium brown with gray weathering.
```

### 14. Rock Material
```
Create a natural stone material for decorative rocks. Use gray granite with speckled texture and natural color variations. The stone should appear solid and natural with weathered surfaces. Include subtle moss growth in crevices. Color: medium gray with darker and lighter speckles.
```

## Usage Instructions

1. **Open Unity AI Material Creator**
2. **Copy and paste each prompt** into the AI material generator
3. **Adjust settings** as needed for your specific art style
4. **Save materials** to `Assets/Materials/` folder with descriptive names
5. **Assign materials** to the corresponding fields in BasicGroundSetup and BasicVillageLayout scripts

## Material Assignment

After creating materials, assign them in Unity:

**BasicGroundSetup component:**
- Grass Material → Your generated grass material
- Stone Path Material → Your generated stone path material  
- Dirt Material → Your generated dirt material
- Cobble Material → Your generated cobblestone material

**BasicVillageLayout component:**
- Leave prefab fields empty to use placeholder buildings with generated materials

The scripts will automatically apply these materials to the appropriate objects during village creation.