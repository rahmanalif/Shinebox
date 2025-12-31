# Level Setup Guide - Phutt Phutt Golf

## Creating New Levels

### Step 1: Duplicate Existing Level
1. In Unity, go to `Assets/Scenes/`
2. Right-click on "Phutt Phutt Golf.unity"
3. Select "Duplicate"
4. Rename to "Level_01.unity", "Level_02.unity", etc.

### Step 2: Level Design Recommendations

#### Level 1 (Easy - Par 3)
- Simple straight shot to goal
- 1-2 wall bounces
- No obstacles
- Goal: Teach basic mechanics

#### Level 2 (Easy - Par 3)
- Add 1 Boost Pad
- Simple curved path
- Goal: Introduce boost mechanic

#### Level 3 (Medium - Par 4)
- Add 1-2 Bouncy Bumpers
- More complex wall layout
- 1 Sand Trap (optional path)
- Goal: Introduce obstacles

#### Level 4 (Medium - Par 4)
- Add 1 Moving Platform
- Combine Boost Pad + Bumpers
- Multiple paths to goal
- Goal: Strategic thinking

#### Level 5 (Hard - Par 5)
- Combine all obstacle types
- Tight spaces requiring precision
- Moving obstacles
- Multiple Sand Traps
- Goal: Master all mechanics

### Step 3: Adding Obstacles to Scene

#### Boost Pad Setup
1. Create new GameObject → 2D Object → Sprite (or any shape)
2. Add Component → Box Collider 2D
3. Set "Is Trigger" to TRUE
4. Add Component → Boost Pad script
5. Configure:
   - Boost Multiplier: 2.0 (doubles speed)
   - OR Use Directional Boost + set direction
6. Tag object as "BoostPad" (optional for organization)

#### Sand Trap Setup
1. Create new GameObject → 2D Object → Sprite
2. Add Component → Box Collider 2D
3. Set "Is Trigger" to TRUE
4. Add Component → Sand Trap script
5. Configure:
   - Slow Down Rate: 0.9 (higher = more friction)
   - Friction Multiplier: 0.5
6. Change sprite color to sandy yellow/brown

#### Bouncy Bumper Setup
1. Create new GameObject → 2D Object → Circle/Sprite
2. Add Component → Circle Collider 2D (or Box Collider 2D)
3. Set "Is Trigger" to FALSE
4. Add Component → Rigidbody 2D
5. Set Rigidbody2D to "Kinematic"
6. Add Component → Bouncy Bumper script
7. Configure:
   - Bounce Force: 15.0
   - Use Reflection: TRUE
8. Add a bright colored sprite (red/yellow)

#### Moving Platform Setup
1. Create new GameObject → 2D Object → Sprite
2. Add Component → Box Collider 2D
3. Set "Is Trigger" to FALSE
4. Add Component → Rigidbody 2D
5. Set Rigidbody2D to "Kinematic"
6. Add Component → Moving Platform script
7. Configure in Inspector:
   - Point A: Start position (local coordinates)
   - Point B: End position (local coordinates)
   - Speed: 2.0
   - Movement Type: PingPong
8. You'll see cyan lines in Scene view showing movement path

### Step 4: Configure GameManager in Each Level
1. Find GameManager GameObject in scene
2. Update in Inspector:
   - Par For Level: Set appropriate par (3-5)
   - Next Level Name: Name of next level scene (e.g., "Level_02")

### Step 5: Add Levels to Build Settings
1. Go to File → Build Settings
2. Click "Add Open Scenes" for each new level
3. Ensure levels are in correct order

### Step 6: Configure LevelManager (Optional)
If using LevelManager for automatic progression:
1. Create empty GameObject in first level
2. Add LevelManager script
3. In Inspector, add all level names to array:
   - Level Scene Names: ["Level_01", "Level_02", "Level_03", etc.]
   - Par For Each Level: [3, 3, 4, 4, 5]

## Level Design Tips

### Difficulty Progression
- **Par 3 (Easy)**: Direct path, minimal obstacles
- **Par 4 (Medium)**: Requires planning, some obstacles
- **Par 5 (Hard)**: Complex paths, many obstacles, precision required

### Obstacle Placement
- **Boost Pads**: Place before long stretches or to help reach high areas
- **Sand Traps**: Use to punish poor shots or create risk/reward scenarios
- **Bouncy Bumpers**: Create chaos or require precise angles
- **Moving Platforms**: Add timing challenges

### Testing Checklist
- [ ] Can complete level in par strokes (test multiple times)
- [ ] Ball can't get permanently stuck
- [ ] Goal is visible from start (or path is obvious)
- [ ] All obstacles work correctly
- [ ] Scene transitions to next level properly

## Power Meter UI Setup

Each level needs a Power Meter UI:

1. In scene, create UI → Canvas (if not exists)
2. Under Canvas, create UI → Panel (name it "PowerMeterPanel")
3. Add UI → Image to PowerMeterPanel (name it "PowerBackground")
4. Add UI → Image to PowerMeterPanel (name it "PowerFill")
5. Configure PowerFill:
   - Image Type: Filled
   - Fill Method: Horizontal
   - Fill Amount: 1
6. Find Ball GameObject in scene
7. Select golf Script component
8. Add PowerMeterUI component to Canvas or PowerMeterPanel
9. Configure PowerMeterUI:
   - Power Meter Panel: Drag PowerMeterPanel
   - Power Fill Image: Drag PowerFill
   - Power Background Image: Drag PowerBackground
10. In golf Script, assign Power Meter UI reference

## Scene Checklist for Each Level

Required GameObjects:
- [x] Ball (with golf Script, Rigidbody2D, Collider2D)
- [x] Goal (with GoalScript, Trigger Collider2D)
- [x] GameManager (with par and next level configured)
- [x] Canvas with Power Meter UI
- [x] Walls/Boundaries
- [ ] Obstacles (varies by level)
- [x] Camera (Main Camera)

Required Tags:
- Ball: "Ball"
- Goal: "Untagged" (GoalScript uses trigger detection)

Required Layers:
- Default layer is fine for most objects
- Ensure Ball and obstacles can collide (check Physics2D collision matrix)
