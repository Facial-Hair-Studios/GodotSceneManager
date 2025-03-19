using System;
using Godot;
using Levels;

[Tool]
public partial class LevelSpace : Control
{
    // ToDo, level selector UI element
    [Export] public string LevelGuid;
    private LevelSelect _levelSelect;
    private Label _levelNameLabel;

#if TOOLS
    public void SetLevel(Guid l)
    {
        if (l != Guid.Empty)
        {
            LevelGuid = l.ToString();
        }
    }
#endif

    public override void _EnterTree()
    {
        base._EnterTree();

        if (!string.IsNullOrEmpty(LevelGuid))
        {
            _levelNameLabel = GetNode<Label>("LevelName");
            _levelSelect = (LevelSelect)GetParent();

            if (!Engine.IsEditorHint())
            {
                var level = LevelManager.Manager.ManagerData.GetLevel(LevelGuid.ToString()).GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");
                _levelNameLabel.Text = level.LevelName;
            }
            _levelSelect.AddLevelSpace(this);
        }

    }

    public void ActivateLevel() => LevelManager.ChangeScene(Guid.Parse(LevelGuid), LevelManager.Manager.ActivePlayerRef);

}
