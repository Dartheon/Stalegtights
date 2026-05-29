using Godot;
using System.Collections.Generic;

public partial class InputManager : Node
{
    #region Variables
    private StateMachine stateMachineScript;
    private GroundState groundStateScript;
    private Player playerCB2D;
    private MenuUI menuUI;

    //These inputs are for one-time presses that need timers to reset the bool value
    public Dictionary<string, bool> PlayerInputBuffers { get; private set; } = new()
    {
        //Player Inputs That Require a Timer to Reset
        { "ground_jump", false },
        { "wall_jump", false },
        { "wall_jump_right",false},
        { "wall_jump_left",false},
        { "roll",false},
    };

    //These inputs are for continuous key presses that the bool needs to be reset in the code
    public Dictionary<string, bool> PlayerContinuousInputs { get; private set; } = new()
    {
        //Player Inputs That Get Reset In the Code
        { "interact", false },
        { "duck", false },
        { "crawling_left", false },
        { "crawling_right", false },
        { "climb_up", false },
        { "climb_down", false },
        { "climb_down_ladder_top", false },
        { "climb_enter_ladder",false},
        { "wall_drop_down", false },
        { "slide",false },
        { "power_slide",false },

        //Menu
        { "pause_menu", false },
        { "main_menu",false },

        //Debug Inputs
        { "engine_scale_up", false },
        { "engine_scale_down", false },
        { "engine_scale_reset", false },
    };

    public Timer GroundJumpTimer { get; private set; }
    public Timer WallJumpTimer { get; private set; }

    public bool JumpOutWallCancel { get; private set; } = false;

    //Mainly for analog stick Vector2 using direction inputs
    public float HorizontalInput { get; private set; } = 0.0f;
    public float VerticalInput { get; private set; } = 0.0f;
    public Vector2 RawInput { get; private set; }

    //For reading the direction input an using that to set a bool for specific directions pressed
    public bool UpIntent { get; private set; }
    public bool DownIntent { get; private set; }
    public bool RightIntent { get; private set; }
    public bool LeftIntent { get; private set; }

    //Controller Deadzone setting. 0 is center, -1 is left and up, 1 is right and down. Deadzone set to amount before limit to account for stick drift
    public float AnalogDeadzoneMax { get; private set; } = 0.8f;
    public float AnalogDeadzoneMin { get; private set; } = 0.2f;
    #endregion

    #region Ready
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");
        menuUI = GetNode<MenuUI>("/root/Main/UILayer/MenuUI");
        GroundJumpTimer = GetNode<Timer>("GroundJumpTimer");
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        #region General
        //
        #endregion

        #region Debug
        //
        #endregion

