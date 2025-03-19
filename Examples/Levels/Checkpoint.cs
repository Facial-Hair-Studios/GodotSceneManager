using Godot;
using Levels;

public partial class Checkpoint : Area2D
{
    [Export]
    public bool isActive;

    [Export]
    public bool wasActive;

    Level level;
    public override void _Ready()
    {
        BodyEntered += Activate;
        level = (Level)Owner;
    }
    public void Activate(object body)
    {
        if (body is Player)
        {
            GD.Print("Checkpoint touched");
            if (!wasActive && !isActive)
            {
                level.Checkpoint(this);
            }
        }
    }

    public void Deactivate()
    {
        isActive = false;
        wasActive = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
