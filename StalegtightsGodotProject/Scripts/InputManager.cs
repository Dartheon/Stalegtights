using Godot;
using System;
using System.Collections.Generic;

public partial class InputManager : Node
{
    #region Variables
    private StateMachine stateMachineScript;
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
    };

    //These inputs are for continuous key presses that the bool needs to be reset in the code
    public Dictionary<string, bool> PlayerContinuousInputs { get; private set; } = new()
    {
        //Player Inputs That Get Reset In the Code
        { "move_right", false },
        { "move_left", false },
        { "interact", false },
        { "duck", false },
        { "crawling_left", false },
        { "crawling_right", false },
        { "climb_up", false },
        { "climb_down", false },
        { "climb_down_ladder_top", false },
        { "wall_drop_down", false },

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

    //Controller Deadzone setting. 0 is center, -1 is left and up, 1 is right and down. Deadzone set to amount before limit to account for stick drift
    public float AnalogDeadzoneMax { get; private set; } = 0.8f;
    public float AnalogDeadzoneMin { get; private set; } = 0.2f;
    #endregion

    #region Ready
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");
        menuUI = GetNode<MenuUI>("/root/Main/UILayer/MenuUI");
        GroundJumpTimer = GetNode<Timer>("GroundJumpTimer");
        WallJumpTimer = GetNode<Timer>("WallJumpTimer");
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
        //
        #endregion

        #region Debug
        //
        #endregion

        #region Movement
        //Moving
        RawInput = Input.GetVector("move_left", "move_right", "up", "down");

        HorizontalInput = ApplyDeadzone(RawInput.X);
        VerticalInput = ApplyDeadzone(RawInput.Y);

        //Ducking
        PlayerContinuousInputs["duck"] = Input.IsActionPressed("duck");

        //Crawling
        PlayerContinuousInputs["crawling_left"] = Input.IsActionPressed("duck") && Input.IsActionPressed("move_left");
        PlayerContinuousInputs["crawling_right"] = Input.IsActionPressed("duck") && Input.IsActionPressed("move_right");

        //Climbing
        PlayerContinuousInputs["climb_up"] = Input.IsActionPressed("climb_up") && playerCB2D.PlayerOnLadder;
        PlayerContinuousInputs["climb_down"] = Input.IsActionPressed("climb_down") && playerCB2D.PlayerOnLadder;
        PlayerContinuousInputs["climb_down_ladder_top"] = Input.IsActionPressed("climb_down") && playerCB2D.IsOnFloor() && Player.PlayerAboveLadder.Count > 0;

        //Wall
        PlayerContinuousInputs["wall_drop_down"] = Input.IsActionPressed("down");
        #endregion
    }
    #endregion

    #region Unhandled Input
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        #region General
        //Player Interact
        PlayerContinuousInputs["interact"] = Input.IsActionJustPressed("interact") && (GameManager.InteractablesEntered?.Count > 0);

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

        #region Movement
        //Player Jump
        if (Input.IsActionJustPressed("jump") && (playerCB2D.IsOnFloor() || playerCB2D.PlayerOnLadder || !GroundJumpTimer.IsStopped()))
        {
            PlayerInputBuffers["ground_jump"] = true;
        }

        if (Input.IsActionJustPressed("jump") && playerCB2D.IsOnWall() || !WallJumpTimer.IsStopped())
        {
            PlayerInputBuffers["wall_jump"] = true;
        }

        //Wall Jump Out
        if (playerCB2D.IsOnWall() && Input.IsActionPressed("wall_jump_right"))
        {
            PlayerInputBuffers["wall_jump_right"] = true;
            JumpOutWallCancel = false;
        }

        if (playerCB2D.IsOnWall() && Input.IsActionPressed("wall_jump_left"))
        {
            PlayerInputBuffers["wall_jump_left"] = true;
            JumpOutWallCancel = false;
        }

        //Wall Jump Out Cancel when direction is released
        if (Input.IsActionJustReleased("wall_jump_left") || Input.IsActionJustReleased("wall_jump_right"))
        {
            JumpOutWallCancel = true;
        }

        if (playerCB2D.IsOnWall() && Input.IsActionJustPressed("up"))
        {
            PlayerInputBuffers["wall_jump_left"] = false;
            PlayerInputBuffers["wall_jump_right"] = false;
        }

        if (playerCB2D.IsOnWall() && Input.IsActionJustPressed("down"))
        {
            PlayerInputBuffers["wall_jump_left"] = false;
            PlayerInputBuffers["wall_jump_right"] = false;
        }
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