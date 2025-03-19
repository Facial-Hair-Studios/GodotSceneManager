# Godot Scene Manager
<p> Godot Scene Manager is an attempt to help Godot recognize and manage what is a level vs a player or non level / scene GameObject since all GameObject's are viewed as scene's in Godot there's been no real distinction between them until now. This allows a distinct seperation between levels and game objects as well as categorization of levels. </p>

### Features:
- Extensible API, inherit from Level common and define your own Level type
- Scene Manager can and will handle player instance in scene, 2D, 3D etc.
- Can handle many types of scenes and sub scenes or sub levels.
- Can add or remove Levels in the editor.

### ToDo:
- Want to add a better UI to select type of new level to add based on currently known level types
- Add more complete functionality ala Unity or other LevelManager based Game Engines