#if TOOLS
using Godot;
using Levels;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

[Tool]
public partial class LevelDockScript : Panel
{
    private Button _save;
    private Button _refresh;
    private Button _newLevelButton;
    private Button _newLevelSelectButton;
    private List<Guid> _guids = new();
    private LevelManagerData _managerData;
    private string _levelManagerPath = "res://Assets/Data/LevelManagerData.tscn";

    // ItemList Properties
    private ItemList _levelList;
    private Button _addLevelButton;
    private Button _removeLevelButton;
    private Vector2 _currentListPostion;
    private int _currentIndex;

    // File selector for the Level at the start of a new game
    private OptionButton _newGameLevelSelector;
    private FileDialog _dialog;
    // To set the actual LevelManager refrence to PlayerData
    private FileSelector _playerRef;

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        base._EnterTree();
        _guids = new List<Guid>();

        _dialog = GetNode<FileDialog>("FileDialog");
        _managerData = new LevelManagerData();
        _save = GetNode<Button>("ManagerLabel/ControlButtonsContainer/Save");
        _refresh = GetNode<Button>("ManagerLabel/ControlButtonsContainer/Refresh");

        _levelList = GetNode<ItemList>("VBoxContainer/LevelList");
        _addLevelButton = GetNode<Button>("VBoxContainer/HBoxContainer/AddLevel");
        _newLevelButton = GetNode<Button>("VBoxContainer/HBoxContainer/NewLevel");
        _newLevelSelectButton = GetNode<Button>("VBoxContainer/HBoxContainer/NewLevelSelect");
        _removeLevelButton = GetNode<Button>("VBoxContainer/HBoxContainer/RemoveLevel");

        _dialog.InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen;

        _levelList.ItemClicked += ItemClicked;
        _addLevelButton.Pressed += AddScene_Button;
        _removeLevelButton.Pressed += Remove;

        _save.Pressed += Save;
        _refresh.Pressed += RefreshUI;
        _newLevelButton.Pressed += NewLevel_Button;
        _newLevelSelectButton.Pressed += NewLevelSelect_Button;

        _playerRef = GetNode<FileSelector>("FilePickerContainer/FilePickerBackground/VBoxContainer/PlayerRef");
        _playerRef.Init(string.Empty, new string[] { "*.tscn" });
        if (_managerData.PackedPlayer != null)
        {
            _playerRef.SetPathField(_managerData.PackedPlayer.ResourcePath);
        }

        _newGameLevelSelector = GetNode<OptionButton>("FilePickerContainer/FilePickerBackground/VBoxContainer/NewGameLevel");
        _newGameLevelSelector.ItemSelected += SetNewGameLevelInLevelManager;

