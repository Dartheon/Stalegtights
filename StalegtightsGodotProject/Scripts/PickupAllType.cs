using Godot;

[Tool]
public partial class PickupAllType : Area2D
{
    #region Variables
    #region Class Scripts
    private StateMachine stateMachine;
    private GroundState groundStateScript;
    #endregion

    #region Enums
    public enum PickupType
    {
        SPEEDUP,
        SPEEDDOWN,
        ACCELERATIONUP,
        ACCELERATIONDOWN,
        GRAVITYUP,
        GRAVITYDOWN,
        JUMPHEIGHTUP,
        JUMPHEIGHTDOWN,
        DEFAULT
    };
    #endregion

    #region General
    //Sets the modifier before changing the main modifier in the StateMachineScript
    private static float SPEEDMODIFIER = 1.0f;
    private static float ACCELMODIFIER = 1.0f;
    private static float JUMPMODIFIER = 1.0f;
    private static float GRAVITYMODIFIER = 1.0f;

    [ExportSubgroup("Pickup Selection")]
    //[Export] public PickupType PickupSelected { get; set; } = PickupType.DEFAULT; //Used by the switch statments to determine the nature of the pickup
    [Export]
    public PickupType PickupSelected
    {
        get => _pickupSelected;
        set
        {
            _pickupSelected = value;
            if (Engine.IsEditorHint())
                UpdatePickupVisuals();
        }
    }
    private PickupType _pickupSelected = PickupType.DEFAULT;
    private Sprite2D pickupSprite;
    private int pickupSpriteFrame = 0; //sets the frame for the sprite for the specific powerup choosen
    [Export] public bool Interactable { get; set; } = false; //sets if the pickup needs the Interact Input to be used
    [Export] public bool Consumable { get; set; } = false; //sets if the pickup will dissapear once used
    private string newName;
    #endregion

    #region Pickup Modifiers
    [ExportSubgroup("Pickup Modifier Changes")]
    //Sets the percentage to change the players stats
    [Export] public float SpeedChange { get; set; } = 0.1f;
    [Export] public float AccelerationChange { get; set; } = 0.1f;
    [Export] public float JumpChange { get; set; } = 0.1f;
    [Export] public float GravityChange { get; set; } = 0.1f;
    #endregion

    #region Pickup Effects
    [ExportSubgroup("Pickup Movement")]
    //used by the powerup to move slightly during runtime
    [Export] public float BobHeight { get; set; } = 5.0f; //setting how far coin will bob up and down from starting point

    [Export] public float BobSpeed { get; set; } = 5.0f; //how fast the coin moves

    private float time = 0.0f; //used for keeping track of time to calculate the sin wave
    private float movement; // initializing the variable that will hold the sin wave formula

    private Vector2 pos; //initializing the variable to change position of the coin
    private float startY; //initializing the variable to set the starting point of the coin
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        //Initilizes Scripts
        if (!Engine.IsEditorHint())
        {
            stateMachine = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
            groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");

            //Setting up Pickup Effects Variables
            pos = Position;//setting the position of the object to a variable to make future changes
            startY = pos.Y; //setting up the starting point for the object to use later
        }

        pickupSprite = GetNode<Sprite2D>("PickupSprite2D");