        #region Animation
        //
        #endregion
    }
    #endregion

    #region Physic Process
    public override void _PhysicsProcess(double delta)
    {
        #region General
        //Player Interact
        PlayerContinuousInputs["interact"] = Input.IsActionJustPressed("interact") && (GameManager.InteractablesEntered?.Count > 0);
        #endregion

        #region Debug
        //
        #endregion

        #region Movement
        //Moving
        RawInput = Input.GetVector("move_left", "move_right", "up", "down");

        VerticalInput = ApplyDeadzone(RawInput.Y);

        if (stateMachineScript.smDisableVerticalInput)
        {
            VerticalInput = 0;
        }
        else
        {
            // Disable UP only
            if (stateMachineScript.smDisableUpInput && VerticalInput < 0)
            {
                VerticalInput = 0;
            }

            // Disable DOWN only
            if (stateMachineScript.smDisableDownInput && VerticalInput > 0)
            {
                VerticalInput = 0;
            }
        }

        UpIntent = VerticalInput < -0.5f;
        DownIntent = VerticalInput > 0.5f;

        HorizontalInput = ApplyDeadzone(RawInput.X);

        if (stateMachineScript.smDisableHorizontalInput)
        {
            HorizontalInput = 0;
        }
        else
        {
            // Disable LEFT only
            if (stateMachineScript.smDisableLeftInput && HorizontalInput < 0)
            {
                HorizontalInput = 0;
            }

            // Disable RIGHT only
            if (stateMachineScript.smDisableRightInput && HorizontalInput > 0)
            {
                HorizontalInput = 0;
            }
        }

        LeftIntent = HorizontalInput < -0.5f;
        RightIntent = HorizontalInput > 0.5f;

        //Rolling
        PlayerInputBuffers["roll"] = Input.IsActionJustPressed("roll") && (LeftIntent || RightIntent) && playerCB2D.IsOnFloor();

        //Sliding
        PlayerContinuousInputs["slide"] = Input.IsActionPressed("slide") && playerCB2D.IsOnFloor();

        if (PlayerContinuousInputs["slide"] && groundStateScript.CurrentMovementState != GroundState.GroundMovementStates.Sliding)
        {
            groundStateScript.CurrentSlideState = GroundState.GroundSlideState.InitialSlide;
            groundStateScript.SlideTimer = 0f;
        }

        if (!PlayerContinuousInputs["slide"])
        {
            groundStateScript.SlideCancel = false;
        }

        //Ducking
        PlayerContinuousInputs["duck"] = DownIntent && !UpIntent && !RightIntent && !LeftIntent && playerCB2D.IsOnFloor() && stateMachineScript.smPlayerVelocity == Vector2.Zero;

        //Crawling
        PlayerContinuousInputs["crawling_left"] = DownIntent && LeftIntent && !RightIntent && !UpIntent && stateMachineScript.smPlayerVelocity.X > -groundStateScript.CrawlGroundSpeed;
        PlayerContinuousInputs["crawling_right"] = DownIntent && RightIntent && !LeftIntent && !UpIntent && stateMachineScript.smPlayerVelocity.X < groundStateScript.CrawlGroundSpeed;

        //Climbing
        PlayerContinuousInputs["climb_up"] = UpIntent && !DownIntent && playerCB2D.PlayerOnLadder;
        PlayerContinuousInputs["climb_down"] = DownIntent && !UpIntent && playerCB2D.PlayerOnLadder;
        PlayerContinuousInputs["climb_down_ladder_top"] = DownIntent && !UpIntent && !RightIntent && !LeftIntent && playerCB2D.IsOnFloor() && Player.PlayerAboveLadder.Count > 0;
        PlayerContinuousInputs["climb_enter_ladder"] = UpIntent && !DownIntent && !RightIntent && !LeftIntent && playerCB2D.PlayerOnLadder && Player.PlayerAboveLadder.Count == 0;

        //Wall
        PlayerContinuousInputs["wall_drop_down"] = DownIntent && !UpIntent && !RightIntent && !LeftIntent;

        //Jumping
        if (Input.IsActionJustPressed("jump") && (playerCB2D.IsOnFloorOnly() || playerCB2D.PlayerOnLadder || !GroundJumpTimer.IsStopped()))
        {
            PlayerInputBuffers["ground_jump"] = true;
        }

        if (Input.IsActionJustPressed("jump") && playerCB2D.IsOnWallOnly())
        {
            PlayerInputBuffers["wall_jump"] = true;
        }

        //TO DO: add a ground wall jump when in a corner using isonfloor() and isonwall() with a groundjump

        //Wall Jump Out
        if (playerCB2D.IsOnWallOnly() && RightIntent && !LeftIntent && !UpIntent && !DownIntent)
        {
            PlayerInputBuffers["wall_jump_right"] = true;
            JumpOutWallCancel = false;
        }

        if (playerCB2D.IsOnWallOnly() && LeftIntent && !RightIntent && !UpIntent && !DownIntent)
        {
            PlayerInputBuffers["wall_jump_left"] = true;
            JumpOutWallCancel = false;
        }

        //Wall Jump Out Cancel when direction is released
        if (Input.IsActionJustReleased("move_left") || Input.IsActionJustReleased("move_right"))
        {
            JumpOutWallCancel = true;
        }

        if (playerCB2D.IsOnWall() && (UpIntent || DownIntent) && !RightIntent && !LeftIntent)
        {
            PlayerInputBuffers["wall_jump_left"] = false;
            PlayerInputBuffers["wall_jump_right"] = false;
        }
        #endregion
    }
    #endregion

    #region Unhandled Input - Menus/UI Interaction
    public override void _UnhandledInput(InputEvent @event)
    {
        #region General
        //Main Menu
        PlayerContinuousInputs["main_menu"] = Input.IsActionJustPressed("main_menu");

        //Pause Menu
        PlayerContinuousInputs["pause_menu"] = Input.IsActionJustPressed("pause_menu") && !menuUI.StartMenu.Visible;
        #endregion

        #region Debug
        //Player Interact
        PlayerContinuousInputs["engine_scale_up"] = Input.IsActionJustPressed("engine_scale_up");
        //Player Interact
        PlayerContinuousInputs["engine_scale_down"] = Input.IsActionJustPressed("engine_scale_down");
        //Player Interact
        PlayerContinuousInputs["engine_scale_reset"] = Input.IsActionJustPressed("engine_scale_reset");
        #endregion
    }
    #endregion
    #region Methods
    private float ApplyDeadzone(float value)
    {
        if (Mathf.Abs(value) < AnalogDeadzoneMin)
        { return 0; }

        return Mathf.Sign(value) * Mathf.InverseLerp(AnalogDeadzoneMin, AnalogDeadzoneMax, Mathf.Min(Mathf.Abs(value), AnalogDeadzoneMax));
    }
    #endregion
}