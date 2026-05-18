using System.Data;
using Godot;

public partial class AirState : States
{
    #region Variables
    #region DEBUG
    //DEBUG Variables Go Here...
    private bool firsttime = false;
    #endregion

    #region General
    /*References from StateMachine
    Player              - CharacterBody2D
    PlayerScript        - Player
    Gravity             - Float
    AirJumpBranch       - String
    */

    /*Variables from StateMachine that need direct Referenece
    StateMachineScript.smPlayerVelocity                          - Vector2
    StateMachineScript.PlayerAnimTree                            - AnimationTree
    StateMachineScript.LastFacingDirection                       - int
    StateMachineScript.isJumping                                 - bool
    StateMachineScript.smInputManager.PlayerInputBuffers[string] - bool
    */
    #endregion

    #region Animations
    //
    #endregion

    #region Movement
    [Export] private float inAirMoveSpeed = 300.0f; //speed of the character in the air
    private int currentDirection = 0;
    private float airControlMultiplier = 1.0f;
    private bool oppositeDirection;
    private bool belowSpeedCap;
    #endregion
    #endregion

    public override void Start()
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
        #endregion
    }

    public override void Enter()
    {
        //set up stuff when entering this state
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Entering {AIRSTATESTRING}      from {PreviousState}");

        #endregion

        #region General
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
        #endregion
    }

    public override void Update(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animation
        //Updates the LastFacingDirection based on velocity if no input
        if (StateMachineScript.smPlayerVelocity.X != 0 && InputManager.HorizontalInput == 0) // If player moving with no input, update facing
        {
            StateMachineScript.LastFacingDirection = StateMachineScript.smPlayerVelocity.X > 0 ? 1 : -1;
        }

        //Updates LastFacingDirection based on Input
        if (InputManager.HorizontalInput != 0) // If moving, update facing
        {
            StateMachineScript.LastFacingDirection = InputManager.HorizontalInput > 0 ? 1 : -1;
        }

        //Sets the blend using te LastFacingPosition
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/AIR STATE/AIR NORMAL/GROUNDJUMP/blend_position", StateMachineScript.LastFacingDirection);
        //WallJumps -  based off wall direction to keep consistant
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/AIR STATE/AIR NORMAL/WALLJUMPOUT/blend_position", StateMachineScript.smWallDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/AIR STATE/AIR NORMAL/WALLDIVEOUT/blend_position", StateMachineScript.smWallDirection);
        #endregion
    }

    #region InputBuffer
    public override void InputBuffer(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        //
        #endregion

        #region Movement
        //
        #endregion
    }
    #endregion

    public override void HandleContinuousInput(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        #region Check for Climbing Input
        if (InputManager.HorizontalInput != 0 && StateMachineScript.smPlayerVelocity.X < inAirMoveSpeed && StateMachineScript.smPlayerVelocity.X > -inAirMoveSpeed)
        {
            StateMachineScript.smPlayerVelocity.X = InputManager.HorizontalInput * inAirMoveSpeed;
        }

        if (!StateMachineScript.smTeleporting && PlayerScript.PlayerOnLadder && !InputManager.PlayerInputBuffers["ground_jump"] && StateMachineScript.smLadderDetachTimer <= 0 && InputManager.PlayerContinuousInputs["climb_up"] || InputManager.PlayerContinuousInputs["climb_down"])
        {
            StateMachineScript.ToClimbBranch = "AirToClimb";

            ChangeToNewState(CLIMBINGSTATESTRING);
            return;
        }
        #endregion

        #region Check Input for Moving Right/Left
        if (InputManager.HorizontalInput != 0)
        {
            StateMachineScript.BaseAcceleration = 20.0f;

            airControlMultiplier = 1.0f;

            oppositeDirection = Mathf.Sign(InputManager.HorizontalInput) != Mathf.Sign(StateMachineScript.smPlayerVelocity.X);

            if (StateMachineScript.smWallCancel && oppositeDirection)
            {
                airControlMultiplier = 0.15f;
            }

            belowSpeedCap = Mathf.Abs(StateMachineScript.smPlayerVelocity.X) < inAirMoveSpeed;

            // Allow acceleration if:
            // - below speed cap
            // - OR changing directions
            if (belowSpeedCap || oppositeDirection)
            {
                StateMachineScript.smPlayerVelocity.X += InputManager.HorizontalInput * StateMachineScript.RunAcceleration * airControlMultiplier;
            }
        }
        else
        {
            StateMachineScript.BaseAcceleration = 10.0f;

            StateMachineScript.smPlayerVelocity.X = Mathf.MoveToward(StateMachineScript.smPlayerVelocity.X, 0, StateMachineScript.RunAcceleration);
        }
        #endregion
        #endregion
    }

    public override void PhysicsUpdate(double delta)
    {

        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement
        #region Apply Gravity on Character
        /*
        two timers 
        gravity variable - max time before gravity has to kick in
        button pressed timer(button held timer)
        time pressed is >= gravity timer
        gravity timer = max jump
        200,gt = max height
        if button pressed timer < do short hop
        else if how long held * max jump
        bool set to true unless button pressed - apply gravity always
        */
        StateMachineScript.smPlayerVelocity.Y += Gravity * (float)delta; //When in the air the Y Velocity is equal to the Gravity Value 
        #endregion

        #region Landing - check to see if leaving air state or chaining to next jump from input during timing window - add code
        //
        #endregion

        #region Check for Knockback - add code
        //
        #endregion

        #region Check for Character interacting with the wall
        if (PlayerCB2D.IsOnWall())
        {
            Vector2 wallNormal = PlayerCB2D.GetWallNormal();

            if (wallNormal.X > 0)
            {
                StateMachineScript.smWallDirection = -1;
            }
            else if (wallNormal.X < 0)
            {
                StateMachineScript.smWallDirection = 1;
            }
        }
        else
        {
            StateMachineScript.smWallDirection = 0;
        }


        if (!StateMachineScript.smTeleporting && !StateMachineScript.smWallCancel && PlayerCB2D.IsOnWall())
        {
            Vector2 wallNormal = PlayerCB2D.GetWallNormal();

            // Only attach if moving INTO the wall
            if (Mathf.Sign(StateMachineScript.smPlayerVelocity.X) == -Mathf.Sign(wallNormal.X))
            {
                ChangeToNewState(WALLSTATESTRING);
                return;
            }
        }

        #endregion

        #region Check if the Character is Grounded
        if (!StateMachineScript.smTeleporting && PlayerCB2D.IsOnFloor())
        {
            StateMachineScript.IsLandingBranch = true; //for the animation tree

            ChangeToNewState(GROUNDSTATESTRING);
            return;
        }
        #endregion

        #region General
        //
        #endregion
        #endregion
    }

    public override void HandleInput(InputEvent @event)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        #region Check for Air Dodge Input - add code
        //
        #endregion

        #region Check for Diving Input - add code
        //
        #endregion

        #region Check for Double Jump Input - add code
        //
        #endregion

        #region CHeck for Long Jump Input - add code
        //
        #endregion

        #region Check for Short Jupt Input - add code
        //
        #endregion

        #region Check for Running Jump Input - add code
        //
        #endregion

        #region Check for Jump Input - add code
        //
        #endregion

        #region Check for Attacking Input - add code
        //
        #endregion
        #endregion
    }

    public override void Exit()
    {
        //clean up this state when moving out of it
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Exiting  {AIRSTATESTRING}");
        #endregion

        #region General
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
        #endregion
    }
}