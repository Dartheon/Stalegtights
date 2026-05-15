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
    private bool wallDive = false;
    private bool jumpOut = false;
    private bool powerJumpOut = false;
    #endregion

    #region Movement
    private int enteringDirection; //holds the direction player was when entering the wall
    private int wallDirection;
    private Timer wallDetachTimer;
    private float waitTimer = 1.0f; //used to control how long player attaches to the wall after no input is detected to hold to wall

    private Timer wallJumpOutTimer;
    private float jumpOutWaitTimer = 1.0f;

    private enum WallSlideState
    {
        InitialJump,   // player just touched wall
        ControlledSlide, // slow slide
        FastSlide      // accelerating slide
    }

    private WallSlideState wallSlideState; //holds the current slide state enum used for switches

    private float wallSlideTimer = 0f; //to keep track of how long player is on the wall and used to transition through wall slide/jump states

    private float initialSlideSpeed = 40f; //regular slide speed for initial and regular wall sliding
    private float maxSlideSpeed = 160f; //speed used for when on the wall too long without jumping off

    private float accelerationDelay = 0.6f; // time taken to switch from initial jump to regular jump
    private float fastSlideStart = 2.0f; //time taken to switch from regular jump/slide to fast slide/weak jump
    private float accelerationRate = 2.5f; //the rate used for sliding down walls

    private float currentSlideSpeed; //used for holding speed sliding down wall gradually faster 
    private float jumpStrength; //Set to how strong jumping out of the wall is


    //the different jump strengths used for jumping of the Wall
    private float momentumWallJumpPower; //Will be set to a formula based on current player momentum to assist jump power off wall
    private float normalWallJumpPower; //Set to default jump power, currently set to PlayerJumpVelocity
    private float weakWallJumpPower; //Set to a constant weaker jump based on the default jump power
    private float jumpHorizontalPower = 500f; //diving out horizontal power
    private float jumpVerticalPower = -100f; //diving out vertical power
    private float jumpOutPower = 750f; //horizontal power to push away from wall when jumping up
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
        //Wall Detach Timer
        wallDetachTimer = new()
        {
            WaitTime = waitTimer,
            OneShot = true
        };

        AddChild(wallDetachTimer);

        wallDetachTimer.Timeout += () => ChangeToNewState(AIRSTATESTRING);

        //Jump Out Timer
        wallJumpOutTimer = new()
        {
            WaitTime = jumpOutWaitTimer,
            OneShot = true
        };

        AddChild(wallJumpOutTimer);

        wallJumpOutTimer.Timeout += () =>
        {
            InputManager.PlayerInputBuffers["wall_jump_right"] = false;
            InputManager.PlayerInputBuffers["wall_jump_left"] = false;
        };

        momentumWallJumpPower = PlayerJumpVelocity * 1.5f;
        normalWallJumpPower = PlayerJumpVelocity;
        weakWallJumpPower = PlayerJumpVelocity / 2;
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
        if (!wallJumpOutTimer.IsStopped())
        {
            wallJumpOutTimer.Stop();
        }
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        enteringDirection = StateMachineScript.LastFacingDirection;
        wallDirection = StateMachineScript.smWallDirection;

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
        //Normal
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/WALL STATE/WALL NORMAL/WALLIDLE/blend_position", StateMachineScript.smWallDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/WALL STATE/WALL NORMAL/WALLSLIDEDOWN/blend_position", StateMachineScript.smWallDirection);

        StateMachineScript.WallSlideDown = wallSlideTimer > 0.01f ? true : false;
        StateMachineScript.WallCling = wallSlideTimer > 0.01f ? false : true;
        #endregion
    }

    #region InputBuffer
    public override void InputBuffer(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        #region Check Jump State
        //sets the jumpStrength based on switch, placed here so jumpStrength is available before being called on when jumping
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
                currentSlideSpeed += accelerationRate * currentSlideSpeed * (float)delta;
                currentSlideSpeed = Mathf.Min(currentSlideSpeed, maxSlideSpeed);
                break;
        }
        #endregion
        #endregion

        #region Movement
        #region Detect Diving Out
        if (StateMachineScript.smWallDirection == 1)
        {
            if (InputManager.PlayerInputBuffers["wall_jump"] && InputManager.PlayerInputBuffers["wall_jump_right"])
            {
                wallDive = true;
                WallJump(-jumpOutPower, jumpVerticalPower);
                return;
            }
        }
        else if (StateMachineScript.smWallDirection == -1)
        {
            if (InputManager.PlayerInputBuffers["wall_jump"] && InputManager.PlayerInputBuffers["wall_jump_left"])
            {
                wallDive = true;
                WallJump(jumpOutPower, jumpVerticalPower);
                return;
            }
        }

        if (InputManager.JumpOutWallCancel)
        {
            if (wallJumpOutTimer.IsStopped())
            {
                wallJumpOutTimer.Start();
            }
        }
        else
        {
            if (!wallJumpOutTimer.IsStopped())
            {
                wallJumpOutTimer.Stop();
            }
        }
        #endregion

        #region Detect Jumping Off
        if (StateMachineScript.smWallDirection != 0 && InputManager.PlayerInputBuffers["wall_jump"])
        {
            jumpOut = true;
            WallJump(-wallDirection * jumpHorizontalPower, jumpStrength);
            return;
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
        #region Check to see if still on wall - if character collider is still interacting with the wall
        if (PlayerCB2D.IsOnWall() && ((!InputManager.LeftIntent && !InputManager.RightIntent) || (StateMachineScript.smWallDirection < 0 && InputManager.RightIntent) || (StateMachineScript.smWallDirection > 0 && InputManager.LeftIntent)))
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
        if (!StateMachineScript.smTeleporting && PlayerCB2D.IsOnFloor())
        {
            ChangeToNewState(GROUNDSTATESTRING);
            return;
        }
        #endregion

        #region Process Sliding Down the Wall
        wallSlideTimer += (float)delta;

        // Determine slide phase
        if (wallSlideTimer < accelerationDelay)
        {
            wallSlideState = WallSlideState.InitialJump;
        }
        else if (wallSlideTimer > accelerationDelay && wallSlideTimer < fastSlideStart)
        {
            wallSlideState = WallSlideState.ControlledSlide;
            currentSlideSpeed = initialSlideSpeed;
        }
        else if (wallSlideTimer > fastSlideStart)
        {
            wallSlideState = WallSlideState.FastSlide;
        }

        // Prevent upward motion
        float yVelocity = PlayerCB2D.Velocity.Y;
        if (yVelocity < 0)
        {
            yVelocity = 0;
        }

        // Apply slide speed (always downward)
        yVelocity = Mathf.Max(yVelocity, currentSlideSpeed);

        StateMachineScript.smPlayerVelocity = new(0, yVelocity);
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
        #region Detect if Character is Dropping Down
        //if direction out of wall and down enter air state
        if (InputManager.PlayerContinuousInputs["wall_drop_down"])
        {
            ChangeToNewState(AIRSTATESTRING);
            return;
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
        if (!wallJumpOutTimer.IsStopped())
        {
            wallJumpOutTimer.Stop();
        }
        #endregion

        #region Animations
        StateMachineScript.WallCling = false;
        StateMachineScript.WallSlideDown = false;
        #endregion

        #region Movement
        StateMachineScript.smWallDirection = 0;
        #endregion
    }

    //Wall State Methods
    public void WallJump(float jumpHorizontal, float jumpUpStrength)
    {
        //Diving Out
        if (wallDive)
        {
            StateMachineScript.WallDiveOut = true;
            StateMachineScript.WallJumpOut = false;
            StateMachineScript.WallPowerSlideOut = false;
            StateMachineScript.LadderJump = false;
        }
        //Wall Jump Out
        else if (jumpOut)
        {
            StateMachineScript.WallJumpOut = true;
            StateMachineScript.WallDiveOut = false;
            StateMachineScript.WallPowerSlideOut = false;
            StateMachineScript.LadderJump = false;
        }
        else if (powerJumpOut)
        {
            StateMachineScript.WallPowerSlideOut = true;
            StateMachineScript.WallJumpOut = false;
            StateMachineScript.WallDiveOut = false;
            StateMachineScript.LadderJump = false;
        }

        //sets the jump velocity for the player to trigger
        StateMachineScript.smPlayerVelocity = new(jumpHorizontal, jumpUpStrength);

        InputManager.PlayerInputBuffers["wall_jump"] = false;
        InputManager.PlayerInputBuffers["wall_jump_right"] = false;
        InputManager.PlayerInputBuffers["wall_jump_left"] = false;

        wallDive = false;
        jumpOut = false;
        powerJumpOut = false;


        //Cancels the players ability to reattach to the wall immediatly after jumping
        WallCancel();

        //change State to air state
        StateMachineScript.TransitionToState(AIRSTATESTRING);
    }

    public async void WallCancel()
    {
        StateMachineScript.smWallCancel = true;

        await ToSignal(GetTree().CreateTimer(0.25f), SceneTreeTimer.SignalName.Timeout);

        StateMachineScript.smWallCancel = false;
    }
}