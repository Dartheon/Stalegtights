using Godot;

public partial class Ladders : Area2D
{
    #region Variables
    private Player playerScript;
    private CollisionShape2D ladderTop;
    private float ladderBodyAreaPosX;
    private float waitTimer = 0.2f; //changes time for waiting before enabling the Collider
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        playerScript = GetNode<Player>("/root/Main/World/Player");
        ladderTop = GetNodeOrNull<CollisionShape2D>("%LadderTopCollisionShape");

        ladderBodyAreaPosX = GlobalPosition.X;
    }
    #endregion

    public async void OnWayDisableLadderTop(CollisionShape2D shape2D)
    {
        if (shape2D != null)
        {
            shape2D.Disabled = true;
            await ToSignal(GetTree().CreateTimer(waitTimer), SceneTreeTimer.SignalName.Timeout);
            shape2D.Disabled = false;
        }
    }

    public void OnPlayerBodyEntered(Node body)
    {
        if (body != null && body.IsInGroup("Player"))
        {

            Player.PlayerAboveLadder.Add(ladderTop);

            playerScript.SetLadderPosition(GetLadderGlobalPositionX());
        }
    }

    public void OnPlayerBodyExited(Node body)
    {
        if (body != null && body.IsInGroup("Player"))
        {
            Player.PlayerAboveLadder.Remove(ladderTop);
        }
    }

    public float GetLadderGlobalPositionX()
    {
        return ladderBodyAreaPosX;
    }
    #endregion
}