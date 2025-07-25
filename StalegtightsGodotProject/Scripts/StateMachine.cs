using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    #region DEBUG
    //DEBUG Variables Go Here...
    //
    #endregion

    #region General
    //References to Other Components
    public CharacterBody2D smPlayerCB2D { get; set; }
    public Player smPlayerScript { get; set; }

    //State Machine
    public States CurrentState { get; private set; }
    [Export] public NodePath InitialState { get; set; }
    private Dictionary<string, States> states = new();
    public string SMPreviousState { get; set; } = "DEFAULT STATE";

    //Movement
    public float smGravity { get; set; }
    public float RunAcceleration { get; set; } = 20.0f;

    #endregion

    #region Animations
    private AnimationTree playerAnimTree;
    private AnimationNodeStateMachinePlayback playback;
    private string playerState = "DEFAULT STATE";
    private float lastFacingDirection = 1.0f;
    public bool PlayerAnimIdle { get; set; }
    public bool hasWeapon;
    public bool hasStalag;
    #endregion

    #region Movement
    public Vector2 smPlayerVelocity;
    [Export] public float smPlayerJumpVelocity = -500.0f;
    #endregion

    #region Methods
    public override void _Ready()
    {
        //Initialization for all states
        smPlayerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");
        smPlayerScript = GetNode<Player>("/root/Main/World/Player");

        smGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        playerAnimTree = GetNode<AnimationTree>("/root/Main/World/Player/PlayerAnimationTree");
        playback = (AnimationNodeStateMachinePlayback)playerAnimTree.Get("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/playback");

        //Sets the State Nodes and Initializes them in order
        foreach (Node stateNode in GetChildren())
        {
            if (stateNode is States state)
            {
                states[stateNode.Name] = state;
                state.StateMachineScript = this;
                state.Start();
                state.Exit(); //reset all states
            }
        }

        CurrentState = GetNode<States>(InitialState);
        CurrentState.Enter();
        SMPreviousState = InitialState;
    }

    public override void _Process(double delta)
    {
        CurrentState.Update(delta);

        GD.Print(playback.GetCurrentNode());
        PlayerAnimIdle = smPlayerVelocity.X == 0.0f ? true : false;
        GD.Print(PlayerAnimIdle);

        if (Mathf.Abs(smPlayerVelocity.X) > 0.1f) // If moving, update facing
        {
            lastFacingDirection = smPlayerVelocity.X > 0 ? 1f : -1f;
            GD.Print(lastFacingDirection);
        }

        playerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/IDLE/blend_position", lastFacingDirection);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Call current state for continuous input handling (first)
        CurrentState.HandleContinuousInput(delta);

        // Call current state for physics-based updates (after input handling)
        CurrentState.PhysicsUpdate(delta);

        float runBlend = Mathf.Clamp(smPlayerVelocity.X / 500.0f, -1f, 1f);
        GD.Print(runBlend);
        playerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/RUN/blend_position", runBlend);

        smPlayerCB2D.Velocity = smPlayerVelocity;
        smPlayerCB2D.MoveAndSlide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CurrentState.HandleInput(@event);
    }

    public void TransitionToState(string key)
    {
        States newState = states[key];

        if (!states.ContainsKey(key) || CurrentState == newState) { return; }

        CurrentState.Exit();
        CurrentState = newState;
        playerState = key; //For changing Animations in the Tree
        CurrentState.Enter();
        SMPreviousState = key;
    }
    #endregion

}
