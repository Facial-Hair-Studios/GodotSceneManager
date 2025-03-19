using Godot;
using Levels;
using System;
using Godot.Collections;
using System.Linq;
using System.Text.Json.Serialization;

[Tool]
public partial class LevelManagerData : Resource
{
    public string CurrentLevel { get; set; }
    public Guid NewGameScene { get; set; } // What scene starts when newgame on title screen
    public Dictionary<string, long> Levels { get; set; } // Contain's levels, levels are defined as LevelCommon
    public PackedScene PackedPlayer { get; set; }
    public LevelManagerData() => Levels = new Dictionary<string, long>();
    public LevelManagerData(Dictionary<string, long> lev) => Levels = lev;

    // Returns the instantiated packed scene as a Level.
    public Node2D GetLevel(string levelID) => ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(Levels.First(x => x.Key == levelID).Value)).Instantiate<Node2D>();

    // Returns the active Player refrence
    public Player CreatePlayer() => PackedPlayer.Instantiate<Player>(); // This creates a player and is used to spawn it in scene. This should only run if Player null

#if TOOLS
    public void SetPlayerRef(string path) => PackedPlayer = ResourceLoader.Load<PackedScene>(path);

    public bool Add(Guid levelID, long uid)
    {
        LevelFileEntry temp = new LevelFileEntry { Guid = levelID, Uid = uid };
        temp.File = ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(uid));
        Levels.Add(levelID.ToString(), uid);
        return true;
    }

    public bool Remove(Guid levelID)
    {
        if (Levels.Any(x => x.Key == levelID.ToString()))
        {
            Levels.Remove(Levels.First(x => x.Key == levelID.ToString()).Key);
            return true;
        }
        GD.PrintErr("Level does not exist in manager");
        return false;
    }

    // Set the Level that starts when New game pressed on Title screen
    public void SetNewGameLevel(Guid levelID)
    {
        LevelCommon l = ResourceLoader.Load<PackedScene>(ResourceUid.GetIdPath(Levels.First(x => x.Key == levelID.ToString()).Value)).Instantiate<LevelCommon>();
        NewGameScene = levelID;
    }
#endif

};
