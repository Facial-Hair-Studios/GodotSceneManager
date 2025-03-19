using System.Collections.Generic;
using System;
using System.Linq;
using Godot;

namespace Levels
{

    [Tool]
    public partial class Level : LevelCommon
    {
        public Action Exit;

        // Could also contain items like piece of map
        private GoalPost _levelGoal;
        public LevelCommon ActiveSubScene;
        protected Checkpoint CurrentCheckpoint;
        public LevelData LevelData { get => (LevelData)Data; }
        [Export] public string ParentLevel { get; set; }

        private Node2D _playerStartPoint;

        public void AddPlayer()
        {
            CreatePlayer();
            AddChild(Player);
            Player.ResetPlayerPosition(CurrentCheckpoint);
            Player.SetProcess(true);
            Player.SetPhysicsProcess(true);
        }

        public override void EnterLevel()
        {
            base.EnterLevel();
            if (!LoadOrCreateLevelData())
            {
                throw new Exception("Level Data failed to Create or Load!");
            }

            AddPlayer();
            Exit += ExitLevel;
        }

        public bool LoadOrCreateLevelData()
        {
            if (!ResourceLoader.Exists($"user://Save/LevelData/{LevelName}.tscn"))
            {
                Data = new LevelData();
                return ResourceSaver.Save(Data, $"user://Save/LevelData/{LevelName}.tscn") != Error.Failed;
            }

            else if (ResourceLoader.Exists($"user://Save/LevelData/{LevelName}.tscn"))
            {
                Data = ResourceLoader.Load<LevelData>($"user://Save/LevelData/{LevelName}.tscn");
                return true;
            }
            return false;
        }

        public void ResetPlayerPosition()
        {
            _playerStartPoint = GetNode<Node2D>("_playerStartPoint");

            if (CurrentCheckpoint != null)
            {
                Player.Position = CurrentCheckpoint.GlobalPosition;
            }
            else
            {
                // Need to grab a "Player Starting place"
                Player.Position = _playerStartPoint.GlobalPosition;
            }
        }

        public override void ResetLevel()
        {
            Player.ResetPlayerPosition(CurrentCheckpoint);
            if (Player.IsInsideTree())
            {
                RemoveChild(Player);
            }
            CallDeferred(nameof(EnterLevel));
        }

        public void Checkpoint(Checkpoint NewCheckpoint)
        {
            if (CurrentCheckpoint != null)
            {
                CurrentCheckpoint.Deactivate();
            }
            NewCheckpoint.isActive = true;
            CurrentCheckpoint = NewCheckpoint;
        }

#if !TOOLS
        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }
#endif

        // Ensure the scene closes properly before changing.
        public override void ExitLevel()
        {
            base.ExitLevel();
            RemoveChild(Player);
            Exit -= ExitLevel;
            LevelManager.Manager.SwitchLevel(Guid.Parse(ParentLevel), Player);
        }
    }
}
