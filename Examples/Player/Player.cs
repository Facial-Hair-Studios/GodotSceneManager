using Godot;
using System;

public enum LadderStates { NONE, BEGIN, CLIMBING, EXITING, CROUCH, EXIT };

public partial class Player : CharacterBody2D
{
    private Area2D _ladderArea;
    private Timer _coyoteTimer;
    private Timer _bufferedJumpTimer;

    public Sprite2D PlayerSprite;
    public AnimationPlayer AnimationPlayer;
    public bool IsLadderDetected => _ladderArea.GetOverlappingBodies().Count >= 1;

    // Jump Properties
    [Export] private float _jumpVelocity;
    [Export] private float _jumpGravity;
    [Export] private float _fallGravity;

    private float _jumpHeight;
    private float _jumpTimeToPeak;
    private float _jumpTimeToDescent;

    private int _numJumps;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        PlayerSprite = (Sprite2D)GetNode("CenterContainer/PlayerSprite");
        AnimationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _bufferedJumpTimer = (Timer)GetNode("BufferedJump");

        _ladderArea = (Area2D)GetNode("LadderArea");
        _coyoteTimer = (Timer)GetNode("CoyoteTimer");

        LevelManager.StartNewGame += NewGame;
        Owner = GetParent();
        _jumpVelocity = ((2.0f * _jumpHeight) / _jumpTimeToPeak) * -1.0f;
        _jumpGravity = ((-2.0f * _jumpHeight) / (_jumpTimeToPeak * _jumpTimeToPeak)) * -1.0f;
        _fallGravity = ((-2.0f * _jumpHeight) / (_jumpTimeToDescent * _jumpTimeToDescent)) * -1.0f;
    }

    new protected float GetGravity()
    {
        if (Velocity.Y < 0.0)
        {
            return _fallGravity;
        }
        return _jumpGravity;
    }

    public void ApplyGravity(float delta)
    {
        var _Velocity = Velocity;
        _Velocity.Y += delta * GetGravity();
        Velocity = _Velocity;

    }

    public void NewGame()
    {
        GD.Print("Initalize Any Player data in here!");
        LevelManager.StartNewGame -= NewGame;
    }

    public void ResetPlayerPosition(Checkpoint CurrentCheckpoint)
    {
        Node2D PlayerStartPoint = LevelManager.Manager.CurrentScene.GetNode<Node2D>("PlayerStartPoint");

        SetPhysicsProcess(false);

        if (CurrentCheckpoint != null)
        {
            Position = CurrentCheckpoint.GlobalPosition;
        }
        else
        {
            // Need to grab a "Player Starting place"
            Position = PlayerStartPoint.GlobalPosition;
        }
    }

    public bool CanJumpAgain => IsOnFloor() || _numJumps > 0;
    public bool CanJump => IsOnFloor() || _coyoteTimer.GetTimeLeft() > 0;

    public void Jump()
    {

    }




    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        ApplyGravity((float)delta);
        MoveAndSlide();
    }
}
