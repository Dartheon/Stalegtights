using System.Linq;
using Godot;

public partial class GroundState : States
{
    #region Variables
    #region DEBUG
    //DEBUG Variables Go Here...
    #endregion

    #region General
    /*References from StateMachine
    PlayerCB2D          - CharacterBody2D
    PlayerScript        - Player
    Gravity             - Float
    */
    /*Variables from StateMachine that need direct Referenece
    StateMachineScript.smPlayerVelocity                          - Vector2
    StateMachineScript.hasWeapon                                 - bool
    StateMachineScript.hasStalag                                 - bool
    StateMachineScript.LastFacingDirection                       - int
    StateMachineScript.PlayerAnimIdle                            - bool
    StateMachineScript.PlayerAnimTree                            - AnimationTree
    StateMachineScript.smInputManager.PlayerInputBuffers[string] - bool
    */
    #endregion

    #region Animation
    public float AnimHorizontal { get; private set; }
    private float runBlend; //Used for storing a variable to use in the AnimationTree BlendSpace1D for running
    #endregion

    #region Movement
    [Export] public float BaseSpeed { get; private set; } = 500.0f; //characters speed
    public float MoveSpeedModifier { get; set; } = 1.0f;
    public float GroundMoveSpeed => BaseSpeed * MoveSpeedModifier;
    #endregion
    #endregion

    #region Start
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
    #endregion

    #region Enter
    public override void Enter()
    {
        //set up stuff when entering this state
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Entering {GROUNDSTATESTRING}   from {PreviousState}");
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
    #endregion

    #region Update
    /*Update Tasks
     U* Animations
    */
    public override void Update(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animations
        //Sets the Idle bool to true or false
        StateMachineScript.PlayerAnimIdle = StateMachineScript.smPlayerVelocity.X == 0.0f ? true : false;

        //Set horizontal input as long as input is not 0
        if (InputManager.HorizontalInput != 0)
        {
            AnimHorizontal = InputManager.HorizontalInput;

            //Updates the LastFacingDirection based on Input
            StateMachineScript.LastFacingDirection = Mathf.Sign(InputManager.HorizontalInput);
        }

        //Sets the blend Value in the AnimationTree
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/RUN/blend_position", AnimHorizontal);

        // Apply to animations
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/IDLE/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/LANDING/blend_position", StateMachineScript.LastFacingDirection);
        #endregion
    }
    #endregion

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
        #region Detect any Jump and keep track of the type of jump used
        if (InputManager.PlayerInputBuffers["ground_jump"])
        {
            //timer here as long as holding, reset on ground
            InputManager.PlayerInputBuffers["ground_jump"] = false;
            StateMachineScript.smPlayerVelocity.Y = PlayerJumpVelocity;

            StateMachineScript.WallDiveOut = false;
            StateMachineScript.WallJumpOut = false;
            StateMachineScript.WallPowerSlideOut = false;
            StateMachineScript.LadderJump = false;

            ChangeToNewState(AIRSTATESTRING);
            return;
        }
        #endregion
        #endregion
    }
    #endregion

