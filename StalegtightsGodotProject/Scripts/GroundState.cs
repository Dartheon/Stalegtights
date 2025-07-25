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
    StateMachineScript.smPlayerVelocity     - Vector2
    StateMachineScript.hasWeapon            - bool
    StateMachineScript.hasStalag            - bool
    StateMachineScript.LastFacingDirection  - float
    StateMachineScript.PlayerAnimIdle       - bool
    StateMachineScript.PlayerAnimTree       - AnimationTree
    StateMachineScript.isJumping            - bool
    */
    #endregion

    #region Animation
    //
    private float runBlend;
    #endregion

    #region Movement
    [Export] private float groundMoveSpeed = 500.0f; //characters speed
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
        NewStateChange = GROUNDSTATESTRING;
        #endregion

        #region Animations
        //Used for testing, will be handled different later
        StateMachineScript.hasStalag = HasStalag;
        StateMachineScript.hasWeapon = HasWeapon;
        IsLanding = false;
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

        //Normalizes the Velocity before setting the BlendSpace value to it
        runBlend = Mathf.Clamp(StateMachineScript.smPlayerVelocity.X / groundMoveSpeed, -1f, 1f);
        //Sets the blend Value in the AnimationTree
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/RUN/blend_position", runBlend);

        //Updates the LastFacingDirection based on velocity
        if (Mathf.Abs(StateMachineScript.smPlayerVelocity.X) > 0.1f) // If moving, update facing
        {
            StateMachineScript.LastFacingDirection = StateMachineScript.smPlayerVelocity.X > 0 ? 1f : -1f;
        }

        //Sets the blend using te LastFacingPosition
        StateMachineScript.PlayerAnimTree.Set("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/IDLE/blend_position", StateMachineScript.LastFacingDirection);
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
        if (Input.IsActionPressed("duck"))
        {
            //add code for ducking
            GD.Print("Ducking");
        }
        #endregion

        #region Detect Input for Crawling - need to add code and turn off regular ducking input if crawling is true. maybe bool?
        if (Input.IsActionPressed("duck") && Input.IsActionPressed("move_left") || Input.IsActionPressed("duck") && Input.IsActionPressed("move_right"))
        {
            //add code for crawling
            GD.Print("Crawling");
        }
        #endregion

        #region Detect Input for Moving Right or Left
        if (Input.IsActionPressed("move_left"))
        {
            if (StateMachineScript.smPlayerVelocity.X > 0)
            {
                StateMachineScript.RunAcceleration = 200.0f;
                StateMachineScript.smPlayerVelocity.X -= StateMachineScript.RunAcceleration;
            }

            if (StateMachineScript.smPlayerVelocity.X >= -groundMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 20.0f;
                StateMachineScript.smPlayerVelocity.X -= StateMachineScript.RunAcceleration;
            }
            else if (StateMachineScript.smPlayerVelocity.X < -groundMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 0f;
            }

            if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right"))
            {
                StateMachineScript.smPlayerVelocity.X = 0;
            }
        }

        else if (Input.IsActionPressed("move_right"))
        {
            if (StateMachineScript.smPlayerVelocity.X < 0)
            {
                StateMachineScript.RunAcceleration = 200.0f;
                StateMachineScript.smPlayerVelocity.X += StateMachineScript.RunAcceleration;
            }
            if (StateMachineScript.smPlayerVelocity.X <= groundMoveSpeed)
            {
                StateMachineScript.RunAcceleration = 20.0f;
                StateMachineScript.smPlayerVelocity.X += StateMachineScript.RunAcceleration;
            }
            else if (StateMachineScript.smPlayerVelocity.X > groundMoveSpeed)
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

        #region General
        #region Change State Logic
        if (NewStateChange != GROUNDSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
        #endregion
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

        #region Check if Character is Interacting with a Climbable Surface - need to add code
        //
        #endregion

        #region Check if Character is Interacting with a Wall - need to add code
        //
        #endregion

        #region Check if Character is on the Ground
        if (!PlayerCB2D.IsOnFloor())
        {
            NewStateChange = AIRSTATESTRING;
        }
        else
        {
            //sets the velocity to 0 if the player is not touching the ground
            StateMachineScript.smPlayerVelocity.Y = 0.0f;
        }
        #endregion

        #region Change State Logic
        if (NewStateChange != GROUNDSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
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
     HIPU* detect input for climbing
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
        #region Detect any Jump and keep track of the type of jump used
        if (Input.IsActionJustPressed("jump"))
        {
            StateMachineScript.smPlayerVelocity.Y = PlayerJumpVelocity;
            NewStateChange = AIRSTATESTRING;
        }
        #endregion

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

        #region Check for Climbing Input
        //
        #endregion

        #region Check for Wall Jumping
        //
        #endregion

        #region General
        #region Change State Logic
        if (NewStateChange != GROUNDSTATESTRING)
        {
            StateMachineScript.TransitionToState(NewStateChange);
        }
        #endregion
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
        NewStateChange = "";
        #endregion

        #region Animations
        //
        #endregion

        #region Movement
        //
        #endregion
    }
    #endregion
}