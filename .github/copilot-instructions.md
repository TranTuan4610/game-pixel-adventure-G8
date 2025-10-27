# Unity Game Project: Pixel Adventure

This is a Unity game project using pixel art and 2D platformer mechanics. Below are key details to help AI coding assistants understand and work with this codebase.

## Project Overview

- Unity Version: 6000.2.6f1
- Project Type: 2D Platformer Game
- Art Style: Pixel Art
- Core Systems: Player Movement, Game Management, Level Control

## Project Structure

### Key Directories
- `Assets/Scripts/`: Core game scripts
- `Assets/Animations/`: Animation assets for player, enemies etc.
- `Assets/Sprites/`: Pixel art sprites and textures
- `Assets/Scenes/`: Game scenes (Menu, Game levels)
- `Assets/Prefabs/`: Reusable game objects
- `Assets/Audio/`: Sound effects and music

### Core Components

1. Player System
- Located in: `Assets/Scripts/PlayerController.cs`
- Handles: Movement, animations, collisions
- Key Features: 2D platformer controls, animation state management

2. Game Management
- Located in: `Assets/Scripts/GameManager.cs`
- Handles: Score tracking, game state
- Features: Score system, game progression

3. Level System 
- Located in: `Assets/Scripts/LevelManager.cs`
- Handles: Level progression and management
- Features: Level loading, checkpoints

## Development Guidelines

1. Code Style
- Use PascalCase for class names
- Use camelCase for variables/fields
- Follow Unity's component-based architecture

2. Asset Organization
- Place new scripts in appropriate subdirectories under `Assets/Scripts/`
- Keep animation assets organized by entity type
- Use descriptive names for game objects and components

3. Scene Structure
- Main scenes: Menu.unity, Game.unity
- Organize hierarchy logically with empty objects as containers
- Use prefabs for reusable elements

## Common Development Tasks

1. Adding New Features
- Consider impact on existing systems
- Update GameManager if adding new gameplay mechanics
- Test in both play mode and build

2. Animation Integration
- Use Unity's Animator system
- Keep animation controllers organized by entity
- Test transitions between states

3. UI Updates
- Follow existing UI style
- Use TextMeshPro for text elements
- Ensure UI scales correctly on different resolutions

## Best Practices

1. Performance
- Optimize sprite sheets and textures
- Use object pooling for frequently spawned objects
- Keep Update() methods efficient

2. Physics
- Use 2D physics components
- Configure collision layers appropriately
- Avoid physics calculations in FixedUpdate when possible

3. Game State
- Use GameManager for global state
- Implement proper scene transitions
- Handle pause/resume functionality

## Debugging Tips

1. Common Issues
- Check collision matrix for unexpected behavior
- Verify animation transitions in Animator
- Confirm script execution order if timing-dependent

2. Testing
- Test on multiple resolutions
- Verify performance with Profiler
- Check for memory leaks in extended play sessions

## Version Control

1. Git Guidelines
- Keep large binary files in Git LFS
- Update .gitignore for Unity-specific files
- Use meaningful commit messages

## Build Settings

1. Configuration
- Target platform: Windows/WebGL
- Graphics settings optimized for 2D
- Input system configured for keyboard/gamepad