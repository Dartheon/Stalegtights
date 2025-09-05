using Godot;

public partial class Pickup : Area2D
{
    #region Variables
    #region Class Scripts
    private StateMachine stateMachine;
    private GroundState groundStateScript;
    #endregion

    #region General
    private string nodeName;
    private static float SPEEDMODIFIER = 1.0f;
    private static float ACCELMODIFIER = 1.0f;
    private static float JUMPMODIFIER = 1.0f;
    private static float GRAVITYMODIFIER = 1.0f;
    #endregion

    #region Pickup Effects
    private float bobHeight = 5.0f; //setting how far coin will bob up and down from starting point
    private float bobSpeed = 5.0f; //how fast the coin moves

    private float time = 0.0f; //used for keeping track of time to calculate the sin wave
    private float movement; // initializing the variable that will hold the sin wave formula

    private Vector2 pos; //initializing the variable to change position of the coin
    private float startY; //initializing the variable to set the starting point of the coin
    #endregion

    #region Pickup Modifiers
    //Positive
    private float speedChange = 0.1f;
    private float accelerationChange = 0.1f;
    private float jumpChange = 0.1f;
    private float gravityChange = 0.1f;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        //Initilizes Scripts
        stateMachine = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");

        //Gets the Current Node's name and sets the Variable
        nodeName = Name;

        //Setting up Pickup Effects Variables
        pos = Position;//setting the position of the object to a variable to make future changes
        startY = pos.Y; //setting up the starting point for the object to use later

    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        PickupMovement(delta);
    }
    #endregion

    #region Pickup Effects
    public void PickupMovement(double delta)
    {
        time += (float)delta; //increases number slowly by the time

        movement = (Mathf.Sin(time * bobSpeed) + 1) / 2; //creating a sin wave based on time and speed set

        pos.Y = startY + (movement * bobHeight); //setting the Y variable for the objects position

        Position = pos; //applying the position changes to the object
    }
    #endregion

    #region Signal
    public void OnPickupBodyEntered(Node body)
    {
        if (body != null && body.IsInGroup("Player"))
        {
            //Code logic for what happens when picked up
            //PlayerSpeed, Acceleration, Jump Height, Gravity
            switch (nodeName)
            {
                //Changes Player Speed
                case "SpeedPickupUp":
                    SPEEDMODIFIER += speedChange;
                    groundStateScript.MoveSpeedModifier = SPEEDMODIFIER;
                    break;
                case "SpeedPickupDown":
                    SPEEDMODIFIER -= speedChange;
                    groundStateScript.MoveSpeedModifier = SPEEDMODIFIER;
                    break;

                //Changes Player Acceleration
                case "AccelerationPickupUp":
                    ACCELMODIFIER += accelerationChange;
                    stateMachine.RunAccelerationModifier = ACCELMODIFIER;
                    break;
                case "AccelerationPickupDown":
                    ACCELMODIFIER -= accelerationChange;
                    stateMachine.RunAccelerationModifier = ACCELMODIFIER;
                    break;

                //Changes Player Jump Height
                case "JumpHeightPickupUp":
                    JUMPMODIFIER += jumpChange;
                    stateMachine.JumpModifier = JUMPMODIFIER;
                    break;
                case "JumpHeightPickupDown":
                    JUMPMODIFIER -= jumpChange;
                    stateMachine.JumpModifier = JUMPMODIFIER;
                    break;

                //Changes Player Gravity
                case "GravityPickupUp":
                    GRAVITYMODIFIER += gravityChange;
                    stateMachine.GravityModifier = GRAVITYMODIFIER;
                    break;
                case "GravityPickupDown":
                    GRAVITYMODIFIER -= gravityChange;
                    stateMachine.GravityModifier = GRAVITYMODIFIER;
                    break;

                case "PickupTemplate":
                    GD.PushWarning("Default Node Used. Please Use Different Named Pickup Node");
                    return;
            }

            //Removes Pickup after being Collected
            QueueFree();
        }
    }
    #endregion
    #endregion
}