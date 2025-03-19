using Godot;
using System;

public partial class HubActor : Node2D
{
    // Playable sprite bobs up and down.
    AnimationPlayer _animation;

    public override void _Ready()
    {
        base._Ready();
        // _animation = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    // Play animations to the call of the LevelSelect / active path making character walk.
    public void Move()
    {

    }
}
