using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    #region DEBUG
    //DEBUG Variables Go Here...
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
    public float BaseGravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public float GravityModifier { get; set; } = 1.0f;
    public float smGravity => BaseGravity * GravityModifier;
    public float BaseAcceleration { get; set; } = 20.0f;
    public float RunAccelerationModifier { get; set; } = 1.0f;
    public float RunAcceleration => BaseAcceleration * RunAccelerationModifier;

    #endregion

    #region Animations
    public AnimationTree PlayerAnimTree { get; set; } //Used to access the different variables of the AnimationTree
    public AnimationPlayer PlayerCB2DAnimPlayer { get; set; }
    public StringName CurrentAnimationPlaying { get; set; } = "";
    private string playerState = "DEFAULT STATE"; //Used for animation tree transitions between state machines
    public float LastFacingDirection { get; set; } = 1.0f; //Identifies the Players last facing direction used for animation blend
    public bool PlayerAnimIdle { get; set; } //Checks for player movement
    public bool hasWeapon = false; //bool to see if player is holding weapon
    public bool hasStalag = false; //bool to see if player is holding stalag
    public bool isLanding = false; //bool to see if player is jumping
    #endregion

    #region Movement
    public Vector2 smPlayerVelocity; //The Variable for storing and changing the Players Velocity
    [Export] public float BaseJumpVelocity { get; set; } = -500.0f;
    public float JumpModifier { get; set; } = 1.0f;
    public float smPlayerJumpVelocity => BaseJumpVelocity * JumpModifier;
    #endregion

    #region Methods
    public override void _Ready()
    {
        //Initialization for all states
        smPlayerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");
        smPlayerScript = GetNode<Player>("/root/Main/World/Player");

        PlayerAnimTree = GetNode<AnimationTree>("/root/Main/World/Player/PlayerAnimationTree");
        PlayerCB2DAnimPlayer = GetNode<AnimationPlayer>("/root/Main/World/Player/AnimationPlayer");

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
    }

    public override void _PhysicsProcess(double delta)
    {
        // Call current state for continuous input handling (first)
        CurrentState.HandleContinuousInput(delta);

        // Call current state for physics-based updates (after input handling)
        CurrentState.PhysicsUpdate(delta);

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
        playerState = key; //For changing States in the AnimationTree
        CurrentState.Enter();
        SMPreviousState = key;
    }

    public void CurrentAnimationStartPlaying(StringName animName)
    {
        CurrentAnimationPlaying = animName;
    }
    #endregion

}
