using Godot;

public partial class AirState : States
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
    */

    /*Variables from StateMachine that need direct Referenece
    StateMachineScript.smPlayerVelocity     - Vector2
    */
    #endregion

    #region Animations
    //
    #endregion

    #region Movement
    [Export] private float inAirMoveSpeed = 300.0f; //speed of the character in the air
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
        GD.Print($"Entering {AIRSTATESTRING}      from {PreviousState}");

        #endregion

        #region General
        NewStateChange = AIRSTATESTRING;
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

        #region Animation
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
        #region Check for Climbing Input - add code
        //
        #endregion
        #region Detect to change to climbing state
        //
        #endregion

        #region Check for Character interacting with the wall
        //
        #endregion

        #region Check Input for Moving Right/Left
        if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
        {
            if (StateMachineScript.smPlayerVelocity.X > 0)
            {
                StateMachineScript.RunAcceleration = 10.0f;
                StateMachineScript.smPlayerVelocity.X = Mathf.Max(0, StateMachineScript.smPlayerVelocity.X - StateMachineScript.RunAcceleration);
            }

            else if (StateMachineScript.smPlayerVelocity.X < 0)
            {
                StateMachineScript.RunAcceleration = 10.0f;
                StateMachineScript.smPlayerVelocity.X = Mathf.Min(0, StateMachineScript.smPlayerVelocity.X + StateMachineScript.RunAcceleration);
            }
        }

        else if (Input.IsActionPressed("move_left"))
        {
            if (StateMachineScript.smPlayerVelocity.X >= -inAirMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 20.0f;
                StateMachineScript.smPlayerVelocity.X -= StateMachineScript.RunAcceleration;
            }
            else if (StateMachineScript.smPlayerVelocity.X < -inAirMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 0f;
            }
        }

        else if (Input.IsActionPressed("move_right"))
        {
            if (StateMachineScript.smPlayerVelocity.X <= inAirMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 20.0f;
                StateMachineScript.smPlayerVelocity.X += StateMachineScript.RunAcceleration;
            }
            else if (StateMachineScript.smPlayerVelocity.X > inAirMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 0f;
            }

        }

        else if (StateMachineScript.smPlayerVelocity.X > 0)
        {
            StateMachineScript.RunAcceleration = 20.0f;
            StateMachineScript.smPlayerVelocity.X = Mathf.Max(0, StateMachineScript.smPlayerVelocity.X - StateMachineScript.RunAcceleration);
        }

        else if (StateMachineScript.smPlayerVelocity.X < 0)
        {
            StateMachineScript.RunAcceleration = 20.0f;
            StateMachineScript.smPlayerVelocity.X = Mathf.Min(0, StateMachineScript.smPlayerVelocity.X + StateMachineScript.RunAcceleration);
        }
        #endregion
        #endregion
    }

    public override void PhysicsUpdate(double delta)
    {

        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement
        #region Apply Gravity on Character
        StateMachineScript.smPlayerVelocity.Y = Gravity; //When in the air the Y Velocity is equal to the Gravity Value 
        #endregion

        #region Landing - check to see if leaving air state or chaining to next jump from input during timing window - add code
        //
        #endregion

        #region Check for Knockback - add code
        //
        #endregion

        #region Check if the Character is Grounded
        if (PlayerCB2D.IsOnFloor())
        {
            NewStateChange = GROUNDSTATESTRING;
        }
        #endregion

        #region General
        #region Change State Logic
        if (NewStateChange != AIRSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
        #endregion
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
        #region Check for Air Dodge Input - add code
        //
        #endregion

        #region Check for Diving Input - add code
        //
        #endregion

        #region Check for Double Jump Input - add code
        //
        #endregion

        #region CHeck for Long Jump Input - add code
        //
        #endregion

        #region Check for Short Jupt Input - add code
        //
        #endregion

        #region Check for Running Jump Input - add code
        //
        #endregion

        #region Check for Jump Input - add code
        //
        #endregion

        #region Check for Attacking Input - add code
        //
        #endregion

        #region Change State Logic
        if (NewStateChange != AIRSTATESTRING)
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
        GD.Print($"Exiting  {AIRSTATESTRING}");
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