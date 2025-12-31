# Obstacle System Guide - Phutt Phutt Golf

## Overview
This guide covers all four obstacle types and how to use them in your levels.

---

## 1. Boost Pad 🚀

**Purpose**: Speed up the ball, either by multiplying velocity or launching in a specific direction.

### Setup Instructions
1. Create GameObject → 2D Object → Sprite
2. Add `Box Collider 2D` component
3. **IMPORTANT**: Check "Is Trigger" ✓
4. Add `Boost Pad` component

### Configuration Options

#### Mode 1: Velocity Multiplier (Default)
- **Use Directional Boost**: OFF
- **Boost Multiplier**: 2.0 (doubles ball speed)
- Best for: Speeding up existing momentum

#### Mode 2: Directional Launch
- **Use Directional Boost**: ON
- **Boost Direction**: Set X/Y values (e.g., 1, 0 for right)
- **Fixed Boost Force**: 15.0
- Best for: Launching ball in specific direction

### Visual Setup
- Recommended sprite: Arrow or glowing pad
- Recommended colors: Yellow, orange, or bright green
- Add ParticleSystem for boost effect (optional)
- Add AudioClip for boost sound (optional)

### Level Design Tips
- Place before long distances
- Use to create skill shots
- Chain multiple boost pads for speed runs
- Point toward tricky angles

---

## 2. Sand Trap 🏖️

**Purpose**: Slow down the ball significantly, punishing poor shots.

### Setup Instructions
1. Create GameObject → 2D Object → Sprite
2. Add `Box Collider 2D` component
3. **IMPORTANT**: Check "Is Trigger" ✓
4. Add `Sand Trap` component

### Configuration Options
- **Friction Multiplier**: 0.5 (lower = more friction)
- **Slow Down Rate**: 0.9 (per frame slowdown)
- **Minimum Velocity**: 0.1 (ball stops below this)

### Visual Setup
- Recommended color: Sandy yellow/brown (RGB: 245, 222, 179)
- Make it visually distinct from playable area
- Add ParticleSystem for sand dust effect (optional)

### Level Design Tips
- Place near goal as obstacles
- Create risk/reward paths (fast route through sand vs safe route)
- Use large sand traps to heavily punish mistakes
- Combine with walls to create "sand bunkers"

---

## 3. Bouncy Bumper ⚡

**Purpose**: Bounce the ball away with force, creating dynamic challenges.

### Setup Instructions
1. Create GameObject → 2D Object → Circle (or Sprite)
2. Add `Circle Collider 2D` component
3. **IMPORTANT**: "Is Trigger" should be OFF ✗
4. Add `Rigidbody 2D` component
5. Set Rigidbody2D → Body Type: **Kinematic**
6. Add `Bouncy Bumper` component

### Configuration Options