        UpdatePickupVisuals();
    }
    #endregion

    #region Process
    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) { return; }

        PickupMovement(delta);
    }
    #endregion

    #region Visual Changes
    public void UpdatePickupVisuals()
    {
        newName = "Pickup_";

        if (pickupSprite == null) { return; }

        //switch to set initial variables for selected pickup type
        switch (PickupSelected)
        {
            //Sets up the base properties of the selected Pickup
            //Player Speed
            case PickupType.SPEEDUP:
                pickupSpriteFrame = 1;
                newName += "SpeedUp";
                break;
            case PickupType.SPEEDDOWN:
                pickupSpriteFrame = 5;
                newName += "SpeenDown";
                break;

            //Player Acceleration
            case PickupType.ACCELERATIONUP:
                pickupSpriteFrame = 2;
                newName += "AccelUp";
                break;
            case PickupType.ACCELERATIONDOWN:
                pickupSpriteFrame = 6;
                newName += "AccelDown";
                break;

            //Player Jump Height
            case PickupType.JUMPHEIGHTUP:
                pickupSpriteFrame = 3;
                newName += "JumpHeightUp";
                break;
            case PickupType.JUMPHEIGHTDOWN:
                pickupSpriteFrame = 7;
                newName += "JumpHeightDown";
                break;

            //Player Gravity
            case PickupType.GRAVITYUP:
                pickupSpriteFrame = 4;
                newName += "GravityUp";
                break;
            case PickupType.GRAVITYDOWN:
                pickupSpriteFrame = 8;
                newName += "GravityDown";
                break;

            case PickupType.DEFAULT:
                pickupSpriteFrame = 0;
                newName = "PickupSelectType";
                GD.PushWarning($"Default Pickup Selected for {Name}. Please Select PickupType In Export Window for {Name}");
                break;
        }

        pickupSprite.Frame = pickupSpriteFrame; //Changes the Sprite based on the pickup selected

        if (Engine.IsEditorHint())
        {
            Name = newName;
        }
    }
    #endregion

    #region Pickup Effects
    public void PickupMovement(double delta)
    {
        time += (float)delta; //increases number slowly by the time

        movement = (Mathf.Sin(time * BobSpeed) + 1) / 2; //creating a sin wave based on time and speed set

        pos.Y = startY + (movement * BobHeight); //setting the Y variable for the objects position

        Position = pos; //applying the position changes to the object
    }
    #endregion

    #region PlayerInteract
    public void PlayerInteract()
    {
        //Modifies the Players Stats
        switch (PickupSelected)
        {
            //Changes Player Speed
            case PickupType.SPEEDUP:
                SPEEDMODIFIER += SpeedChange;
                groundStateScript.MoveSpeedModifier = SPEEDMODIFIER;
                break;
            case PickupType.SPEEDDOWN:
                SPEEDMODIFIER -= SpeedChange;
                groundStateScript.MoveSpeedModifier = SPEEDMODIFIER;
                break;

            //Changes Player Acceleration
            case PickupType.ACCELERATIONUP:
                ACCELMODIFIER += AccelerationChange;
                stateMachine.RunAccelerationModifier = ACCELMODIFIER;
                break;
            case PickupType.ACCELERATIONDOWN:
                ACCELMODIFIER -= AccelerationChange;
                stateMachine.RunAccelerationModifier = ACCELMODIFIER;
                break;

            //Changes Player Jump Height
            case PickupType.JUMPHEIGHTUP:
                JUMPMODIFIER += JumpChange;
                stateMachine.JumpModifier = JUMPMODIFIER;
                break;
            case PickupType.JUMPHEIGHTDOWN:
                JUMPMODIFIER -= JumpChange;
                stateMachine.JumpModifier = JUMPMODIFIER;
                break;

            //Changes Player Gravity
            case PickupType.GRAVITYUP:
                GRAVITYMODIFIER += GravityChange;
                stateMachine.GravityModifier = GRAVITYMODIFIER;
                break;
            case PickupType.GRAVITYDOWN:
                GRAVITYMODIFIER -= GravityChange;
                stateMachine.GravityModifier = GRAVITYMODIFIER;
                break;

            case PickupType.DEFAULT:
                GD.PushWarning($"Default Pickup Selected for {Name}. Please Select PickupType In Export Window for {Name}");
                return;
        }

        if (Consumable)
        {
            //Removes Pickup after being Collected
            QueueFree();
        }
    }
    #endregion

    #region Signal
    public void OnPickupBodyEntered(Node body)
    {
        if (body != null && body.IsInGroup("Player"))
        {
            if (Interactable) { return; } //If its interactable the Player will handle the Interaction insead of this signal
            else
            {
                //PlayerSpeed, Acceleration, Jump Height, Gravity
                PlayerInteract();
            }
        }
    }
    #endregion
    #endregion
}