        if (!Load())
        {
            GD.Print("Level Manager Empty");
        }
        RefreshUI();
    }

    void Remove()
    {
        DirAccess.RemoveAbsolute(ResourceUid.GetIdPath(_managerData.Levels.First(x => x.Key == _guids[_currentIndex].ToString()).Value));
        _managerData.Remove(_guids[_currentIndex]);
        _levelList.RemoveItem(_currentIndex);
        _currentIndex -= 1;
        EditorInterface.Singleton.GetResourceFilesystem().Scan();

        Save();
        RefreshUI();
    }

    void AddScene(string path)
    {
        var uid = ResourceLoader.GetResourceUid(path);
        if (!ResourceUid.HasId(uid))
        {
            var newUid = ResourceUid.CreateId();
            ResourceUid.AddId(newUid, path);
            uid = newUid;
        }

        var packed = (PackedScene)ResourceLoader.Load(path);
        var level = packed.Instantiate<LevelCommon>();

        if (level.ID == Guid.Empty)
        {
            level.ID = Guid.NewGuid();
            packed.Pack(level);
            ResourceSaver.Save(packed, path);
            ResourceUid.SetId(uid, path);
        }
        if (_managerData.Add(level.ID, uid))
        {
            _guids.Add(level.ID);
            _levelList.AddItem(level.LevelName);
        }
        _dialog.FileSelected -= AddScene;
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
        }
        _newGameLevelSelector.Clear();

        foreach (var entry in _managerData.Levels)
        {
            if (ResourceUid.HasId(entry.Value))
            {
                var packed = ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(entry.Value));
                var temp = packed.Instantiate<Node2D>();
                var tempLev = temp.GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");

                if (temp == null)
                {
                    GD.PrintErr("Failed to due to bad cast! Check if object is [Tool]");
                    return false;
                }

                _newGameLevelSelector.AddItem(tempLev.LevelName);
                _guids.Add(Guid.Parse(entry.Key));
            }
            else
            {
                GD.PrintErr("Uid Path not registered: ", entry.Value);
            }
        }

        _newGameLevelSelector.Select(_guids.FindIndex(x => x == _managerData.NewGameScene));
        if (_managerData.PackedPlayer != null)
        {
            _playerRef.SetPathField(_managerData.PackedPlayer.ResourcePath);
        }
        return true;
    }

    public void Save()
    {
        var err = ResourceSaver.Save(_managerData, _levelManagerPath);
        if (err != Error.Ok)
        {
            GD.PrintErr("LevelManager Failed to save: ", err);
        }
    }

    public void RefreshUI()
    {
        _levelList.Clear();
        _newGameLevelSelector.Clear();

        GD.Print("Levels Count: " + _managerData.Levels.Count);
        GD.Print("Guids Count: " + _guids.Count);
        foreach (var Level in _managerData.Levels)
        {
            if (ResourceUid.HasId(Level.Value))
            {
                var packed = ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(Level.Value));
                var temp = packed.Instantiate<Node2D>();
                var tempLev = temp.GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");

                _levelList.AddItem(tempLev.LevelName);
                _newGameLevelSelector.AddItem(tempLev.LevelName);
            }
        }
    }
    void PlayerFileSelected() => _managerData.SetPlayerRef(_playerRef.Path);
    public void SetPlayerRef(string path) => _managerData.SetPlayerRef(path);
    public void SetNewGameLevelInLevelManager(long id) => _managerData.SetNewGameLevel(_guids[(int)id]);
    public void ChangeSceneInEditor(Guid levelID)
    {
        if (_managerData.Levels != null)
        {
            if (ResourceUid.HasId(_managerData.Levels.First(x => x.Key == levelID.ToString()).Value))
            {
                EditorInterface.Singleton.OpenSceneFromPath(ResourceUid.GetIdPath(_managerData.Levels.First(x => x.Key == levelID.ToString()).Value));
            }
        }
    }

    void NewLevel_Button()
    {
        _dialog.FileSelected += CreateExampleLevel;
        _dialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        _dialog.Popup();
    }

    void NewLevelSelect_Button()
    {
        _dialog.FileSelected += CreateExampleLevelSelect;
        _dialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        _dialog.Popup();
    }

    void AddScene_Button()
    {
        _dialog.FileSelected += AddScene;
        _dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        _dialog.Popup();
    }

    void NewGameSceneSelected(long index)
    {
        var levelGuid = _guids[(int)index];
        if (_managerData.NewGameScene == levelGuid)
        {
            return;
        }
        _managerData.NewGameScene = levelGuid;
    }

    void ItemClicked(long i, Vector2 positon, long mousePos)
    {
        _currentIndex = (int)i;
        _currentListPostion = positon;
        ChangeSceneInEditor(_guids[_currentIndex]);
    }

    // Note the below functions are describing and creating Godot packed scene's in code
    // Generate Level Type node with script attached. Attach nodes, set properties like Name
    // Generate .tscn file, generate refrence to tscn to be added to Level Manager
    public void CreateExampleLevel(string path)
    {
        var level = new LevelCommon();
        var name = Path.GetFileNameWithoutExtension(path);

        level.LevelName = name;
        level.Name = level.LevelName;
        level.ID = Guid.NewGuid();

        // Add TileMap
        var tileMap = new TileMapLayer();
        tileMap.Name = "Layer";
        level.AddChild(tileMap);
        tileMap.Owner = level;

        var goal = new GoalPost();
        var shape = new CollisionShape2D();
        var rect = new RectangleShape2D();
        goal.Name = "GoalPost";
        rect.Size = new Vector2(100, 100);
        shape.Shape = rect;
        goal.AddChild(shape);
        level.AddChild(goal);
        goal.Owner = level;

        var startPoint = new Node2D();
        startPoint.Name = "PlayerStartPoint";
        level.AddChild(startPoint);
        startPoint.Owner = level;

        var packedLevel = new PackedScene();
        packedLevel.Pack(level);
        packedLevel.ResourceName = level.LevelName;
        var uid = ResourceUid.CreateId();

        ResourceSaver.Save(packedLevel, path);

        ResourceUid.AddId(uid, path);

        EditorInterface.Singleton.GetResourceFilesystem().Scan();

        AddScene(path);

        _dialog.FileSelected -= CreateExampleLevel;

        RefreshUI();
        Save();
    }

    public void CreateExampleLevelSelect(string path)
    {
        var level = new LevelCommon();
        var name = Path.GetFileNameWithoutExtension(path);
        level.LevelName = name;

        level.ID = Guid.NewGuid();
        level.SetLevelData(new LevelData());

        var packedLevel = new PackedScene();
        packedLevel.Pack(level);
        ResourceSaver.Save(packedLevel, path);

        EditorInterface.Singleton.GetResourceFilesystem().UpdateFile(path);

        var uid = ResourceUid.CreateId();

        if (!_managerData.Add(level.ID, uid))
        {
            GD.PrintErr("Failed to add Level");
        }

        _dialog.FileSelected -= CreateExampleLevelSelect;

        ResourceUid.AddId(uid, path);

        Save();
        RefreshUI();
        ChangeSceneInEditor(level.ID);
    }

    void RemoveDeferred() => _levelList.Clear();

    public override void _ExitTree()
    {
        base._ExitTree();
        _levelList.Clear();
        _save.Pressed -= Save;
        _refresh.Pressed -= RefreshUI;
        _newLevelButton.Pressed -= NewLevel_Button;
        _newGameLevelSelector.ItemSelected -= SetNewGameLevelInLevelManager;
    }
}
#endif
