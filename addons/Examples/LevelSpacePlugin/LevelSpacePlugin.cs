#if TOOLS
using Godot;
using System;

[Tool]
public partial class LevelSpacePlugin : EditorPlugin
{
    LevelSpaceInspector _levelSpaceInspector;
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        _levelSpaceInspector = new LevelSpaceInspector();
        AddInspectorPlugin(_levelSpaceInspector);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        RemoveInspectorPlugin(_levelSpaceInspector);
    }
}
#endif
