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
    private float threshold; //used to determine the percentage of the max speed before starting the zoom
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
    [Export] public float OffsetDistanceX { get; set; } = 1.0f; //how far the offset will go
    private Vector2 xTargetOffset = Vector2.Zero;
    private Vector2 xNewTargetOffset = Vector2.Zero;
    private float xStopTimer = 0.0f;
    private bool xReachedMaxOffset = false;

    //Drag Properties Y
    private Tween yTween;
    private const float VerticalOffsetTweenTime = 2.0f; // how fast the offset happens in the applly offset tween method
    [Export] public float OffsetDistanceY { get; set; } = 1.0f; //how far the offset will go
    private Vector2 yTargetOffset = Vector2.Zero;
    private Vector2 yNewTargetOffset = Vector2.Zero;

    private Vector2 restingVerticalOffset = new(0, -150.0f);
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");

        maxPlayerSpeed = groundStateScript.GroundMoveSpeed;
        threshold = maxPlayerSpeed * SpeedPercentageThreshold;

        zoomInTimer = GetNode<Timer>("ZoomInTimer");
        cameraRecenterTimer = GetNode<Timer>("RecenterTimer");

        // Sets the Player Zoom
        defaultZoom = Zoom;

        PlayerCameraLimitTop = LimitTop;
        PlayerCameraLimitBottom = LimitBottom;
        PlayerCameraLimitLeft = LimitLeft;
        PlayerCameraLimitRight = LimitRight;

        // Sets the Camera Position
        PlayerCameraPosition = Position;
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
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
        /*
        Position Down
        Drag Margin Maintains 0.45
        Idle: -150f
        Moving Camera Down Max: 250f(400?)
        Idle After Moving Down: 0f

        when camera moves down(1) tween camera offset vertical from default -150 to 250(400?)

        When stationary, Timer, then tween offset vertical to 0

        Vertical Offset Up
        Top Margin 0.77 = Regular Jump without shifting camera
        */
        #endregion

        #region CameraMovement X
        if (stateMachineScript.smPlayerVelocity.Length() > threshold)
        {
            cameraRecenterTimer.Stop();
            startTimer = false;
            returningToCenter = false; // cancel any center return if movement resumes

            bool directionFlipped = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX) != Mathf.Sign(xTargetOffset.X) && xTargetOffset.X != 0;

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

            // Determine movement offset
            if (!xReachedMaxOffset && Mathf.Abs((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX) >= Mathf.Abs(OffsetDistanceX))
            {
                xNewTargetOffset.X = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX) * Mathf.Abs(OffsetDistanceX);
                xReachedMaxOffset = true;
            }
            else if (xReachedMaxOffset)
            {
                xNewTargetOffset.X = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX) * Mathf.Abs(OffsetDistanceX);
            }
            else
            {
                xNewTargetOffset.X = (stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistanceX;
            }

            // Start tween if target changes
            if (xNewTargetOffset != xTargetOffset)
            {
                xTargetOffset = xNewTargetOffset;
                ApplyOffsetTweenX();
            }
        }
        else if (stateMachineScript.smPlayerVelocity.Length() == 0.0f)
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

        xTween.TweenProperty(this, "drag_horizontal_offset", xTargetOffset.X, duration)
               .SetTrans(Tween.TransitionType.Linear)
               .SetEase(Tween.EaseType.In);
    }

    private void ApplyOffsetTweenY(float duration = VerticalOffsetTweenTime)
    {
        // If a tween is active, stop and clear it
        if (yTween != null && IsInstanceValid(yTween) && yTween.IsRunning())
        {
            yTween.Kill();
            yTween = null;
        }

        // Create new tween safely
        yTween = CreateTween();

        // Ensure we clear reference when finished to avoid "freed object" calls
        yTween.Finished += () =>
        {
            yTween = null;
            returningToCenter = false; // allow normal stationary logic again
        };

        yTween.TweenProperty(this, "drag_vertical_offset", yTargetOffset.Y, duration)
               .SetTrans(Tween.TransitionType.Linear)
               .SetEase(Tween.EaseType.In);
    }
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
        if (xTargetOffset.X != 0)
        {
            xNewTargetOffset.X = 0;

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

    public void CameraRecenterY()
    {
        //
    }
    #endregion
    #endregion
    #endregion
}