using Godot;
using System.Linq;
using Levels;

public partial class GoalPost : Area2D
{
    // To Do add gui selector in here for hub to transition to from LevelManager
    [Export] string hub;

    async void DetectBody()
    {
        await ToSignal(GetTree(), "physics_frame");
        await ToSignal(GetTree(), "physics_frame");
        if (GetOverlappingBodies().Count >= 1)
        {
            if (GetOverlappingBodies()[0] is Player)
            {
                using var temp = (Level)Owner;
                temp.ExitLevel();
            }
        }

    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        DetectBody();
    }
}
