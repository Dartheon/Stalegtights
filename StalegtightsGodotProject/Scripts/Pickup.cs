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
    #endregion

    #region Percentage Modifiers
    private float speedChangePercent;
    private float accelerationChangePercent;
    private float jumpChangePercent;
    private float gravityChangePercent;
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

        //Setting the Percents the Variables will be changed by
        speedChangePercent = 10.0f;
        accelerationChangePercent = 10.0f;
        jumpChangePercent = 10.0f;
        gravityChangePercent = 10.0f;

    }
    #endregion

    #region Pickup Signal
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
                    groundStateScript.GroundMoveSpeed *= 1 + speedChangePercent / 100f;
                    break;
                case "SpeedPickupDown":
                    groundStateScript.GroundMoveSpeed *= 1 - speedChangePercent / 100f;
                    break;

                //Changes Player Acceleration
                case "AccelerationPickupUp":
                    stateMachine.RunAcceleration *= 1 + accelerationChangePercent / 100f;
                    break;
                case "AccelerationPickupDown":
                    stateMachine.RunAcceleration *= 1 - accelerationChangePercent / 100f;
                    break;

                //Changes Player Jump Height
                case "JumpHeightPickupUp":
                    stateMachine.smPlayerJumpVelocity *= 1 + jumpChangePercent / 100f;
                    break;
                case "JumpHeightPickupDown":
                    stateMachine.smPlayerJumpVelocity *= 1 - jumpChangePercent / 100f;
                    break;

                //Changes Player Gravity
                case "GravityPickupUp":
                    stateMachine.smGravity *= 1 + gravityChangePercent / 100f;
                    break;
                case "GravityPickupDown":
                    stateMachine.smGravity *= 1 - gravityChangePercent / 100f; ;
                    break;

                case "Pickup":
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