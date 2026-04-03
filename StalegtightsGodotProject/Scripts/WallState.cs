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
    private float currentDirection;
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
        #region Check to see if still on wall - if character collider is still interacting with the wall
        if (!StateMachineScript.smInputManager.PlayerContinuousInputs["move_left"] && !StateMachineScript.smInputManager.PlayerContinuousInputs["move_right"])
        {
            //start delay timer
            //change state to air
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

        #region Touching the Wall
        currentDirection = StateMachineScript.smInputManager.PlayerContinuousInputs["move_left"] ? -1 : 0;
        currentDirection = StateMachineScript.smInputManager.PlayerContinuousInputs["move_right"] ? 1 : 0;
        #endregion

        #region Touching the Ground
        if (PlayerCB2D.IsOnFloor())
        {
            NewStateChange = GROUNDSTATESTRING;
        }
        #endregion

        #region Process Sliding Down the Wall
        //move player down
        //duration timer counting down
        //when timer hits 0 move player down faster expontentally over time till max speed
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
        #region Detect Jumping Off
        //set vel to 0 before jumping
        //check duration timer, if timer not 0 use standard jump, if 0 use lesser jump
        //check direction either up or out from wall + jump to jump either straight up or out <_ direction held might not matter when jumping up or out
        #endregion

        #region Detect Diving Out - add code
        //set vel to 0 before jumping
        //apply horizontal jump if direction held into the wall and down
        #endregion

        #region Detect if Character is Dropping Down - add code
        //if direction out of wall and down enter air state
        if (enteringDirection != currentDirection && StateMachineScript.smInputManager.PlayerContinuousInputs["wall_drop_down"])
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
        //
        #endregion
    }
}