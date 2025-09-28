using Godot;

public partial class Teleporter : Area2D
{
    //TO DO LIST:
    /*
    - after teleporting, disable teleporter collider until Player exits the area2D then signal (Timer or teleporter) to reenable the collider
    - In future will need to incorperate loading areas/having areas loaded before teleporting player through and reevaluating loaded areas after teleporting
    */
    #region Variables
    #region Class Scripts
    //For Assigning the Different Scripts Needed
    private CharacterBody2D playerCB2D;
    private StateMachine stateMachineScript;
    private GameManager gameManager;
    #endregion

    #region Markers
    //For Assigning the Markers to the Teleporter
    private Marker2D rightMarker;
    private Marker2D leftMarker;
    #endregion

    #region Export Linked Portal
    //Used to Assign a Linked Portal to this Teleporter
    [Export] private Teleporter linkedPortalLocation;
    #endregion

    #region General
    //To assign the MetaData attached to this teleporter for determining if the Portal puts the Player on the Left or Right
    private string teleporterType;
    //To set the Global Position for the Player to Teleport To
    private Vector2 teleportLocation = new();
    //To Update the Facing Position of the Player after Teleporting through the Portal and having the Player Face away from the Portal
    private float playerFacingDirection;
    private bool teleporterVariant;
    #endregion
    #endregion

    #region Methods
    public override void _Ready()
    {
        //Class Script Assigning
        playerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        gameManager = GetNode<GameManager>("/root/GameManager");

        //Marker Variable Assigning
        rightMarker = GetNode<Marker2D>("RightMarker");
        leftMarker = GetNode<Marker2D>("LeftMarker");

        //Assigning the MetaData for Determining Left or Right Teleporter
        teleporterType = (string)GetMeta("TeleporterType");
        teleporterVariant = (bool)GetMeta("TeleporterVariant");

        //Adding the Teleporter to the TeleporterDictionary to use in the DebugUI
        gameManager.TeleporterDictionary.Add(Name, this);

        //This switch sets the parameters of the specific Teleporter Depending on what MetaData is Set
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
                GD.PushWarning("Invald Teleporter Type, Default Scene Used. Choose Teleporter Left, Right, or Variant Instead");
                break;
        }
    }

    //Called from the DebugUI when a button is clicked to teleport the Player to the specific Teleporter
    public void TeleportPlayer()
    {
        stateMachineScript.LastFacingDirection = playerFacingDirection;
        playerCB2D.GlobalPosition = teleportLocation;
        GD.Print($"Teleported to {Name} using DebugUI");
    }

    //Signal when the Player Enters the Teleporter
    public void OnPlayerBodyEntered(Node2D body)
    {
        if (linkedPortalLocation.teleportLocation == new Vector2())
        {
            GD.PushWarning($"Linked Portal Is Not Set Up In Export Window: {Name}");
            return;
        }

        if (body != null && body.IsInGroup("Player") && !teleporterVariant)
        {
            stateMachineScript.LastFacingDirection = playerFacingDirection;
            body.GlobalPosition = linkedPortalLocation.teleportLocation;
            GD.Print($"Teleported to {linkedPortalLocation.Name} from {Name}");
        }
    }

    public void PlayerInteract()
    {
        if (teleporterVariant)
        {
            stateMachineScript.LastFacingDirection = playerFacingDirection;
            playerCB2D.GlobalPosition = linkedPortalLocation.teleportLocation;
            GD.Print($"Teleported to {linkedPortalLocation.Name} from {Name} using an Interactive Teleporter");
        }
    }
    #endregion
}