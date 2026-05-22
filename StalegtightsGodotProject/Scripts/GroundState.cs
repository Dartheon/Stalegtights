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
    IsLandingBranch     - String
    */
    /*Variables from StateMachine that need direct Referenece
    StateMachineScript.smPlayerVelocity                          - Vector2
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
    private enum GroundMovementStates
    {
        Idle,
        Running,
        Ducking,
        Crawling,
        Braking,
        Sliding,
        Rolling,
        PowerSliding,
        DEFAULT
    }

    private GroundMovementStates currentMovementState = GroundMovementStates.DEFAULT;

    [Export] public float BaseSpeed { get; private set; } = 500.0f; //characters speed
    public float MoveSpeedModifier { get; set; } = 1.0f;
    public float GroundMoveSpeed => BaseSpeed * MoveSpeedModifier;

    public enum GroundJumpState
    {
        RegularJump,
        RunningJump,
        HighJump,
        LongJump,
        ShortHop,
        DEFAULT

    }
    public GroundJumpState JumpState = GroundJumpState.DEFAULT;
    #endregion
    #endregion

    #region Start
    public override void Start()
    {
        #region DEBUG
        //DEBUG Variables Go Here...
        #endregion

        #region General
        currentMovementState = GroundMovementStates.Idle;
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
        //StateMachineScript.PlayerAnimIdle = StateMachineScript.smPlayerVelocity.X == 0.0f ? true : false;
        if (Mathf.Abs(StateMachineScript.smPlayerVelocity.X) < 0.01f && !InputManager.LeftIntent && !InputManager.RightIntent && !InputManager.UpIntent && !InputManager.DownIntent)
        {
            currentMovementState = GroundMovementStates.Idle;
        }

        //Set horizontal input as long as input is not 0
        if (InputManager.HorizontalInput != 0)
        {
            AnimHorizontal = InputManager.HorizontalInput;

            //Updates the LastFacingDirection based on Input
            StateMachineScript.LastFacingDirection = Mathf.Sign(InputManager.HorizontalInput);
        }

        //Sets the blend Value in the AnimationTree
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDIDLE/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDRUN/blend_position", AnimHorizontal);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDDUCKING/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDCRAWLING/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDBRAKING/blend_position", AnimHorizontal);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDROLLING/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDSLIDING/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/GROUNDPOWERSLIDE/blend_position", StateMachineScript.LastFacingDirection);
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/LANDING/blend_position", StateMachineScript.LastFacingDirection);
        #endregion
    }
    #endregion

    #region InputBuffer
    //Detect Jumps
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
            //TO DO: use jump type enum for airjumpbranch and setting velocity.Y
            //TO DO: timer here as long as holding, reset on ground
            InputManager.PlayerInputBuffers["ground_jump"] = false;
            StateMachineScript.smPlayerVelocity.Y = PlayerJumpVelocity;

            StateMachineScript.AirJumpBranch = "GroundRunningJump";

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
     CIPU* detect input for braking
	 CIPU* detect input for power sliding
	 CIPU* detect input for sliding
	 CIPU* detect input for rolling
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
        #region Detect Input for Moving Right or Left
        if (InputManager.HorizontalInput != 0 && !InputManager.DownIntent && !InputManager.UpIntent)
        {
            if (currentMovementState != GroundMovementStates.Ducking && currentMovementState != GroundMovementStates.PowerSliding)
            {
                currentMovementState = GroundMovementStates.Running;
            }
        }
        else
        {
            if (currentMovementState == GroundMovementStates.Running)
            {
                StateMachineScript.BaseAcceleration = 20.0f;

                StateMachineScript.smPlayerVelocity.X = Mathf.MoveToward(StateMachineScript.smPlayerVelocity.X, 0, StateMachineScript.RunAcceleration);
                //Switch to Crawling from Running
                if ((InputManager.DownIntent && InputManager.RightIntent) || (InputManager.DownIntent && InputManager.LeftIntent))
                {
                    currentMovementState = GroundMovementStates.Crawling;
                }
                //Switch to Ducking from Running
                else if (InputManager.DownIntent && !InputManager.RightIntent && !InputManager.LeftIntent)
                {
                    currentMovementState = GroundMovementStates.Ducking;
                }
            }
        }
        #endregion

        GD.Print(currentMovementState, StateMachineScript.GroundMoveBranch);

        //Switch for actioning what the Player is doing while on the ground
        switch (currentMovementState)
        {
            case GroundMovementStates.Ducking:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundDucking";

                //Movement
                StateMachineScript.smPlayerVelocity.X = 0;
                //TO DO: add camera movement change to falling state after timer ends
                //Switch to Crawling from Ducking
                if (InputManager.PlayerContinuousInputs["crawling_left"] || InputManager.PlayerContinuousInputs["crawling_right"])
                {
                    currentMovementState = GroundMovementStates.Crawling;
                }
                //Jump out of Duck to High Jump 
                else if (InputManager.PlayerInputBuffers["ground_jump"])
                {
                    //TO DO: add High Jump logic outside of switch
                }
                break;

            case GroundMovementStates.Crawling:
                if (InputManager.PlayerContinuousInputs["crawling_left"] || InputManager.PlayerContinuousInputs["crawling_right"])
                {
                    //Animation
                    StateMachineScript.GroundMoveBranch = "GroundCrawling";
                    //Movement
                    //TO DO: add crawling movement that is slower that walking speed

                    //Switch to Rolling from Crawling
                    if (InputManager.PlayerInputBuffers["roll"])
                    {
                        currentMovementState = GroundMovementStates.Rolling;
                    }
                    //Jump out of Crawl to High Jump
                    else if (InputManager.PlayerInputBuffers["ground_jump"])
                    {
                        //TO DO: add High Jump logic outside of switch
                    }
                }
                //Switch to Ducking from Crawling
                else if (InputManager.PlayerContinuousInputs["duck"])
                {
                    currentMovementState = GroundMovementStates.Ducking;
                }
                //Switch back to Ducking from Crawling
                else
                {
                    currentMovementState = GroundMovementStates.Ducking;
                }
                break;

            case GroundMovementStates.Running:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundRunning";

                //Movement
                if (Mathf.Sign(InputManager.HorizontalInput) != Mathf.Sign(StateMachineScript.smPlayerVelocity.X) && StateMachineScript.smPlayerVelocity.X != 0)
                {
                    StateMachineScript.BaseAcceleration = 200.0f;
                }
                else
                {
                    StateMachineScript.BaseAcceleration = 20.0f;
                }

                StateMachineScript.smPlayerVelocity.X = Mathf.MoveToward(StateMachineScript.smPlayerVelocity.X, InputManager.HorizontalInput * GroundMoveSpeed, StateMachineScript.RunAcceleration);
                //StateMachineScript.smPlayerVelocity.X += InputManager.HorizontalInput * StateMachineScript.RunAcceleration;
                //StateMachineScript.smPlayerVelocity.X = Mathf.Clamp(StateMachineScript.smPlayerVelocity.X, -GroundMoveSpeed * -InputManager.HorizontalInput, GroundMoveSpeed * InputManager.HorizontalInput);

                //TO DO: add logic for braking

                //Switch to Sliding from Running
                if (InputManager.PlayerContinuousInputs["slide"])
                {
                    currentMovementState = GroundMovementStates.Sliding;
                }

                //TO DO: add logic for running jump outside of switch
                //TO DO: add logic for short hop jump outside of switch

                //Move to Crawling from Running
                if (InputManager.PlayerContinuousInputs["crawling_left"] || InputManager.PlayerContinuousInputs["crawling_right"])
                {
                    currentMovementState = GroundMovementStates.Crawling;
                }
                else if (InputManager.UpIntent && !InputManager.RightIntent && !InputManager.LeftIntent)
                {
                    currentMovementState = GroundMovementStates.Idle;
                }
                break;

            case GroundMovementStates.Braking:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundBraking";

                //Movement

                break;

            case GroundMovementStates.PowerSliding:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundPowerSlide";

                //Movement

                break;

            case GroundMovementStates.Sliding:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundSliding";

                //Movement
                if (InputManager.PlayerContinuousInputs["slide"])
                {
                    //TO DO: add slide enum for movement

                    //Switch to Rolling from Sliding
                    if (InputManager.PlayerInputBuffers["roll"])
                    {
                        currentMovementState = GroundMovementStates.Rolling;
                    }
                    //Switch to PowerSliding from Sliding
                    else if (InputManager.PlayerContinuousInputs["power_slide"])
                    {
                        currentMovementState = GroundMovementStates.PowerSliding;
                    }
                }
                break;

            case GroundMovementStates.Rolling:
                //Input Cleanup
                InputManager.PlayerInputBuffers["roll"] = false;

                //Animation
                StateMachineScript.GroundMoveBranch = "GroundRolling";

                //Movement
                //Switch to sliding from Rolling
                if (InputManager.PlayerContinuousInputs["slide"])
                {
                    currentMovementState = GroundMovementStates.Sliding;
                }
                //Switch to Crawling from Rolling
                if (InputManager.PlayerContinuousInputs["crawling_left"] || InputManager.PlayerContinuousInputs["crawling_right"])
                {
                    currentMovementState = GroundMovementStates.Crawling;
                }
                break;

            case GroundMovementStates.Idle:
                //Animation
                StateMachineScript.GroundMoveBranch = "GroundIdle";

                //Movement
                StateMachineScript.smPlayerVelocity.X = 0;
                //TO DO: add jumping for regular and short hop

                //Switch from Idle to Ducking
                if (InputManager.PlayerContinuousInputs["duck"])
                {
                    currentMovementState = GroundMovementStates.Ducking;
                }
                break;

            default:
                GD.PushWarning($"No Acceptable GroundState entered in Movement Enum: {currentMovementState}");
                break;
        }

        #region Check if Character is Interacting with a Climbable Surface
        if (!StateMachineScript.smTeleporting && InputManager.PlayerContinuousInputs["climb_enter_ladder"])
        {
            StateMachineScript.ToClimbBranch = "GroundToClimb";

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

                StateMachineScript.ToClimbBranch = "GroundToClimb";

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
     HIPU* detect input for stalag system
	 HIPU* detect input for stomping
	 HIPU* detect input for attacking
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
        StateMachineScript.IsLandingBranch = false;
        #endregion

        #region Movement
        //
        #endregion
    }
    #endregion
}