    #region HandleContinuousInput
    /*HandleContinuousinput Tasks
     CIPU* detect input for ducking

     CIPU* detect input for crawling

     CIPU* detect input for moving right or left
     */
    public override void HandleContinuousInput(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        #region Detect Input for Ducking - need to add code
        if (InputManager.PlayerContinuousInputs["duck"])
        {
            //add code for ducking
            GD.Print("Ducking");
        }
        #endregion

        #region Detect Input for Crawling - need to add code
        if (InputManager.PlayerContinuousInputs["crawling_left"] || InputManager.PlayerContinuousInputs["crawling_right"])
        {
            //add code for crawling
            GD.Print("Crawling");
        }
        #endregion

        #region Detect Input for Moving Right or Left
        if (InputManager.HorizontalInput != 0)
        {
            if (Mathf.Sign(InputManager.HorizontalInput) != Mathf.Sign(StateMachineScript.smPlayerVelocity.X) && StateMachineScript.smPlayerVelocity.X != 0)
            {
                StateMachineScript.BaseAcceleration = 200.0f;
            }
            else
            {
                StateMachineScript.BaseAcceleration = 20.0f;
            }

            StateMachineScript.smPlayerVelocity.X += InputManager.HorizontalInput * StateMachineScript.RunAcceleration;

            StateMachineScript.smPlayerVelocity.X = Mathf.Clamp(StateMachineScript.smPlayerVelocity.X, -GroundMoveSpeed, GroundMoveSpeed);
        }
        else
        {
            StateMachineScript.BaseAcceleration = 20.0f;

            StateMachineScript.smPlayerVelocity.X = Mathf.MoveToward(StateMachineScript.smPlayerVelocity.X, 0, StateMachineScript.RunAcceleration);
        }
        #endregion

        #region Check if Character is Interacting with a Climbable Surface
        if (!StateMachineScript.smTeleporting && InputManager.PlayerContinuousInputs["climb_enter_ladder"])
        {
            StateMachineScript.GroundToClimb = true;
            StateMachineScript.AirToClimb = false;

            ChangeToNewState(CLIMBINGSTATESTRING);
            return;
        }

        if (!StateMachineScript.smTeleporting && InputManager.PlayerContinuousInputs["climb_down_ladder_top"])
        {
            CollisionShape2D shape2D = Player.PlayerAboveLadder.FirstOrDefault<CollisionShape2D>();

            if (shape2D != null)
            {
                PlayerScript.PlayerOnLadder = true;
                shape2D.GetParent().GetParent().CallDeferred("OnWayDisableLadderTop", shape2D);

                StateMachineScript.GroundToClimb = true;
                StateMachineScript.AirToClimb = false;

                ChangeToNewState(CLIMBINGSTATESTRING);
                return;
            }
        }
        #endregion
        #endregion

        #region General
        //
        #endregion
    }
    #endregion

    #region PhysicsUpdate
    /*Physics Update Tasks
     PUPU* check and apply for stun
     PUPU* check and apply for knockback
     PUPU* check if character is interacting with climbing surface
     PUPU* check if character is interacting with the wall
     PUPU* if character is on the ground
    */
    public override void PhysicsUpdate(double delta)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Movement
        #region Check for Stun - need to add code
        //
        #endregion

        #region Check for Knockback- need to add code
        //
        #endregion

        #region Check if Character is Interacting with a Wall
        if (PlayerCB2D.IsOnWall())
        {
            Vector2 wallNormal = PlayerCB2D.GetWallNormal();

            if (wallNormal.X > 0)
            {
                StateMachineScript.smWallDirection = -1;
            }
            else if (wallNormal.X < 0)
            {
                StateMachineScript.smWallDirection = 1;
            }
        }
        else
        {
            StateMachineScript.smWallDirection = 0;
        }
        #endregion

        #region Check if Character is on the Ground
        if (!PlayerCB2D.IsOnFloor() && ((Player.PlayerAboveLadder.Count == 0) || !InputManager.PlayerInputBuffers["ground_jump"]))
        {
            ChangeToNewState(AIRSTATESTRING);
            return;
        }
        else
        {
            //sets the velocity to 0 if the player is touching the ground
            StateMachineScript.smPlayerVelocity.Y = 0.0f;
        }
        #endregion
        #endregion
    }
    #endregion

    #region HandleInput
    /*HandleInput Tasks
     HIPU* detect any jump
     HIPU* detect input for braking
	 HIPU* detect input for power sliding
	 HIPU* detect input for sliding
	 HIPU* detect input for rolling
     HIPU* detect input for stalag system
	 HIPU* detect input for stomping
	 HIPU* detect input for attacking
     HIPU* detect input for walljumping
    */
    public override void HandleInput(InputEvent @event)
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        #region Check for Braking input - add code
        //
        #endregion

        #region Check for Power Slide Input - add code
        //
        #endregion

        #region Check for Slide Input - add code
        //
        #endregion

        #region Check for rolling Input - add code
        //
        #endregion

        #region Check for stalag Input - add code
        //
        #endregion

        #region Check for Stomping Input - add code
        //
        #endregion

        #region Check for Attacking Input - add code
        //
        #endregion

        #region Check for Wall Jumping
        //
        #endregion

        #region General
        //
        #endregion
        #endregion
    }
    #endregion

    #region Exit
    public override void Exit()
    {
        //clean up this state when moving out of it
        #region DEBUG
        //DEBUG Variables Go Here...
        GD.Print($"Exiting  {GROUNDSTATESTRING}");
        #endregion

        #region General
        //
        #endregion

        #region Animations
        StateMachineScript.IsLanding = false;
        #endregion

        #region Movement
        //
        #endregion
    }
    #endregion
}