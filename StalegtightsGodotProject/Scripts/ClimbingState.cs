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
    ToClimbBranch       - String
    LadderClimbBranch   - String
    */

    /*Variables from StateMachine that need direct Referenece
    stateMachine.smPlayerVelocity     - Vector2
    */
    #endregion

    #region Animations
    private float animDirectionY;
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
        //
        #endregion

        #region Animations
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
        //Set horizontal input as long as input is not 0
        if (InputManager.HorizontalInput != 0)
        {
            //Updates the LastFacingDirection based on Input
            StateMachineScript.LastFacingDirection = Mathf.Sign(InputManager.HorizontalInput);
        }

        animDirectionY = Mathf.Sign(StateMachineScript.smPlayerVelocity.Y);

        //Sets the blend Value in the AnimationTree
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/CLIMB STATE/CLIMB NORMAL/GROUNDTOCLIMB/blend_position.x", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/CLIMB STATE/CLIMB NORMAL/GROUNDTOCLIMB/blend_position.y", animDirectionY);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/CLIMB STATE/CLIMB NORMAL/AIRTOCLIMB/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/CLIMB STATE/CLIMB NORMAL/CLIMBING/blend_position", InputManager.HorizontalInput);
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
        if (InputManager.PlayerInputBuffers["ground_jump"])
        {
            //timer here as long as holding, reset on ground
            StateMachineScript.smPlayerVelocity.Y = PlayerJumpVelocity;
            PlayerScript.PlayerOnLadder = false;   // detach ladder
            StateMachineScript.smLadderDetachTimer = 1.0f;
            InputManager.PlayerInputBuffers["ground_jump"] = false;

            StateMachineScript.AirJumpBranch = "LadderJump";

            ChangeToNewState(AIRSTATESTRING);
            return;
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
        if (InputManager.PlayerContinuousInputs["climb_up"])
        {
            StateMachineScript.smPlayerVelocity.Y -= climbSpeed * (float)delta;
            PlayerCB2D.GlobalPosition = new(Mathf.Lerp(PlayerCB2D.GlobalPosition.X, PlayerScript.LadderPosX, climbSnapWeight * (float)delta), PlayerCB2D.GlobalPosition.Y);
        }
        else if (InputManager.PlayerContinuousInputs["climb_down"])
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
        if (!PlayerScript.PlayerOnLadder)
        {
            if (!StateMachineScript.smTeleporting && PlayerCB2D.IsOnFloor())
            {
                ChangeToNewState(GROUNDSTATESTRING);
                return;
            }
            else
            {
                ChangeToNewState(AIRSTATESTRING);
                return;
            }
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
        if (!StateMachineScript.smTeleporting && InputManager.PlayerContinuousInputs["climb_down"] && PlayerCB2D.IsOnFloor())
        {
            ChangeToNewState(GROUNDSTATESTRING);
            return;
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
        //
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
        #endregion
    }
}