#if TOOLS
using Godot;
using Levels;

public partial class LevelInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        return @object is Level;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type,
        string name, PropertyHint hintType, string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        if (type == Variant.Type.String && name == "ParentLevel")
        {
            AddPropertyEditor(name, new LevelProperty());
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
