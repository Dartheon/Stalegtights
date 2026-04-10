using Godot;

public partial class WallState : States
{
    #region Variables
    #region DEBUG
    //DEBUG Variables Go Here...
    #endregion

    #region General
    /*References from StateMachine
    Player              - CharacterBody2D
    PlayerScript        - Player
    Gravity             - Float
    PlayerJumpVelocity  - Float
    */

    /*Variables from StateMachine that need direct Referenece
    stateMachine.smPlayerVelocity     - Vector2
    StateMachineScript.smInputManager.PlayerContinuousInputs
    */
    #endregion

    #region Animations
    //
    #endregion

    #region Movement
    private float enteringDirection;
    private int currentDirection;
    private Timer wallDetachTimer;
    private float waitTimer = 2.0f;

    public enum WallSlideState
    {
        InitialJump,   // player just touched wall
        ControlledSlide, // slow slide
        FastSlide      // accelerating slide
    }

    private WallSlideState wallSlideState;

    private float wallSlideTimer = 0f;

    private float initialSlideSpeed = 40f;
    private float maxSlideSpeed = 350f;

    private float accelerationDelay = 0.6f;   // when exponential acceleration begins
    private float accelerationRate = 2.5f;

    private float currentSlideSpeed;
    private float jumpStrength;

    //the different jump strengths used for jumping of the Wall
    private float momentumWallJumpPower;
    private float normalWallJumpPower;
    private float weakWallJumpPower;
    private float jumpHorizontalPower = 100f;
    private float jumpOutPower = 250f;
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

        wallDetachTimer = new();
        wallDetachTimer.WaitTime = waitTimer;
        wallDetachTimer.OneShot = true;

        AddChild(wallDetachTimer);

        wallDetachTimer.Timeout += () => NewStateChange = AIRSTATESTRING;

        momentumWallJumpPower = StateMachineScript.smPlayerJumpVelocity;
        normalWallJumpPower = StateMachineScript.smPlayerJumpVelocity / 2;
        weakWallJumpPower = StateMachineScript.smPlayerJumpVelocity / 3;
        #endregion
    }

    public override void Enter()
    {
        //set up stuff when entering this state
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Entering {WALLSTATESTRING}     from {PreviousState}");
        #endregion

        #region General
        NewStateChange = WALLSTATESTRING;
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        enteringDirection = StateMachineScript.LastFacingDirection;

        wallSlideTimer = 0f;
        currentSlideSpeed = initialSlideSpeed;

        wallSlideState = WallSlideState.InitialJump;
        StateMachineScript.smPlayerVelocity = Vector2.Zero;
        #endregion
    }

    public override void Update(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animations
        //
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
        #region Detect Jumping Off
        if (StateMachineScript.smWallDirection != 0)
        {
            GD.Print("wall");
            if (StateMachineScript.smInputManager.PlayerInputBuffers["wall_jump"])
            {
                GD.Print("jump off");
                WallJump(jumpHorizontalPower, -jumpStrength);
            }
        }
        #endregion
        #region Detect Diving Out - add code
        if (StateMachineScript.smWallDirection == 1)
        {
            if (StateMachineScript.smInputManager.PlayerInputBuffers["wall_jump"] && StateMachineScript.smInputManager.PlayerContinuousInputs["move_right"])
            {
                GD.Print("jump out");
                WallJump(jumpOutPower, 0);
            }
        }
        else if (StateMachineScript.smWallDirection == -1)
        {
            if (StateMachineScript.smInputManager.PlayerInputBuffers["wall_jump"] && StateMachineScript.smInputManager.PlayerContinuousInputs["move_left"])
            {
                GD.Print("jump out");
                WallJump(jumpOutPower, 0);
            }
        }
        #endregion
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
        #region Touching the Wall
        currentDirection = (StateMachineScript.smInputManager.PlayerContinuousInputs["move_right"] ? 1 : 0) - (StateMachineScript.smInputManager.PlayerContinuousInputs["move_left"] ? 1 : 0);
        #endregion

        #region Check to see if still on wall - if character collider is still interacting with the wall
        if (PlayerCB2D.IsOnWall() && !StateMachineScript.smInputManager.PlayerContinuousInputs["move_left"] && !StateMachineScript.smInputManager.PlayerContinuousInputs["move_right"])
        {
            if (wallDetachTimer.IsStopped())
            {
                wallDetachTimer.Start();
            }
        }
        else
        {
            wallDetachTimer.Stop();
        }

        #endregion

        #region Detect power slide out - add code
        //
        #endregion

        #region Detect for power slide - add code
        //
        #endregion
        #endregion
    }

    public override void PhysicsUpdate(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement

        #region Character Stun - add code
        //
        #endregion

        #region Touching the Ground
        if (PlayerCB2D.IsOnFloor())
        {
            NewStateChange = GROUNDSTATESTRING;
        }
        #endregion

        #region Process Sliding Down the Wall
        wallSlideTimer += (float)delta;

        // Determine slide phase
        if (wallSlideTimer < 0.2f)
        {
            wallSlideState = WallSlideState.InitialJump;
        }
        else if (wallSlideTimer < accelerationDelay)
        {
            wallSlideState = WallSlideState.ControlledSlide;
            currentSlideSpeed = initialSlideSpeed;
        }
        else
        {
            wallSlideState = WallSlideState.FastSlide;
        }

        // Handle slide speed
        if (wallSlideState == WallSlideState.FastSlide)
        {
            currentSlideSpeed += accelerationRate * currentSlideSpeed * (float)delta;
            currentSlideSpeed = Mathf.Min(currentSlideSpeed, maxSlideSpeed);
        }

        // Apply velocity
        StateMachineScript.smPlayerVelocity = new(PlayerCB2D.Velocity.X, currentSlideSpeed);
        #endregion

        #region Check Jump State
        switch (wallSlideState)
        {
            case WallSlideState.InitialJump:
                jumpStrength = momentumWallJumpPower;
                break;

            case WallSlideState.ControlledSlide:
                jumpStrength = normalWallJumpPower;
                break;

            case WallSlideState.FastSlide:
                jumpStrength = weakWallJumpPower;
                break;
        }
        #endregion

        #region Change State Logic
        if (NewStateChange != WALLSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
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
        #region Detect if Character is Dropping Down - add code
        //if direction out of wall and down enter air state
        if (StateMachineScript.smWallDirection != currentDirection && StateMachineScript.smInputManager.PlayerContinuousInputs["wall_drop_down"])
        {
            NewStateChange = AIRSTATESTRING;
        }
        #endregion

        #region Change State Logic
        if (NewStateChange != WALLSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
        #endregion
        #endregion
    }

    public override void Exit()
    {
        //clean up this state when moving out of it
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Exiting  {WALLSTATESTRING}");
        #endregion

        #region General
        NewStateChange = "";
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        StateMachineScript.smWallDirection = 0;
        wallDetachTimer.Stop();
        #endregion
    }
}