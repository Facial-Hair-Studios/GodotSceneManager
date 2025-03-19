#if TOOLS
using Godot;
using Levels;

[Tool]
public partial class LevelPlugin : EditorPlugin
{
    LevelInspector _levelInspector;
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        _levelInspector = new LevelInspector();
        AddInspectorPlugin(_levelInspector);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        RemoveInspectorPlugin(_levelInspector);
    }
}
#endif
