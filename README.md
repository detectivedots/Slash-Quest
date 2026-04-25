# Slash-Quest

Slash-Quest is a 2D Unity platformer with action combat, ranged attacks, ledge grabbing, enemy encounters, and scene-based progression. The project combines classic side-scrolling movement with swordplay, blocking, and projectile combat.

## Features

- 2D platforming with left/right movement, jumping, and sprinting
- Armed and unarmed states
- Melee combat with attack chains / combos
- Air attacks, including directional variants
- Shield blocking and shield attacks
- Projectile deflection through active combat hitboxes
- Charged arrow shots
- Ledge grabbing and hanging
- Enemy AI and spawners
- Pause menu support
- Save data support
- Mobile-friendly input support

## Gameplay Overview

The player can switch between a combat-ready state and a movement-focused state. While armed, the character can attack with the sword, block with the shield, shoot arrows, and use special air or shield actions. When unarmed, the player moves more freely and can draw the sword again when needed.

Combat is built around timing and state management:

- Ground sword attacks can chain into combo sequences.
- Airborne attacks change depending on movement input.
- Blocking stops movement and can be used defensively.
- Active attack and block states can interact with incoming projectiles.
- Ranged attacks are charged before firing, with stronger shots coming from longer charge time.

Traversal also has some nice platforming tools:

- The player can grab ledges and hang from edges.
- Jumping from a hang state is handled separately from a normal jump.
- Falling out of bounds reduces health and returns the player to the last safe position.

## Screenshots
<img width="1919" height="1079" alt="Screenshot 2026-04-25 081335" src="https://github.com/user-attachments/assets/86c07f82-152c-4f23-9ab6-a53bd3ce3cd3" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082343" src="https://github.com/user-attachments/assets/23a10ff0-b395-4157-ba0d-830dff9e8d23" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082330" src="https://github.com/user-attachments/assets/45731a12-426e-470f-9043-8fe09d29d85b" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082319" src="https://github.com/user-attachments/assets/6f8ac484-1095-42b6-b850-930ff32bf075" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082301" src="https://github.com/user-attachments/assets/351e6e0c-05a2-4748-b8b9-6992b39e04f8" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082237" src="https://github.com/user-attachments/assets/2ab652d0-bdba-4577-b717-d5d71b578d14" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082201" src="https://github.com/user-attachments/assets/c2e6883c-348e-4fde-b610-201b912167d2" />
<img width="1920" height="1080" alt="Screenshot 2026-04-25 082123" src="https://github.com/user-attachments/assets/88ca474c-24ff-4436-82fc-b74c828086fb" />


## Controls

### Keyboard

- `Arrow Keys` — Move
- `Space` — Jump
- `Z` — Melee attack
- `Ctrl` — Ranged attack
- `Q` — Toggle weapon / draw sword
- `Shift` — Run / toggle running
- `Esc` — Pause

### Mobile / Touch

The project also includes mobile-friendly controls and UI actions such as left hand, right hand, shoot, run, pause, and draw-return actions.

## Getting Started

### Requirements

- Unity `2021.3.24f1`

### Run the Project

1. Clone the repository.
2. Open the project in Unity Hub.
3. Let Unity import all assets and packages.
4. Open one of the scenes from `Assets/Scenes`.
5. Press Play in the Unity Editor.

## Scenes

The repository includes these scenes:

- `MainMenu`
- `SampleScene`
- `Level2`
- `Boss`

## Project Structure

- `Assets/Scripts` — Gameplay, combat, AI, UI, and utility scripts
- `Assets/Scenes` — Unity scenes
- `Assets/Prefabs` — Reusable prefabs
- `Assets/Sprites` — Sprite sheets and art assets
- `Assets/Audio` — Sound and music assets
- `ProjectSettings` — Unity project configuration

## Main Gameplay Systems

### Player Controller
Handles movement, jumping, running, armed/unarmed state, sword attacks, shield actions, arrow charging/shooting, pause input, and ledge hanging.

### Combat System
Includes basic melee attacks, attack chaining, air attacks, shield attacks, blocking, knockback, and projectile interaction.

### Damage System
Tracks damage, temporary immunity after hits, death / fall recovery behavior, and player knockback response.

### Enemy / Hazard Scripts
The script folder includes behavior for several enemy and obstacle types, such as:

- Boar
- Bee
- Octorok
- Player stalker
- Rock projectile

### World Interaction
The project includes scripts for:

- Ledge detection
- Block detection
- Ground detection
- Boundary / out-of-bounds handling
- Level exits
- Mob spawning

## Credits

### Engine / Tools
- Unity Engine

### Input / UI Assets
- Joystick Pack
- Kenny On Screen Controls

# Link:
https://drive.google.com/file/d/1uIM4q5lhr0KW5MlVmN-5BW5dIJE_10Mw/view?usp=sharing
