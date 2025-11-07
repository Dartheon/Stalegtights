using System;
using Godot;

public partial class PlayerCamera : Camera2D
{
    #region Variables
    // References
    private StateMachine stateMachineScript;
    private GroundState groundStateScript;
    private Player playerCB2D;

    // Camera Limits
    public int PlayerCameraLimitTop { get; set; }
    public int PlayerCameraLimitBottom { get; set; }
    public int PlayerCameraLimitLeft { get; set; }
    public int PlayerCameraLimitRight { get; set; }

    // Camera Zoom
    private Vector2 playerCameraZoom;
    private Vector2 defaultZoom = new(1.0f, 1.0f);
    private Vector2 zoomOutMax = new(0.9f, 0.9f); // how far camera can zoom out

    // Camera Positioning
    public Vector2 PlayerCameraPosition { get; set; }

    // Player Movement
    private Vector2 smPlayerVelocity => stateMachineScript.smPlayerVelocity;
    private float maxPlayerSpeed;

    //Zoom Properties
    [Export] public float ZoomOutDuration { get; set; } = 1.5f; // seconds to fully zoom out
    [Export] public float ZoomInDuration { get; set; } = 1.5f;  // seconds to fully zoom in
    [Export] public float MoveHoldTime { get; set; } = 3.0f; // time player must move before zooming out
    [Export] public float SpeedPercentage { get; set; } = 0.9f; //the percentage used for the threshold

    private Timer zoomInTimer; //hold the Timer child to reference
    private float threshold; //used to determine the percentage of the max speed before starting the zoom
    private float moveTimer = 0f;
    private bool isZoomedOut = false;

    //Drag Properties
    public const float HorizontalOffsetTweenTime = 2.0f; // how fast the offset happens in the applly offset tween method
    [Export] public float HorizontalOffsetStationaryTimer { get; set; } = 10.0f;
    [Export] public float VerticalOffsetTweenTime { get; set; } = 2.0f; // how fast the offset happens in the applly offset tween method
    [Export] public float OffsetDistance { get; set; } = 1.0f;
    private Vector2 _targetOffset = Vector2.Zero;
    private Vector2 newTargetOffset = Vector2.Zero;
    private float stopTimer = 0.0f;
    private float previousDirection = 0f;
    private bool directionChanged = false;
    private Tween _xTween;
    private bool _reachedMaxOffset = false;
    private bool _returningToCenter = false;
    private bool startTimer = false;
    private Timer cameraRecenterTimer;

    #endregion

    #region Methods
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");

        maxPlayerSpeed = groundStateScript.GroundMoveSpeed;
        threshold = maxPlayerSpeed * SpeedPercentage;

        zoomInTimer = GetNode<Timer>("ZoomInTimer");
        cameraRecenterTimer = GetNode<Timer>("RecenterTimer");

        // Sets the Player Zoom
        playerCameraZoom = Zoom = defaultZoom;

        PlayerCameraLimitTop = LimitTop;
        PlayerCameraLimitBottom = LimitBottom;
        PlayerCameraLimitLeft = LimitLeft;
        PlayerCameraLimitRight = LimitRight;

