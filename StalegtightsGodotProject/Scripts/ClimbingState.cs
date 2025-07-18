using Godot;

public partial class ClimbingState : States
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
    */
    #endregion

    #region Animations
    //
    #endregion

    #region Movement
    //
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
        GD.Print($"Entering {CLIMBINGSTATESTRING} from {PreviousState}");
        #endregion

        #region General
        NewStateChange = CLIMBINGSTATESTRING;
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
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
        //
        #endregion
    }

    public override void PhysicsUpdate(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement

        if (NewStateChange != CLIMBINGSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
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
        //
        #endregion
    }

    public override void Exit()
    {
        //clean up this state when moving out of it
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Exiting  {CLIMBINGSTATESTRING}");
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