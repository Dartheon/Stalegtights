using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public partial class PlayerCamera : Camera2D
{
    //bottom margin .45 = 64px
    //offsetY -150 = 64px  (default rest) 
    #region Variables
    #region Class Variables
    private StateMachine stateMachineScript;
    private GroundState groundStateScript;
    private Player playerCB2D;
    private AirState airState;
    private InputManager inputManager;
    #endregion

    #region Camera Limits
    public int PlayerCameraLimitTop { get; set; }
    public int PlayerCameraLimitBottom { get; set; }
    public int PlayerCameraLimitLeft { get; set; }
    public int PlayerCameraLimitRight { get; set; }
    #endregion

    #region  Camera Positioning
    public Vector2 PlayerCameraPosition { get; set; } //holds the camera position
    #endregion

    #region  Player Movement
    private float maxPlayerSpeed; //hold the players max speed to use for figuring out the threshold
    #endregion

    #region Zoom Properties
    // Camera Zoom
    private Vector2 defaultZoom;
    [Export] public Vector2 ZoomOutMax { get; set; } = new(1.0f, 1.0f); // how far camera can zoom out

    //Zoom Properties
    [Export] public float ZoomOutDuration { get; set; } = 1.5f; // seconds to fully zoom out
    [Export] public float ZoomInDuration { get; set; } = 1.5f;  // seconds to fully zoom in
    [Export] public float MoveDelayTimer { get; set; } = 3.0f; // time player must move before zooming out
    [Export] public float SpeedPercentageThreshold { get; set; } = 0.9f; //the percentage used for the threshold
    private Timer zoomInTimer; //hold the Timer child to reference

    /*****UPDATE LATER*******
    Set threshold to a hardcoded value so the behaviour of the camera is consistent at low speeds even as player increases max speed*/
    private float threshold; //used to determine the percentage of the max speed before starting the zoom //
    private float moveTimer = 0f;
    private bool isZoomedOut = false;
    #endregion

    #region Drag Properties
    //Drag Properties General
    private Timer cameraRecenterTimer;
    private bool returningToCenter = false; //checks if camera is moving back to center and pauses the timer
    private bool startTimer = false; //if bool to stop repeated signal sending

    //Drag Properties X
    private Tween xTween;
    private const float HorizontalOffsetTweenTime = 2.0f; // how fast the offset happens in the applly offset tween method
    [Export] public float HorizontalOffsetStationaryTimer { get; set; } = 10.0f;
    [Export] public float HorizontalOffsetUnderThresholdTimer { get; set; } = 20.0f;
    [Export] public float OffsetDistanceX { get; set; } = 1.0f; //how far the offset will go. 1 Unit of Offset is equal to 1/10th of ScreenWidth. (5 will put the player at the edge of the sceen at Zoom = 1. At Zoom = 0.5, 10 will put the player at the edge of the screen.)
    private float xTargetOffset;
    private float xNewTargetOffset;
    private bool xReachedMaxOffset = false;

    //Drag Properties Y
    private Tween yTween;

    private const float VerticalResetTweenTime = 1.0f;
    private float defaultOffsetY = 60f;

    private const float FallingOffsetTweenTime = 0.75f; // how fast the offset happens in the apply offset tween method
    private float fallingInDistanceY = 250.0f;
    private float fallingOutDistanceY = 400.0f;

    private const float RisingOffsetTweenTime = 0.75f; // how fast the offset happens in the apply offset tween method
    private float risingInDistanceY = -80.0f;
    private float risingOutDistanceY = -150.0f;

    private float verticalOffset;
    private float fallingThreshold = 775.0f;
    private enum CameraYMovementState
    {
        Falling,
        Rising,
        Reset,
        Default
    }
    private CameraYMovementState currentCameraState;
    private float fillRate = 0.4f;   // lower = slower fill (≈ 2 jumps)
    private float decayRate = 0.8f;  // higher = faster reset
    private float normalizedY;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");
        airState = GetNode<AirState>("/root/Main/World/Player/PLAYERSTATEMACHINE/AIR STATE");
        inputManager = GetNode<InputManager>("/root/InputManager");

        maxPlayerSpeed = groundStateScript.GroundMoveSpeed;
        threshold = maxPlayerSpeed * SpeedPercentageThreshold;

        zoomInTimer = GetNode<Timer>("ZoomInTimer");
        cameraRecenterTimer = GetNode<Timer>("RecenterTimer");

        currentCameraState = CameraYMovementState.Default;

        // Sets the Player Zoom
        defaultZoom = Zoom;

        PlayerCameraLimitTop = LimitTop;
        PlayerCameraLimitBottom = LimitBottom;
        PlayerCameraLimitLeft = LimitLeft;
        PlayerCameraLimitRight = LimitRight;

        // Sets the Camera Position
        PlayerCameraPosition = Position;

        TweenCameraOffsetY(defaultOffsetY, VerticalResetTweenTime);
    }
    #endregion

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("test_method"))

        {
            //ForceRecenterY(playerCB2D);
            RecenterOffsetY(playerCB2D);
        }

    }

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        GD.Print(Offset.Y);
        //Two Systems Working Seperately
        /*Camera Zoom
        //Works with Timers Before Zooming In
            - Idle - Zoomed In (Default Position)
            - Velocity < 90% Max Speed - No Change In Zoom
            - Velocity > 90% Max Speed - Start Zoom Out
            - Back To Idle - Wait Timer - Zoom In Over Time
        */
        #region Camera Zoom
        if (stateMachineScript.smPlayerVelocity.Length() > threshold)
        {
            // Increment timer when moving above threshold
            moveTimer += (float)delta;

            // Only trigger zoom if player has moved steadily for the required time
            if (moveTimer >= MoveDelayTimer && !isZoomedOut)
            {
                isZoomedOut = true;

                Tween zoomTween = CreateTween();
                zoomTween.TweenProperty(this, "zoom", ZoomOutMax, ZoomOutDuration)
                         .SetTrans(Tween.TransitionType.Linear)
                         .SetEase(Tween.EaseType.InOut);
            }
        }
        else
        {
            // Reset timer if player stops or slows down
            moveTimer = 0f;
        }

        if (stateMachineScript.smPlayerVelocity.Length() == 0)
        {
            if (zoomInTimer.IsStopped())
            {
                zoomInTimer.Start();
            }
        }
        else
        {
            if (!zoomInTimer.IsStopped())
            {
                zoomInTimer.Stop();
            }
        }
        #endregion

        /*Camera Movement
        //Moving Ahead of Player happens after Player Drag Margin is Active
        /*
            - Idle - Centered on Player(Default Position)/ Tween back to center when camera is not dragged
            - Drag Margin Enabled
                - Modify offset to move camera ahead of player when camera is dragged
                - Based on Player Velocity X/Y
                    - (-Left/+Right)
                    - (-Up/+Down)
        */
        #region CameraMovement Y
        #region Player Jumping Consecutively
        // Normalized vertical influence (0–1)
        normalizedY = Mathf.Clamp(Mathf.Abs(stateMachineScript.smPlayerVelocity.Y) / Mathf.Abs(stateMachineScript.smPlayerJumpVelocity), 0f, 1f);

        if (!playerCB2D.IsOnFloor() && currentCameraState != CameraYMovementState.Falling)
        {
            // AIR: only fill, never decay
            verticalOffset += normalizedY * fillRate * Mathf.Abs(stateMachineScript.smPlayerJumpVelocity) * (float)delta;
        }
        else
        {
            // GROUND: decay
            verticalOffset -= decayRate * Mathf.Abs(stateMachineScript.smPlayerJumpVelocity) * (float)delta;
        }

        // Clamp once at the end
        verticalOffset = Mathf.Clamp(verticalOffset, 0f, Mathf.Abs(stateMachineScript.smPlayerJumpVelocity));
        verticalOffset = Mathf.Round(verticalOffset);
        #endregion

        #region Determining Enum State
        if (!playerCB2D.IsOnFloor() && stateMachineScript.smPlayerVelocity.Y > fallingThreshold)
        {
            //do falling camera
            currentCameraState = CameraYMovementState.Falling;
        }
        else if (verticalOffset > Math.Abs(stateMachineScript.smPlayerJumpVelocity) * 0.3f)
        {
            //do rising camera
            currentCameraState = CameraYMovementState.Rising;
        }
        else
        {
            //default camera
            currentCameraState = CameraYMovementState.Reset;
        }
        #endregion

        #region Enum Switch Statement
        switch (currentCameraState)
        {
            case CameraYMovementState.Falling:
                //TweenCameraOffsetY(fallingInDistanceY, FallingOffsetTweenTime);
                if (Zoom == ZoomOutMax)
                {
                    //Zoom Out
                    TweenCameraOffsetY(fallingOutDistanceY, FallingOffsetTweenTime);
                }
                else
                {
                    //Zoom In
                    TweenCameraOffsetY(fallingInDistanceY, FallingOffsetTweenTime);
                }
                break;
            case CameraYMovementState.Rising:
                //TweenCameraOffsetY(risingInDistanceY, RisingOffsetTweenTime);
                if (Zoom == ZoomOutMax)
                {
                    //Zoom Out
                    TweenCameraOffsetY(risingOutDistanceY, RisingOffsetTweenTime);
                }
                else
                {
                    //Zoom In
                    TweenCameraOffsetY(risingInDistanceY, RisingOffsetTweenTime);
                }
                break;
            case CameraYMovementState.Reset:
                //Reset
                TweenCameraOffsetY(defaultOffsetY, VerticalResetTweenTime);
                break;
            case CameraYMovementState.Default:
                GD.PushWarning("CameraYMovementState Not Set to Correct State");
                break;
        }
        #endregion
        #endregion

        #region CameraMovement X
        //Check whether the player has changed direction
        bool directionFlipped = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX) != Mathf.Sign(xTargetOffset) && xTargetOffset != 0;
        // When moving quickly Camera adjusts with player directly according to speed
        if (Mathf.Abs(stateMachineScript.smPlayerVelocity.X) >= threshold)
        {
            cameraRecenterTimer.Stop();
            startTimer = false;
            returningToCenter = false; // cancel any center return if movement resumes

            #region TweenManagement
            // Resume tween if paused and direction hasn’t changed
            if (xTween != null && IsInstanceValid(xTween) && !xTween.IsRunning() && !directionFlipped)
            {
                xTween.Play(); // resume timer
            }

            if (directionFlipped)
            {
                xReachedMaxOffset = false;

                // Restart tween for direction change
                if (xTween != null && IsInstanceValid(xTween))
                    xTween.Kill();

                xTween = null;
            }
            #endregion

            // Determine offset according to player speed
            xNewTargetOffset = (stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX; // Changes the max offset to a percentage of player speed.

            // Start tween if target changes
            if (xNewTargetOffset != xTargetOffset)
            {
                xTargetOffset = xNewTargetOffset;
                ApplyOffsetTweenX();
            }
        }
        // TO DO: else if (stateMachineScript.smPlayerVelocity.Length() < threshold)
        // camera slow recentering & quick recentering if direction flipped while moving
        else if (Mathf.Abs(stateMachineScript.smPlayerVelocity.X) < threshold)
        {
            xNewTargetOffset = 0.0f;

            // Start tween if target changes
            if (xNewTargetOffset != xTargetOffset)
            {
                xTargetOffset = xNewTargetOffset;
                ApplyOffsetTweenX(HorizontalOffsetUnderThresholdTimer);
            }
        }
        else if (Mathf.Abs(stateMachineScript.smPlayerVelocity.X) == 0.0f)
        {
            // only pause normal tweens if camera is not returning to center
            if (!returningToCenter && xTween != null && IsInstanceValid(xTween) && xTween.IsRunning())
            {
                xTween.Pause();
            }

            // Skip the "hold offset" freeze while returning to center
            if (!returningToCenter)
            {
                xNewTargetOffset = xTargetOffset;
            }

            // After delay, return to center (new tween)
            if (!startTimer)
            {
                startTimer = true;
                cameraRecenterTimer.Start();
            }
        }
        #endregion
    }
    #endregion

    #region Apply Tween Offsets
    #region X Offset
    private void ApplyOffsetTweenX(float duration = HorizontalOffsetTweenTime)
    {
        // If a tween is active, stop and clear it
        if (xTween != null && IsInstanceValid(xTween) && xTween.IsRunning())
        {
            xTween.Kill();
            xTween = null;
        }

        // Create new tween safely
        xTween = CreateTween();

        // Ensure we clear reference when finished to avoid "freed object" calls
        xTween.Finished += () =>
        {
            xTween = null;
            returningToCenter = false; // allow normal stationary logic again
        };

        xTween.TweenProperty(this, "drag_horizontal_offset", xTargetOffset, duration)
               .SetTrans(Tween.TransitionType.Linear)
               .SetEase(Tween.EaseType.In);
    }
    #endregion

    #region Y Offset
    private void TweenCameraOffsetY(float targetY, float duration)
    {
        if (yTween != null && IsInstanceValid(yTween))
        {
            yTween.Kill();
            yTween = null;
        }

        yTween = CreateTween();

        yTween.Finished += () =>
        {
            yTween = null;
        };

        yTween.TweenProperty(this, "offset:y", targetY, duration)
                .SetTrans(Tween.TransitionType.Linear)
                .SetEase(Tween.EaseType.In);
    }
    #endregion
    #endregion

    #region Camera Boundary
    // Used to Set the Boundaries in an Area so the Camera doesn't Show Places it Shouldn't
    public void SetCameraLimits(int top, int bottom, int left, int right)
    {
        PlayerCameraLimitTop = top;
        PlayerCameraLimitBottom = bottom;
        PlayerCameraLimitLeft = left;
        PlayerCameraLimitRight = right;

        LimitTop = PlayerCameraLimitTop;
        LimitBottom = PlayerCameraLimitBottom;
        LimitLeft = PlayerCameraLimitLeft;
        LimitRight = PlayerCameraLimitRight;
    }
    #endregion

    #region Signals
    #region Zoom Timer Signal
    // Timer used for code activate after Player is back to idle but can be interrupted if velocity is over threshold
    public void ZoomInTimer()
    {
        if (Zoom != defaultZoom)
        {
            isZoomedOut = false;

            Tween zoomTween = CreateTween();
            zoomTween.TweenProperty(this, "zoom", defaultZoom, ZoomInDuration)
                     .SetTrans(Tween.TransitionType.Linear)
                     .SetEase(Tween.EaseType.In);
        }
    }
    #endregion

    #region Camera Recentering Timer Signal
    public void CameraRecenterX()
    {
        if (xTargetOffset != 0)
        {
            xNewTargetOffset = 0;

            // Safely stop paused tween before new one
            if (xTween != null && IsInstanceValid(xTween) && !xTween.IsRunning())
            {
                xTween.Kill();
                xTween = null;
            }

            xTargetOffset = xNewTargetOffset;

            returningToCenter = true; // prevent stationary block from pausing it

            ApplyOffsetTweenX(HorizontalOffsetStationaryTimer);
        }
    }

    #endregion

    public async void ForceRecenterY(CharacterBody2D player)
    {
        GD.Print("ForceRecenterY");
        // Disable drag so margins don't block recentering
        DragVerticalEnabled = false;

        // Reset offset
        Offset = new Vector2(Offset.X, defaultOffsetY);

        // Snap camera to player Y immediately
        GlobalPosition = new Vector2(GlobalPosition.X, player.GlobalPosition.Y);

        // Wait one frame so Godot updates internals
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // Re-enable drag
        DragVerticalEnabled = true;
    }

    public async void RecenterOffsetY(CharacterBody2D player)
    {
        GD.Print("RecenterOffsetY");
        // Disable drag so margins don't block recentering
        DragVerticalEnabled = false;

        // Snap camera to player Y immediately
        GlobalPosition = new Vector2(GlobalPosition.X, player.GlobalPosition.Y);

        // Wait one frame so Godot updates internals
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // Re-enable drag
        DragVerticalEnabled = true;
    }
    #endregion
    #endregion
}