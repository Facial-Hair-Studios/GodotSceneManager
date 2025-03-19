#if TOOLS
using Godot;

public partial class LevelSpaceInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        return @object is LevelSpace;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        if (type == Variant.Type.String && name == "LevelGuid")
        {
            AddPropertyEditor(name, new LevelSpaceProperty());
            return true;
        }
        return false;
    }

    public override void _ParseEnd(GodotObject @object)
    {
        base._ParseEnd(@object);
    }

}
#endif
