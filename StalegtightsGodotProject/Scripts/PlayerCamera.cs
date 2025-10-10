using Godot;

public partial class PlayerCamera : Camera2D
{
    #region Variables
    // References
    private StateMachine stateMachineScript;
    private Player playerCB2D;

    // Camera Limits
    public int PlayerCameraLimitTop { get; set; }
    public int PlayerCameraLimitBottom { get; set; }
    public int PlayerCameraLimitLeft { get; set; }
    public int PlayerCameraLimitRight { get; set; }

    // --- Configurable Settings ---
    [Export] public Vector2 BaseZoom { get; set; } = new Vector2(1f, 1f);
    [Export] public float ZoomOutMax = 1.3f;           // Maximum zoom-out factor
    [Export] public float ZoomSpeed = 2f;              // How quickly the camera zooms
    [Export] public float FollowOffset = 100f;         // How far ahead camera leads
    [Export] public float FollowSmoothness = 5f;       // Smoothness of follow/offset
    [Export] public float SpeedThreshold = 100f;       // Minimum speed before offset applies
    [Export] public Vector2 CameraMargins = new Vector2(100f, 60f); // Margin box around player
    [Export] public float ZoomStartPercent = 0.6f;     // % of velocity.X at which zoom-out begins (60%)
    [Export] public float MaxSpeedForZoom = 600f;      // Speed at which full zoom-out is reached

    // Internal state
    private Vector2 smPlayerVelocity;
    private Vector2 targetOffset;
    private Vector2 currentOffset;
    private Vector2 targetZoom;
    private Vector2 currentZoom;
    private Vector2 playerPosition;
    private Vector2 cameraTargetPos;
    #endregion

    #region Methods
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");

        PlayerCameraLimitTop = LimitTop;
        PlayerCameraLimitBottom = LimitBottom;
        PlayerCameraLimitLeft = LimitLeft;
        PlayerCameraLimitRight = LimitRight;

        currentZoom = Zoom;
        targetZoom = BaseZoom;
    }

    public override void _PhysicsProcess(double delta)
    {
        smPlayerVelocity = stateMachineScript.smPlayerVelocity;
        playerPosition = playerCB2D.GlobalPosition;

        float horizontalSpeed = Mathf.Abs(smPlayerVelocity.X);
        float overallSpeed = smPlayerVelocity.Length();

        // ---- CAMERA FOLLOW WITH MARGIN ----
        Vector2 cameraToPlayer = playerPosition - GlobalPosition;
        Vector2 moveOffset = Vector2.Zero;

        if (Mathf.Abs(cameraToPlayer.X) > CameraMargins.X)
            moveOffset.X = cameraToPlayer.X - Mathf.Sign(cameraToPlayer.X) * CameraMargins.X;

        if (Mathf.Abs(cameraToPlayer.Y) > CameraMargins.Y)
            moveOffset.Y = cameraToPlayer.Y - Mathf.Sign(cameraToPlayer.Y) * CameraMargins.Y;

        cameraTargetPos = GlobalPosition + moveOffset;
        GlobalPosition = GlobalPosition.Lerp(cameraTargetPos, (float)delta * FollowSmoothness);

        // ---- CAMERA OFFSET / LOOK-AHEAD ----
        if (overallSpeed > SpeedThreshold)
        {
            targetOffset = smPlayerVelocity.Normalized() * FollowOffset;
        }
        else
        {
            targetOffset = Vector2.Zero;
        }

        currentOffset = currentOffset.Lerp(targetOffset, (float)delta * FollowSmoothness);
        Offset = currentOffset;

        // ---- CAMERA ZOOM ----
        // Start zooming out when velocity.X exceeds 60% of threshold
        float zoomStartSpeed = MaxSpeedForZoom * ZoomStartPercent;

        float excessSpeed = Mathf.Max(0f, horizontalSpeed - zoomStartSpeed);
        float zoomPercent = Mathf.Clamp(excessSpeed / (MaxSpeedForZoom - zoomStartSpeed), 0f, 1f);

        // Smooth zoom factor
        float zoomFactor = Mathf.Lerp(1f, ZoomOutMax, zoomPercent);
        targetZoom = BaseZoom / zoomFactor;

        currentZoom = currentZoom.Lerp(targetZoom, (float)delta * ZoomSpeed);
        Zoom = currentZoom;
    }

    public void SetCameraLimits(int top, int bottom, int left, int right)
    {
        LimitTop = top;
        LimitBottom = bottom;
        LimitLeft = left;
        LimitRight = right;

        PlayerCameraLimitTop = top;
        PlayerCameraLimitBottom = bottom;
        PlayerCameraLimitLeft = left;
        PlayerCameraLimitRight = right;
    }
    #endregion
}


/*using Godot;

public partial class PlayerCamera : Camera2D
{
    #region Variables
    #region Class Scripts
    private StateMachine stateMachineScript;
    #endregion

    //Camera Limits
    public int PlayerCameraLimitTop { get; set; }
    public int PlayerCameraLimitBottom { get; set; }
    public int PlayerCameraLimitLeft { get; set; }
    public int PlayerCameraLimitRight { get; set; }

    //Camera Zoom
    public Vector2 PlayerCameraZoom { get; set; }

    //Camera Positioning
    public Vector2 PlayerCameraPosition { get; set; }

    //Player Movement
    private Vector2 smPlayerVelocity;
    #endregion

    #region Methods
    public override void _Ready()
    {
        //Class Variables
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");

        //Setting Camera Boundary Limits
        PlayerCameraLimitTop = LimitTop;
        PlayerCameraLimitBottom = LimitBottom;
        PlayerCameraLimitLeft = LimitLeft;
        PlayerCameraLimitRight = LimitRight;

        //Sets the Player Zoom
        PlayerCameraZoom = Zoom;

        //Sets the Camera Positon
        PlayerCameraPosition = Position;

        //Player Movement
        smPlayerVelocity = stateMachineScript.smPlayerVelocity;
    }
    #endregion
}*/