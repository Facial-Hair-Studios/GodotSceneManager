#if TOOLS
using Godot;
using Levels;
using System;
using System.Collections.Generic;

public partial class LevelSpaceProperty : EditorProperty
{
    // The control for editing the propert
    private OptionButton _scenes;
    private string _currentlySelectedGuid;
    private int _currentlySelectedIndex = -1;
    private LevelManagerData _managerData;
    private string _levelManagerPath = "res://Assets/Data/LevelManagerData.json";
    private List<Guid> _guids;

    private bool _updating;

    public LevelSpaceProperty()
    {
        _scenes = new OptionButton();
        _guids = new List<Guid>();
        _managerData = new LevelManagerData();

        _scenes.Select(-1);

        if (Load())
        {
            EditorInterface.Singleton.GetResourceFilesystem().Scan();

            if (_managerData.Levels != null)
            {
                foreach (var entry in _managerData.Levels)
                {
                    var level = ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(entry.Value)).Instantiate<Node2D>().GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");
                    if (level is not LevelSelect) // We only care about non Level Selects!
                    {
                        _guids.Add(Guid.Parse(entry.Key));
                        _scenes.AddItem(level.LevelName);
                    }
                }

                AddChild(_scenes);
                AddFocusable(_scenes);
                _scenes.ItemSelected += OnItemSelected;
            }
        }
        else
        {
            GD.PrintErr("Load Failed");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _currentlySelectedGuid = (String)GetEditedObject().Get("LevelGuid");

        GD.Print("Guid: ", _currentlySelectedGuid);

        if (!string.IsNullOrEmpty(_currentlySelectedGuid))
        {
            _currentlySelectedIndex = _guids.IndexOf(Guid.Parse(_currentlySelectedGuid));
            _scenes.Select(_currentlySelectedIndex);
        }
    }

    public bool Load()
    {
        try
        {
            _managerData = ResourceLoader.Load<LevelManagerData>(_levelManagerPath);
        }
        catch (InvalidCastException)
        {
            GD.PrintErr("LevelManager Empty!");
            return false;
        }

        return true;
    }

    public override void _UpdateProperty()
    {
        var level = (long)GetEditedObject().Get(GetEditedProperty());
        _scenes.Select(_currentlySelectedIndex);

        if (level != 0)
        {
            if (level == _currentlySelectedIndex)
            {
                return;
            }
        }
        _updating = true;
        _currentlySelectedIndex = (int)level;
        _updating = false;
    }

    void OnItemSelected(long id)
    {
        if (_updating)
        {
            return;
        }
        _currentlySelectedIndex = (int)id;
        _currentlySelectedGuid = _guids[_currentlySelectedIndex].ToString();
        EmitChanged(GetEditedProperty(), _currentlySelectedGuid);
    }
}
#endif
