using Godot;

public partial class Ladders : Node2D
{
    #region Variables
    private CollisionShape2D ladderTop;
    private float ladderBodyAreaPosX;
    private float waitTimer = 0.2f; //changes time for waiting before enabling the Collider
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        ladderTop = GetNodeOrNull<CollisionShape2D>("%LadderTopCollisionShape");

        ladderBodyAreaPosX = GlobalPosition.X;
    }
    #endregion

    public async void OnWayDisableLadderTop()
    {
        if (ladderTop != null)
        {
            ladderTop.Disabled = true;
            await ToSignal(GetTree().CreateTimer(waitTimer), SceneTreeTimer.SignalName.Timeout);
            ladderTop.Disabled = false;
        }
    }

    public float GetLadderGlobalPositionX()
    {
        return ladderBodyAreaPosX;
    }
    #endregion
}