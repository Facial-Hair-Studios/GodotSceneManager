using Godot;
using System.Collections.Generic;

namespace Levels
{

    [Tool]
    public partial class LevelSelect : LevelCommon // types of level
    {
        private HubActor _actor;
        private int _desiredIndex;
        private int _maxLevelIndexes;
        private int _currentLevelIndex;
        private List<LevelSpace> _levelSpaces;
        private AudioStreamPlayer _backgroundPlayer;

        public override void _Ready()
        {
            base._Ready();

        }

        public override void EnterLevel()
        {
            base.EnterLevel();

            _actor = (HubActor)GetNode("LevelSelectActor");

            if (_actor == null)
            {
                _actor = new HubActor();
            }

            if (_levelSpaces != null && _maxLevelIndexes > 0)
            {
                _currentLevelIndex = 0;
                _desiredIndex = 0;

                _actor.GlobalPosition = _levelSpaces[0].GlobalPosition;
            }
            else
            {
                GD.PrintErr("Level Spaces null");
            }

        }

        public override void ResetLevel()
        {
            _currentLevelIndex = 0;
            _actor.GlobalPosition = _levelSpaces[_currentLevelIndex].GlobalPosition;
        }

        public void AddLevelSpace(LevelSpace space)
        {
            if (_levelSpaces == null)
            {
                _levelSpaces = new List<LevelSpace>();
            }

            if (!_levelSpaces.Contains(space))
            {
                _maxLevelIndexes += 1;
                _levelSpaces.Add(space);
            }
        }

        public void RemoveLevelSpace(LevelSpace space)
        {
            if (_levelSpaces.Contains(space))
            {
                _maxLevelIndexes -= 1;
                _levelSpaces.Remove(space);
            }
        }

        public override void Update(double delta)
        {
            base.Update(delta);
            Move();
        }

        void Move()
        {
            if (Input.IsActionJustPressed("Up"))
            {

            }
            if (Input.IsActionJustPressed("Down"))
            {

            }
            if (Input.IsActionJustPressed("Left"))
            {
                if (_desiredIndex-- > 0)
                {
                    _currentLevelIndex -= 1;
                    _actor.GlobalPosition = _levelSpaces[_currentLevelIndex].GlobalPosition;
                }
            }
            if (Input.IsActionJustPressed("Right"))
            {

                if (_desiredIndex++ <= _maxLevelIndexes)
                {
                    _currentLevelIndex += 1;
                    GD.Print(_currentLevelIndex);
                    _actor.GlobalPosition = _levelSpaces[_currentLevelIndex].GlobalPosition;
                }
            }
            if (Input.IsActionJustPressed("Submit"))
            {
                ExitLevel();
            }
        }

        public override void ExitLevel()
        {
            base.ExitLevel();
            // Why is the player getting stuck?
            _levelSpaces[_currentLevelIndex].ActivateLevel();
        }
    }
}
