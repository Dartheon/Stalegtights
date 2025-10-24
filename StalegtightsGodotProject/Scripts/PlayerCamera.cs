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
    private float threshold;
    private Vector2 zoomLock = new();
    private float zoomOutSpeed = 0f;
    private float zoomInSpeed = 1f;
    private Timer zoomInTimer;
    [Export] public float zoomOutDuration = 1.5f; // seconds to fully zoom out
    [Export] public float zoomInDuration = 0.8f;  // seconds to fully zoom in

    //Drag Properties
    [Export] public float TweenTime = 2.0f; // how fast the offset happens
    [Export] public float OffsetDistance = 0.5f;
    private Vector2 _targetOffset = Vector2.Zero;
    private Vector2 newTargetOffset = Vector2.Zero;

    #endregion

    #region Methods
    public override void _Ready()
    {
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");
        playerCB2D = GetNode<Player>("/root/Main/World/Player");

        maxPlayerSpeed = groundStateScript.GroundMoveSpeed;
        threshold = maxPlayerSpeed * 0.9f;

        zoomInTimer = GetNode<Timer>("ZoomInTimer");

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
        /*Camera Zoom - reentable after making movement work
        //Works with Timers Before Zooming In
            - Idle - Zoomed In (Default Position)
            - Velocity < 60% Max Speed - No Change In Zoom
            - Velocity > 60% Max Speed - Start Zoom Out
            - Back To Idle - Wait Timer - Zoom In Over Time
        */
        if (stateMachineScript.smPlayerVelocity.Length() > threshold)
        {
            if (Zoom != zoomOutMax)
            {
                Tween zoomTween = CreateTween();
                zoomTween.TweenProperty(this, "zoom", zoomOutMax, zoomOutDuration)
                         .SetTrans(Tween.TransitionType.Sine)
                         .SetEase(Tween.EaseType.Out);
            }
        }

        if (stateMachineScript.smPlayerVelocity.Length() == 0)
        {
            if (zoomInTimer.IsStopped())
                zoomInTimer.Start();
        }
        else
        {
            if (!zoomInTimer.IsStopped())
                zoomInTimer.Stop();
        }
        /*
        //Two Systems working seperately
        //Camera Movement
        //Moving Ahead of Player happens after Player Drag Margin is Active
        /*
            - Idle - Centered on Player(Default Position)/ Tween back to center when camera is not dragged
            - Drag Margin Enabled
                - Modify offset to move camera ahead of player when camera is dragged
                - Based on Camera Movement X/Y
                    - (-Left/+Right)
                    - (-Up/+Down)
        */

        if (stateMachineScript.smPlayerVelocity.Length() >= threshold)
        {
            newTargetOffset.X = stateMachineScript.smPlayerVelocity.X / maxPlayerSpeed * OffsetDistance;
        }
        if (stateMachineScript.smPlayerVelocity.Length() == 0.0f)
        {
            newTargetOffset.X = 0;
        }

        // Tween only when offset changes
        if (newTargetOffset != _targetOffset)
        {
            _targetOffset = newTargetOffset;
            ApplyOffsetTween();
        }
    }

    private void ApplyOffsetTween()
    {
        // Create new tweens for each axis
        Tween _xTween = CreateTween();
        _xTween.TweenProperty(this, "drag_horizontal_offset", _targetOffset.X, TweenTime)
               .SetTrans(Tween.TransitionType.Sine)
               .SetEase(Tween.EaseType.Out);

        /*Tween _yTween = CreateTween();
        _yTween.TweenProperty(this, "drag_vertical_offset", _targetOffset.Y, TweenTime)
               .SetTrans(Tween.TransitionType.Sine)
               .SetEase(Tween.EaseType.Out);*/
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
            Tween zoomTween = CreateTween();
            zoomTween.TweenProperty(this, "zoom", defaultZoom, zoomInDuration)
                     .SetTrans(Tween.TransitionType.Sine)
                     .SetEase(Tween.EaseType.InOut);
        }
        GD.Print(Zoom);
    }
    #endregion
    #endregion
}

//code to be deleted
/*using Godot;

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

    //Camera Zoom
    public Vector2 PlayerCameraZoom { get; set; }

    //Camera Positioning
    public Vector2 PlayerCameraPosition { get; set; }

    //Player Movement
    private Vector2 smPlayerVelocity => stateMachineScript.smPlayerVelocity;

    //bool States
    private bool zoomInTimer = false;
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

        //Sets the Player Zoom
        PlayerCameraZoom = Zoom;

        //Sets the Camera Positon
        PlayerCameraPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        //Two Systems working seperately
        //Camera Movement
        //Moving Ahead of Player happens after Player Drag Margin is Active
        /*
            - Idle - Centered on Player(Default Position)/ Lerp back to center when camera is not dragged
            - Drag Margin Enabled
                - Modify offset to move camera ahead of player when camera is dragged
                - Based on Camera Movement X/Y
                    - (-Left/+Right)
                    - (-Up/+Down)
        */

//Camera Zoom
//Works with Timers Before Zooming In
/*  
    - Idle - Zoomed In (Default Position)
    - Velocity < 60% Max Speed - No Change In Zoom/ Interrupt Zoom In Maintaining Current Position
    - Velocity > 60% Max Speed - Start Zoom Out/ Zoom Back Out From Interrupt Position
    - Back To Idle - Wait Timer - Zoom In Over Time

}

//Used to Set the Boundaries in an Area so the Camera doesn't Show Places it Shouldn't
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
//Timer used for code activate after Player is back to idle but can be interupted if velocity is over threshold
public void ZoomInTimer()
{
if (zoomInTimer)
{
    //zoom the camera into the player using lerp
}
}
#endregion
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
private Vector2 smPlayerVelocity => stateMachineScript.smPlayerVelocity;
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
}
#endregion
}*/