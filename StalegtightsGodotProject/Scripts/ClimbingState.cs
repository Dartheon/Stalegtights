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
    private bool ladderJump = false;
    #endregion

    #region Movement
    private float climbSpeed = 500.0f;
    private float climbSnapWeight = 10.0f;
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
        ladderJump = false;
        //TO ADD:
        //if entering from air
        //else if entering from ground

        //slidingdown bool
        //climbing to ladder top - playeraboveladder, !playeronladder 
        #endregion

        #region Movement
        StateMachineScript.smPlayerVelocity = new(0, 0);

        #endregion
    }

    public override void Update(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animations
        //Blend Postions set to animplayer in statemachine
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
        //Jumping
        if (StateMachineScript.smInputManager.PlayerInputBuffers["ground_jump"])
        {
            //timer here as long as holding, reset on ground
            StateMachineScript.smPlayerVelocity.Y = PlayerJumpVelocity;
            PlayerScript.PlayerOnLadder = false;   // detach ladder
            StateMachineScript.smLadderDetachTimer = 1.0f;
            StateMachineScript.smInputManager.PlayerInputBuffers["ground_jump"] = false;

            //TO ADD:
            ladderJump = true;

            NewStateChange = AIRSTATESTRING;

            if (NewStateChange != CLIMBINGSTATESTRING)
            {
                StateMachineScript.TransitionToState(NewStateChange);
            }
        }
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
        if (StateMachineScript.smInputManager.PlayerContinuousInputs["climb_up"])
        {
            StateMachineScript.smPlayerVelocity.Y -= climbSpeed * (float)delta;
            PlayerCB2D.GlobalPosition = new(Mathf.Lerp(PlayerCB2D.GlobalPosition.X, PlayerScript.LadderPosX, climbSnapWeight * (float)delta), PlayerCB2D.GlobalPosition.Y);
        }
        else if (StateMachineScript.smInputManager.PlayerContinuousInputs["climb_down"])
        {
            StateMachineScript.smPlayerVelocity.Y += climbSpeed * (float)delta;
            PlayerCB2D.GlobalPosition = new(Mathf.Lerp(PlayerCB2D.GlobalPosition.X, PlayerScript.LadderPosX, climbSnapWeight * (float)delta), PlayerCB2D.GlobalPosition.Y);
        }
        else if (PlayerScript.PlayerOnLadder)
        {
            StateMachineScript.smPlayerVelocity.Y = 0.0f;
        }
        #endregion
    }

    public override void PhysicsUpdate(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement
        #region Change State before PhysicsUpdate
        if (NewStateChange != CLIMBINGSTATESTRING) { return; }
        #endregion

        if (!PlayerScript.PlayerOnLadder)
        {
            if (PlayerCB2D.IsOnFloor())
            {
                NewStateChange = GROUNDSTATESTRING;
            }
            else
            {
                NewStateChange = AIRSTATESTRING;
            }
        }

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
        //When at the bottom of a ladder and the down key is pressed, set the state to ground
        if (StateMachineScript.smInputManager.PlayerContinuousInputs["climb_down"] && PlayerCB2D.IsOnFloor())
        {
            NewStateChange = GROUNDSTATESTRING;
        }
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