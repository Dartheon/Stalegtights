using Godot;

public partial class TimerClock : Node
{
    public double TimePassed { get; private set; } //the correct way to allow access to other scripts while keeping private
    private bool isRunning = false;

    public override void _Process(double delta)
    {
        if (isRunning)
        {
            TimePassed += delta;
        }
    }

    public void StartCounter()
    {
        ResetCounter();
        isRunning = true;
    }

    public void StopCounter()
    {
        isRunning = false;
    }

    private void ResetCounter()
    {
        TimePassed = 0f;
    }
}