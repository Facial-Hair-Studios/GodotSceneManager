using Godot;
using System;
using Levels;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
    public static Action ResetLevel;
    public static Action StartNewGame;
    public static Action<Guid, Player> ChangeScene;

    private Node2D currentScene;
    private string _levelManagerPath = "res://Assets/Data/LevelManagerData.json";

    public LevelManagerData ManagerData;
    public int LevelCount => ManagerData.Levels.Count;
    public Guid NewGameScene => ManagerData.NewGameScene;
    public PackedScene PackedPlayer => ManagerData.PackedPlayer;

    private List<Guid> _guids = new List<Guid>();

    // Because Plugin exists outside the SceneTree, we create our own _Tree or refrence to one.
    private static SceneTree s_tree;

    private Player playerRef;

    public Player ActivePlayerRef
    {
        get
        {
            if (playerRef == null)
            {
                playerRef = ManagerData.CreatePlayer();
            }
            return playerRef;
        }
        set
        {
            playerRef = value;
        }
    }

    private static LevelManager s_levelManager;
    public static new SceneTree GetTree() => s_tree;
    public LevelCommon CurrentScene => currentScene.GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");

    public bool Load()
    {
        try
        {
            ManagerData = ResourceLoader.Load<LevelManagerData>(_levelManagerPath);
        }
        catch (InvalidCastException)
        {
            GD.PrintErr("LevelManager Empty!");
            return false;
        }

        return true;
    }

    public static LevelManager Manager
    {
        get
        {
            if (s_levelManager == null)
            {
                s_levelManager = new LevelManager();
            }
            return s_levelManager;
        }
    }

    // Call this from your TitleScreen or other beginning scripts in game.
    // Because LevelManager exists as a plugin it does not exist in SceneTree
    // As such _Ready will not be called.
    public void Init(SceneTree T)
    {
        s_tree = new SceneTree();
        StartNewGame += NewGame;
        ChangeScene += SwitchLevel;
        ResetLevel += Reset;
        s_tree = T;
        CreateManagerData();
    }

    void Reset() => CurrentScene.ResetLevel();
    public void DestroyPlayer(Player p) => p.Dispose();

    public async void CreateManagerData()
    {
        if (!ResourceLoader.Exists(_levelManagerPath))
        {
            GD.Print("No LevelManager Data!");
            throw new Exception("Unable to load LevelManager!");
        }
        else
        {
            await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
            if (!Load())
            {
                throw new Exception("Unable to load LevelManager!");
            }
            playerRef = ManagerData.CreatePlayer();
        }
    }

    // Play level changing animation.
    // Load new scene and set it as current
    public void SwitchLevel(Guid levelID, Player player)
    {
        // This is what's passing instance of player across scenes!!!
        ActivePlayerRef = player;

        if (ManagerData.Levels.Any(x => x.Key == levelID.ToString()))
        {
            Switch(ManagerData.GetLevel(levelID.ToString()), ref currentScene);
        }
        else
        {
            GD.PrintErr("INVALID LEVEL SELECTED: " + levelID);
        }
    }

    public LevelCommon GetLevel(Guid levelID)
    {
        if (ManagerData.Levels.Any(x => x.Key == levelID.ToString()))
        {
            return ManagerData.GetLevel(levelID.ToString()).GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");
        }
        return null;
    }

    void Switch(Node2D toLoad, ref Node2D current)
    {
        //using var title = s_tree.Root.GetNodeOrNull<TitleScreen>("TitleScreen");

        if (current != null)
        {
            s_tree.Root.RemoveChild(current);
            current.QueueFree();
        }
        /*else if (title != null)
        {
            s_tree.Root.RemoveChild(title);
        }*/
        current = toLoad;
        s_tree.Root.AddChild(current);
        s_tree.CurrentScene = current;
        var lev = current.GetNode<LevelCommon>("SubViewportContainer/SubViewport/Level");
        lev.EnterLevel();
    }

    void NewGame()
    {
        SwitchLevel(ManagerData.NewGameScene, null);
        StartNewGame -= NewGame;
    }
}
