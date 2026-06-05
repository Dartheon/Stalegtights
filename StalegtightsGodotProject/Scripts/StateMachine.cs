using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    #region DEBUG
    //DEBUG Variables Go Here...

    #endregion

    #region General
    //References to Other Components
    public CharacterBody2D smPlayerCB2D { get; private set; }
    public Player smPlayerScript { get; private set; }
    public InputManager smInputManager { get; private set; }

    //State Machine
    public States CurrentState { get; private set; }
    [Export] public NodePath InitialState { get; private set; }
    private Dictionary<string, States> states = new();
    public string SMPreviousState { get; set; } = "DEFAULT STATE";
    public int smWallDirection { get; set; }
    public bool smWallCancel { get; set; } = false;

    //Movement
    public float BaseGravity { get; private set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public float GravityModifier { get; set; } = 1.0f;
    public float smGravity => BaseGravity * GravityModifier;
    public float BaseAcceleration { get; set; } = 20.0f;
    public float BaseDeceleration { get; set; } = 10.0f;
    public float RunAccelerationModifier { get; set; } = 1.0f;
    public float RunDecelerationModifier { get; set; } = 1.0f;
    public float RunAcceleration => BaseAcceleration * RunAccelerationModifier;
    public float RunDeceleration => BaseDeceleration * RunDecelerationModifier;
    public float smLadderDetachTimer { get; set; } = 0f;

    public bool smTeleporting { get; set; }
    #endregion

    #region Animations
    public AnimationTree PlayerAnimTree { get; set; } //Used to access the different variables of the AnimationTree
    public AnimationPlayer PlayerCB2DAnimPlayer { get; set; }
    public StringName CurrentAnimationPlaying { get; set; } = "";
    public string PlayerState { get; private set; } = "DEFAULT STATE"; //Used for animation tree transitions between state machines
    public int LastFacingDirection { get; set; } = 1; //Identifies the Players last facing direction used for animation blend
    //public bool PlayerAnimIdle { get; set; } //Checks for player movement

    //TO DO: Could possibly move all Animation Branches to a Global Script to clean up the StateMachineScript if needed, Just need to change different States Script to point to the Global instead of here and point variables from here to the Global
    //All States
    public string CharacterStateBranch { get; set; } = "DEFAULT"; //Used for HasWeapon or HasStalag or HasNormal; To see if player is holding Weapon,Stalag or is Normal

    //AirState
    public string AirJumpBranch { get; set; } = "DEFAULT"; //Used for WallJumpOut, WallDiveOut, WallPowerSlideOut, GroundJump, LadderJump

    //GroundState
    public bool IsLandingBranch { get; set; } = false; //bool to see if player is landing from being in the air
    public string GroundMoveBranch { get; set; } = "DEFAULT"; //Used for GroundIdle, GroundRunning, GroundDucking, GroundCrawling, GroundRolling, GroundBreaking, GroundSliding, GroundPowerSlide

    //ClimbState
    //Might combine the two into one large string
    public string ToClimbBranch { get; set; } = "DEFAULT"; //Used for GroundToClimb, AirToClimb; To check whether climbing starts from ground or air
    public string LadderClimbBranch { get; set; } = "DEFAULT"; //Used for TopLadderUp, LadderSlideDown

    //WallState
    //Might combine the two into one large string
    public string WallActionBranch { get; set; } = "DEFAULT"; //Used for WallCling, WallSlideDown, WallStomp
    public string WallPowerSlideBranch { get; set; } = "DEFAULT"; //Used for WallPowerSlideUp, WallPowerSlideDown
    #endregion

    #region Movement
    public Vector2 smPlayerVelocity; //The Variable for storing and changing the Players Velocity
    [Export] public float BaseJumpVelocity { get; private set; } = -750.0f;
    public float JumpModifier { get; set; } = 1.0f;
    public float smPlayerJumpVelocity => BaseJumpVelocity * JumpModifier;
    public bool smDisableHorizontalInput { get; set; } = false;
    public bool smDisableVerticalInput { get; set; } = false;
    public bool smDisableUpInput { get; set; } = false;
    public bool smDisableDownInput { get; set; } = false;
    public bool smDisableRightInput { get; set; } = false;
    public bool smDisableLeftInput { get; set; } = false;
    #endregion

    #region Methods
    public override void _Ready()
    {
        //Initialization for all states
        smPlayerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");
        smPlayerScript = GetNode<Player>("/root/Main/World/Player");
        smInputManager = GetNode<InputManager>("/root/InputManager");

        PlayerAnimTree = GetNode<AnimationTree>("/root/Main/World/Player/PlayerAnimationTree");
        PlayerCB2DAnimPlayer = GetNode<AnimationPlayer>("/root/Main/World/Player/AnimationPlayer");

        CharacterStateBranch = "HasNormal"; //To be set somewhere else later, here for testing until other states are ready to use

        //Sets the State Nodes and Initializes them in order
        foreach (Node stateNode in GetChildren())
        {
            if (stateNode is States state)
            {
                states[stateNode.Name] = state;
                state.SetStateMachineScript(this);
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

        if (smLadderDetachTimer > 0)
        {
            smLadderDetachTimer -= (float)delta;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState.InputBuffer(delta);
        //GD.Print($"RawX: {smInputManager.RawInput.X:F3} | Horizontal: {smInputManager.HorizontalInput:F3} | Vertical: {smInputManager.VerticalInput:F3}");

        // Call current state for continuous input handling (first)
        CurrentState.HandleContinuousInput(delta);

        // Call current state for physics-based updates (after input handling)
        CurrentState.PhysicsUpdate(delta);

        bool wasOnFloor = smPlayerCB2D.IsOnFloor();
        smPlayerCB2D.Velocity = smPlayerVelocity;
        smPlayerCB2D.MoveAndSlide();

        if (!smPlayerCB2D.IsOnFloor() && wasOnFloor)
        {
            smInputManager.GroundJumpTimer.Start();
        }
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
        PlayerState = key; //For changing States in the AnimationTree
        CurrentState.Enter();
        SMPreviousState = key;
    }

    //signal method
    //doesn't show looping animations
    public void CurrentAnimationStartPlaying(StringName animName)
    {
        CurrentAnimationPlaying = animName;
    }

    // add different method for teleporters vs portals
    //debug ui to clear velocity and stuff while other interacting portals might keep velocity
    public async void ResetPlayerState()
    {
        smTeleporting = true;
        smWallCancel = true;

        smPlayerVelocity = Vector2.Zero;

        smWallDirection = 0;

        smInputManager.PlayerInputBuffers["wall_jump"] = false;
        smInputManager.PlayerInputBuffers["wall_jump_left"] = false;
        smInputManager.PlayerInputBuffers["wall_jump_right"] = false;

        //Clear Ladders
        Player.PlayerAboveLadder.Clear();
        GameManager.LaddersEntered.Clear();

        TransitionToState("AIR STATE");

        // Wait one physics frame for collisions to refresh
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        smTeleporting = false;

        // Prevent instant wall reattach after teleport
        await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);

        smWallCancel = false;
    }
    #endregion
}