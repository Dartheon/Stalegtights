using Godot;

public partial class Teleporter : Area2D
{
    private CharacterBody2D playerCB2D;
    private StateMachine stateMachineScript;

    private Marker2D rightMarker;
    private Marker2D leftMarker;

    [Export] private Teleporter linkedPortalLocation;

    private string teleporterType;
    private Vector2 teleportLocation = new();
    private float playerFacingDirection;

    public override void _Ready()
    {
        playerCB2D = GetNode<CharacterBody2D>("root/Main/World/Player");
        stateMachineScript = GetNode<StateMachine>("root/Main/World/Player/STATEMACHINE");

        rightMarker = GetNode<Marker2D>("MarkerRight");
        leftMarker = GetNode<Marker2D>("MarkerLeft");

        switch (teleporterType)
        {
            case "TeleporterRight":
                teleportLocation = rightMarker.GlobalPosition;
                playerFacingDirection = 1.0f;
                break;
            case "TeleporterLeft":
                teleportLocation = leftMarker.GlobalPosition;
                playerFacingDirection = -1.0f;
                break;
            default:
                GD.PushWarning("Invald Teleporter Type, Default Scene Used. Choose Teleporter Left or Right Instead");
                break;
        }
    }

    public void PlayerTeleport()
    {
        stateMachineScript.LastFacingDirection = playerFacingDirection;
        playerCB2D.GlobalPosition = teleportLocation;
    }

    public void OnPlayerBodyEntered(Node2D body)
    {
        if (linkedPortalLocation.teleportLocation != new Vector2())
        {
            GD.PushWarning($"Linked Portal Is Not Set Up In Export Window: {Name}");
            return;
        }

        if (body != null && body.IsInGroup("Player"))
        {
            stateMachineScript.LastFacingDirection = playerFacingDirection;
            body.GlobalPosition = linkedPortalLocation.teleportLocation;
        }
    }
}