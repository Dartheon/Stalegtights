using Godot;

[Tool]
public partial class TeleporterAllType : Area2D
{
    //TO DO LIST:
    /*
    - In future will need to incorperate loading areas/having areas loaded before teleporting player through and reevaluating loaded areas after teleporting
        - if active, tell slmanager to load linked portal area
    */
    #region Variables
    #region Class Scripts
    //For Assigning the Different Scripts Needed
    private CharacterBody2D playerCB2D;
    private StateMachine stateMachineScript;
    private GameManager gameManager;
    #endregion

    #region Enum
    public enum TeleporterType
    {
        RightMarkerTeleporter,
        LeftMarkerTeleporter,
        DEFAULT
    };
    #endregion

    #region Markers
    //For Assigning the Markers to the Teleporter
    private Marker2D rightMarker = new();
    private Marker2D leftMarker = new();
    #endregion

    #region Exports
    //Used to Assign a Linked Portal to this Teleporter
    [Export] private TeleporterAllType linkedPortalLocation; //LEAVE UNDEFINED WHEN EXPORTING TO INSPECTOR, MAKE SURE TO NEVER SET linkedPortalLocation = new(); OR WILL PERMANATLY BREAK SCENE

    [Export]
    public TeleporterType TeleporterSelected
    {
        //This is for getting the editor to change teleporter related stuff when it happens
        get => _teleporterSelected;
        set
        {
            _teleporterSelected = value;
            if (Engine.IsEditorHint())
                UpdateTeleporterVisuals();
        }
    }

    [Export] public bool Interactable { get; set; } = false; //for setting if the portal is interactable or not
    #endregion

    #region General
    private TeleporterType _teleporterSelected = TeleporterType.DEFAULT; //sets the default position of the selector

    private Vector2 teleportLocation = new(); //To set the Global Position for the Player to Teleport To

    private float playerFacingDirection; //To Update the Facing Position of the Player after Teleporting through the Portal and having the Player Face away from the Portal

    private string newName; //For setting nodes name when setting up teleporter in the editor
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            //Class Script Assigning
            playerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");
            stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
            gameManager = GetNode<GameManager>("/root/GameManager");

            //Adding the Teleporter to the TeleporterDictionary to use in the DebugUI
            if (!gameManager.TeleporterSelectDictionary.ContainsKey(Name))
            {
                gameManager.TeleporterSelectDictionary.Add(Name, this);
            }
        }

        //Marker Variable Assigning
        rightMarker = GetNode<Marker2D>("RightMarker");
        leftMarker = GetNode<Marker2D>("LeftMarker");

        UpdateTeleporterVisuals();
    }
    #endregion

    #region DebugUI Teleport
    //Called from the DebugUI when a button is clicked to teleport the Player to the specific Teleporter
    public void TeleportPlayer()
    {
        playerCB2D.GlobalPosition = teleportLocation;
        stateMachineScript.LastFacingDirection = linkedPortalLocation.playerFacingDirection;
        GD.Print($"Teleported to {Name} using DebugUI");
    }
    #endregion

    #region Teleporter Parameters
    //Sets the parameters of the current Teleporter
    public void UpdateTeleporterVisuals()
    {
        newName = "Teleporter_";

        switch (TeleporterSelected)
        {
            case TeleporterType.RightMarkerTeleporter:
                teleportLocation = rightMarker.GlobalPosition;
                rightMarker.Visible = true;
                leftMarker.Visible = false;
                playerFacingDirection = -1.0f;
                newName += "RightMarker";
                break;
            case TeleporterType.LeftMarkerTeleporter:
                teleportLocation = leftMarker.GlobalPosition;
                leftMarker.Visible = true;
                rightMarker.Visible = false;
                playerFacingDirection = 1.0f;
                newName += "LeftMarker";
                break;
            case TeleporterType.DEFAULT:
                leftMarker.Visible = true;
                rightMarker.Visible = true;
                teleportLocation = new();
                newName += "SelectType";
                if (Engine.IsEditorHint())
                {
                    Name = newName;
                }
                GD.PushWarning($"Invald Teleporter Type for {Name}, Choose Teleporter Left or Right Instead");
                break;
        }

        if (Engine.IsEditorHint())
        {
            // Allow automatic renaming only if name is default or matches our auto-generated pattern
            if (Name.ToString().StartsWith("Teleporter_"))
            {
                Name = newName;
            }
        }
    }
    #endregion

    #region PlayerInteract
    public void PlayerInteract()
    {
        if (linkedPortalLocation.teleportLocation == new Vector2())
        {
            GD.PushWarning($"Linked Portal Is Not Set Up In Export Window: {Name}");
            return;
        }

        playerCB2D.GlobalPosition = linkedPortalLocation.teleportLocation;
        stateMachineScript.LastFacingDirection = playerFacingDirection;
        GD.Print($"Teleported to {linkedPortalLocation.Name} from {Name} using an Interactive Teleporter");
    }
    #endregion

    #region Signals
    //Signal when the Player Enters the Teleporter
    public void OnPlayerBodyEntered(Node2D body)
    {
        if (linkedPortalLocation.teleportLocation == new Vector2())
        {
            GD.PushWarning($"Linked Portal Is Not Set Up In Export Window: {Name}");
            return;
        }

        //Will teleport the player if set to not interactable
        if (body != null && body.IsInGroup("Player") && !Interactable)
        {
            body.GlobalPosition = linkedPortalLocation.teleportLocation;
            stateMachineScript.LastFacingDirection = playerFacingDirection;
            GD.Print($"Teleported to {linkedPortalLocation.Name} from {Name}");
        }
    }
    #endregion
    #endregion
}