        // Sets the Camera Position
        PlayerCameraPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        //Two Systems Working Seperately
        /*Camera Zoom
        //Works with Timers Before Zooming In
            - Idle - Zoomed In (Default Position)
            - Velocity < 60% Max Speed - No Change In Zoom
            - Velocity > 60% Max Speed - Start Zoom Out
            - Back To Idle - Wait Timer - Zoom In Over Time
        */
        if (stateMachineScript.smPlayerVelocity.Length() > threshold)
        {
            // Increment timer when moving above threshold
            moveTimer += (float)delta;

            // Only trigger zoom if player has moved steadily for the required time
            if (moveTimer >= MoveHoldTime && !isZoomedOut)
            {
                isZoomedOut = true;

                Tween zoomTween = CreateTween();
                zoomTween.TweenProperty(this, "zoom", zoomOutMax, ZoomOutDuration)
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

        /*Camera Movement
        //Moving Ahead of Player happens after Player Drag Margin is Active
        /*
            - Idle - Centered on Player(Default Position)/ Tween back to center when camera is not dragged
            - Drag Margin Enabled
                - Modify offset to move camera ahead of player when camera is dragged
                - Based on Camera Movement X/Y
                    - (-Left/+Right)
                    - (-Up/+Down)
        */
        if (stateMachineScript.smPlayerVelocity.Length() > threshold)
        {
            cameraRecenterTimer.Stop();
            startTimer = false;
            _returningToCenter = false; // cancel any center return if movement resumes

            bool directionFlipped = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistance) != Mathf.Sign(_targetOffset.X) && _targetOffset.X != 0;

            // Resume tween if paused and direction hasn’t changed
            if (_xTween != null && IsInstanceValid(_xTween) && !_xTween.IsRunning() && !directionFlipped)
            {
                _xTween.Play(); // resume smoothly
            }

            if (directionFlipped)
            {
                _reachedMaxOffset = false;

                // Restart tween for direction change
                if (_xTween != null && IsInstanceValid(_xTween))
                    _xTween.Kill();

                _xTween = null;
            }

            // Determine movement offset
            if (!_reachedMaxOffset && Mathf.Abs((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistance) >= Mathf.Abs(OffsetDistance))
            {
                newTargetOffset.X = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistance) * Mathf.Abs(OffsetDistance);
                _reachedMaxOffset = true;
            }
            else if (_reachedMaxOffset)
            {
                newTargetOffset.X = Mathf.Sign((stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistance) * Mathf.Abs(OffsetDistance);
            }
            else
            {
                newTargetOffset.X = (stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed) * OffsetDistance;
            }

            // Start tween if target changes
            if (newTargetOffset != _targetOffset)
            {
                _targetOffset = newTargetOffset;
                ApplyOffsetTween();
            }
        }
        else if (stateMachineScript.smPlayerVelocity.Length() == 0.0f)
        {
            // only pause normal tweens if we're not returning to center
            if (!_returningToCenter && _xTween != null && IsInstanceValid(_xTween) && _xTween.IsRunning())
            {
                _xTween.Pause();
            }

            // Skip the "hold offset" freeze while returning to center
            if (!_returningToCenter)
            {
                newTargetOffset = _targetOffset;
            }

            // After delay, return to center (new tween)
            if (!startTimer)
            {
                startTimer = true;
                cameraRecenterTimer.Start();
            }
        }
    }

    private void ApplyOffsetTween(float duration = HorizontalOffsetTweenTime)
    {
        // If a tween is active, stop and clear it
        if (_xTween != null && IsInstanceValid(_xTween) && _xTween.IsRunning())
        {
            _xTween.Kill();
            _xTween = null;
        }

        // Create new tween safely
        _xTween = CreateTween();

        // Ensure we clear reference when finished to avoid "freed object" calls
        _xTween.Finished += () =>
        {
            _xTween = null;
            _returningToCenter = false; // allow normal stationary logic again
        };

        _xTween.TweenProperty(this, "drag_horizontal_offset", _targetOffset.X, duration)
               .SetTrans(Tween.TransitionType.Linear)
               .SetEase(Tween.EaseType.In);
    }

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

    #region Signals
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

    public void CameraRecenter()
    {
        if (_targetOffset.X != 0)
        {
            newTargetOffset.X = 0;

            // Safely stop paused tween before new one
            if (_xTween != null && IsInstanceValid(_xTween) && !_xTween.IsRunning())
            {
                _xTween.Kill();
                _xTween = null;
            }

            _targetOffset = newTargetOffset;

            _returningToCenter = true; // prevent stationary block from pausing it

            ApplyOffsetTween(HorizontalOffsetStationaryTimer);
        }
    }
    #endregion
    #endregion
}