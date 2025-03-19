using Godot;

namespace Levels
{
    public enum ExampleLevelState { LOCKED, UNLOCKED, NON_COMPLETE, COMPLETE }

    // Note that Level Data has several uses, from saving data in platformer games
    // To storing potential data like Event triggers, Monster Spawns or other needed info in RPG games
    public partial class LevelData : Resource
    {
        public ExampleLevelState LevelState { get; set; }

        public LevelData()
        {
            LevelState = ExampleLevelState.NON_COMPLETE;
        }

    }
}