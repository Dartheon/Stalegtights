using Godot;
using System.Collections.Generic;

public partial class InputManager : Node
{
    #region Variables
    private StateMachine stateMachineScript;

    //These inputs are for one-time presses that need timers to reset the bool value
    public Dictionary<string, bool> PlayerInputBuffers { get; set; } = new()
    {
        //Player Inputs That Require a Timer to Reset
        { "jump", false},
    };

    //These inputs are for continuous key presses that the bool needs to be reset in the code
    public Dictionary<string, bool> PlayerContinuousInputs { get; set; } = new()
    {
        //Player Inputs That Get Reset In the Code
        { "move_right", false},
        { "move_left", false},
        { "interact", false},
        { "duck", false},
        { "crawling_left", false},
        { "crawling_right", false},
        //Debug Inputs
        { "engine_scale_up", false},
        { "engine_scale_down", false},
        { "engine_scale_reset", false},
    };

    private Timer jumpTimer;
    #endregion

    #region Ready
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        jumpTimer = GetNode<Timer>("JumpTimer");
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
        PlayerContinuousInputs["move_left"] = Input.IsActionPressed("move_left");
        PlayerContinuousInputs["move_right"] = Input.IsActionPressed("move_right");

        //Ducking
        PlayerContinuousInputs["duck"] = Input.IsActionPressed("duck");

        //Crawling
        PlayerContinuousInputs["crawling_left"] = Input.IsActionPressed("duck") && Input.IsActionPressed("move_left");
        PlayerContinuousInputs["crawling_right"] = Input.IsActionPressed("duck") && Input.IsActionPressed("move_right");
        #endregion
    }
    #endregion

    #region Unhandled Input
    public override void _UnhandledInput(InputEvent @event)
    {
        #region General
        //Player Interact
        PlayerContinuousInputs["interact"] = Input.IsActionJustPressed("interact") && (GameManager.InteractablesEntered?.Count > 0);
        #endregion

        #region Debug
        //Player Interact
        PlayerContinuousInputs["engine_scale_up"] = Input.IsActionJustPressed("engine_scale_up");
        //Player Interact
        PlayerContinuousInputs["engine_scale_down"] = Input.IsActionJustPressed("engine_scale_down");
        //Player Interact
        PlayerContinuousInputs["endgine_scale_reset"] = Input.IsActionJustPressed("engine_scale_reset");
        #endregion

        #region Movement
        //Player Jump
        if (Input.IsActionJustPressed("jump"))
        {
            PlayerInputBuffers["jump"] = true;
            jumpTimer.Start();
        }
        #endregion
    }
    #endregion

    #region Timer Signals
    //This is for all the Methods to deal with the Timers to reset the bools back to false
    public void OnJumpTimerTimeout()
    {
        PlayerInputBuffers["jump"] = false;
    }
    #endregion

}