#### Bounce Settings
- **Bounce Force**: 15.0 (how hard to bounce)
- **Use Reflection**: ON (realistic physics) / OFF (simple away-bounce)
- **Minimum Impact Velocity**: 0.5 (won't bounce slow balls)

#### Visual Feedback
- **Bounce Effect**: ParticleSystem (flash on impact)
- **Bounce Sound**: AudioClip (satisfying "boing")
- **Bumper Animator**: Optional animation controller
- **Bounce Animation Trigger**: "Bounce" (trigger name)

### Visual Setup
- Recommended shapes: Circle, hexagon
- Recommended colors: Bright red, electric blue, yellow
- Add glowing effect or outline
- Animate on bounce (scale pulse, rotation)

### Level Design Tips
- Place in narrow corridors for chaos
- Create "pinball" sections with multiple bumpers
- Use to redirect ball toward goal
- Combine with boost pads for extreme speed
- Position to block direct paths to goal

---

## 4. Moving Platform 🔄

**Purpose**: Create timing-based challenges with moving surfaces.

### Setup Instructions
1. Create GameObject → 2D Object → Sprite
2. Add `Box Collider 2D` component
3. **IMPORTANT**: "Is Trigger" should be OFF ✗
4. Add `Rigidbody 2D` component
5. Set Rigidbody2D → Body Type: **Kinematic**
6. Add `Moving Platform` component

### Configuration Options

#### Movement Settings
- **Point A**: Starting position (Vector2, local space)
- **Point B**: Ending position (Vector2, local space)
- **Speed**: 2.0 (units per second)
- **Use Local Space**: ON (recommended)
- **Movement Curve**: AnimationCurve (ease in/out, linear, etc.)

#### Movement Types
- **PingPong**: A → B → A → B (continuous)
- **Loop**: A → B → A → B (teleports back to A)
- **Once**: A → B (stops at B)

#### Wait Settings
- **Wait Time At Points**: 0.5 (seconds to pause at endpoints)

### Visual Setup
- Recommended: Rectangular platform
- Use distinct color (gray, metal, different texture)
- Add moving markers or arrows
- You'll see cyan lines in Scene view showing path

### Editor Gizmos
- **Cyan line**: Movement path
- **Wire spheres**: Point A and Point B
- **Yellow cube**: Current platform position

### Level Design Tips
- Require player to time shots to land on platform
- Move platforms across gaps or hazards
- Combine with other obstacles (sand trap below platform)
- Use "Once" type for one-time bridges
- Create sequences of moving platforms
- Vertical platforms for elevation challenges

### Important Notes
- Ball becomes child of platform while on it (moves with platform)
- Ball unparents when leaving platform
- Set reasonable speed (1-3 is usually good)
- Test movement path in editor using gizmos

---

## Combining Obstacles

### Beginner Combos
- Sand Trap + Walls = Bunkers
- Boost Pad + Open space = Speed sections

### Intermediate Combos
- Bouncy Bumper + Boost Pad = Chaos zone
- Moving Platform + Sand Trap below = Risk/reward timing
- Multiple Bumpers = Pinball section

### Advanced Combos
- Boost Pad → Bumper → Goal = Skill shot
- Moving Platform + Bumpers = Moving chaos
- Sand Trap + Bumpers around goal = Precision required
- Chain of Boost Pads with obstacles between = Speed run

---

## Testing Checklist

For each obstacle in your level:

### Boost Pad
- [ ] Ball speeds up when triggered
- [ ] Direction is correct (if directional)
- [ ] Visual/audio feedback works
- [ ] Can reach intended target after boost

### Sand Trap
- [ ] Ball slows down noticeably
- [ ] Ball can eventually escape (not infinite trap)
- [ ] Visual distinct from normal ground
- [ ] Particle effects trigger

### Bouncy Bumper
- [ ] Ball bounces with appropriate force
- [ ] Doesn't create infinite bounce loops
- [ ] Bounce direction feels correct
- [ ] Visual/audio feedback works
- [ ] Rigidbody2D is Kinematic

### Moving Platform
- [ ] Movement path is visible in editor (gizmos)
- [ ] Speed is appropriate (not too fast/slow)
- [ ] Ball moves with platform
- [ ] Ball releases from platform correctly
- [ ] Rigidbody2D is Kinematic
- [ ] Doesn't push ball through walls

---

## Common Issues & Solutions

### Issue: Boost Pad not working
- ✓ Check "Is Trigger" is enabled on collider
- ✓ Verify Ball has "Ball" tag
- ✓ Check boost settings aren't zero

### Issue: Sand Trap not slowing ball
- ✓ Check "Is Trigger" is enabled
- ✓ Verify slow down rate is < 1.0
- ✓ Ensure ball enters trigger volume

### Issue: Bumper not bouncing
- ✓ Check "Is Trigger" is DISABLED
- ✓ Verify Rigidbody2D is "Kinematic"
- ✓ Check bounce force isn't zero
- ✓ Ball velocity might be below minimum impact velocity

### Issue: Platform not moving
- ✓ Check Point A and Point B are different
- ✓ Verify speed is > 0
- ✓ Ensure Rigidbody2D is "Kinematic"
- ✓ Check if platform completed "Once" movement

### Issue: Ball falls through platform
- ✓ Verify platform has collider
- ✓ Check Rigidbody2D is "Kinematic"
- ✓ Ensure Physics2D collision matrix allows ball-platform collision

### Issue: Ball stuck on obstacle
- ✓ Check collider sizes aren't too large
- ✓ Verify ball's physics material (add if missing)
- ✓ Test with slower speeds

---

## Performance Tips

- Use object pooling if you have many obstacles with effects
- Disable particle systems when not visible
- Keep moving platforms count reasonable (5-10 max per scene)
- Use simple collider shapes (box, circle) instead of polygon when possible
- Combine static obstacles into single sprite/collider when possible

---

## Quick Reference Table

| Obstacle | Trigger? | Rigidbody? | Main Effect |
|----------|----------|------------|-------------|
| Boost Pad | ✓ Yes | No | Speed up ball |
| Sand Trap | ✓ Yes | No | Slow down ball |
| Bouncy Bumper | ✗ No | Kinematic | Bounce ball away |
| Moving Platform | ✗ No | Kinematic | Move ball with platform |

---

Happy level designing! 🏌️‍♂️
