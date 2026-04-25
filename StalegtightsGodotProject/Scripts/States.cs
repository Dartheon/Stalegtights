using Godot;

public partial class States : Node
{
    //ALL Public Variables Initialized here are seen in ALL state scripts using States Class
    //Changes to a Public Variable in One Script WILL Change for ALL Scripts using States Class
    #region Variables
    #region DEBUG
    //DEBUG Variables Go Here...
    #endregion

    #region General
    protected StateMachine StateMachineScript { get; private set; }

    protected CharacterBody2D PlayerCB2D => StateMachineScript.smPlayerCB2D;
    protected Player PlayerScript => StateMachineScript.smPlayerScript;
    protected InputManager InputManager => StateMachineScript.smInputManager;

    protected string PreviousState => StateMachineScript.SMPreviousState;


    public const string AIRSTATESTRING = "AIR STATE";
    public const string GROUNDSTATESTRING = "GROUND STATE";
    public const string WALLSTATESTRING = "WALL STATE";
    public const string CLIMBINGSTATESTRING = "CLIMBING STATE";
    public const string DEATHSTATESTRING = "DEATH STATE";

    #endregion

    #region Animations
    public bool HasWeapon { get; protected set; } = false;
    public bool HasStalag { get; protected set; } = false;
    public bool IsLanding { get; protected set; } = false;
    #endregion

    #region Movement
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity => StateMachineScript.smGravity;
    public float PlayerJumpVelocity => StateMachineScript.smPlayerJumpVelocity;
    #endregion
    #endregion

    public virtual void Start() { }

    public virtual void Enter() { }

    public virtual void Update(double delta) { }

    public virtual void InputBuffer(double delta) { }

    //Use HandleContinuousInput for ongoing actions while keys are held down (e.g., walking, dashing).
    public virtual void HandleContinuousInput(double delta) { }

    //Use PhysicsUpdate for environment-based effects, such as gravity or natural deceleration.
    public virtual void PhysicsUpdate(double delta) { }

    //Use HandleInput(InputEvent @event) for discrete, one-time actions (e.g., jumping, attacking).
    public virtual void HandleInput(InputEvent @event) { }

    public virtual void Exit() { }

    public void SetStateMachineScript(StateMachine stateMachine)
    {
        StateMachineScript = stateMachine;
    }

    //Change State
    public void ChangeToNewState(string newState)
    {
        StateMachineScript.TransitionToState(newState);
    }
}