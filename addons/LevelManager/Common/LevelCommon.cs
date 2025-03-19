using Godot;
using System;

namespace Levels
{
    public enum LevelType { DEFAULT, GRASS, ISLAND, ICE, WATER }

    [Tool]
    public partial class LevelCommon : Node2D
    {
        protected Player Player;
        public LevelData Data { get; protected set; }

        public Guid ID;

        // Used for Pause game
        private bool _isActive;
        [Export] private Resource _audioFile;
        [Export] private LevelType _type;
        private AudioStreamPlayer _backgroundPlayer;
        public bool IsLevelComplete => Data.LevelState == ExampleLevelState.COMPLETE;
        public LevelType LevelType => _type;
        [Export] public string LevelName;

        public Action UnlockEvent;
        public void SetLevelData(LevelData level) => Data = level;
        protected void SetIsActive(bool state) => _isActive = state;

        public void CreatePlayer()
        {
            if (Player == null || !IsInstanceValid(Player) || Player.IsQueuedForDeletion())
            {
                if (LevelManager.Manager != null)
                {
                    Player = LevelManager.Manager.ActivePlayerRef;
                }
                // Started outside LevelManager!
                else
                {
                    GD.PushWarning("NO LEVEL MANAGER!");
                    Player = ResourceLoader.Load<PackedScene>("res://Assets/resources/Player/Player.tscn").Instantiate<Player>();
                }
            }
        }

        public virtual void EnterLevel()
        {
            _backgroundPlayer = (AudioStreamPlayer)GetNode("BackgroundAudio");
            CreateAudioStream();
            SetIsActive(true);
        }

        public void CreateAudioStream()
        {
            if (_audioFile != null)
            {
                _backgroundPlayer.Stream = GD.Load<AudioStream>(_audioFile.ResourcePath);
                _backgroundPlayer.Play();
            }
        }

        public virtual void Update(double delta)
        {

        }

        public virtual void FixedUpdate(double delta)
        {

        }

        public override void _Process(double delta)
        {
            if (_isActive)
            {
                base._Process(delta);
                if (!Engine.IsEditorHint())
                {
                    Update(delta);
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_isActive)
            {
                base._PhysicsProcess(delta);
                if (!Engine.IsEditorHint())
                {
                    FixedUpdate(delta);
                }
            }
        }

        public virtual void CompleteLevel()
        {
            Data.LevelState = ExampleLevelState.COMPLETE;
        }

        public virtual void ExitLevel()
        {

        }

        public virtual void CalledDefferedExitLevel()
        {

        }

        public virtual void ResetLevel()
        {

        }
    }
}
