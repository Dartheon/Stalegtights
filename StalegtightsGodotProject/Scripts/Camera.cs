using Godot;
using System.Collections.Generic;

public partial class Camera : Camera2D
{
    private CharacterBody2D playerCB2D;

    private const float LOOKAHEAD_STRENGTH = 100f;
    private const float LOOKAHEAD_SPEED = 2f;
    private const float SMOOTHING_SPEED = 5f;
    private const float SMOOTHING_SNAP_THRESHOLD = 2f;
    private const float ZOOM_SPEED = 3f;
    private static readonly Vector2 OFFSET_ABOVE_PLAYER = new(0, -100);
    private const float PEEK_DISTANCE = 160f;
    private const float PEEK_TWEEN_DURATION = 0.4f;

    private List<CameraArea> _allAreas = new();
    private CameraArea _currentArea = null;
    private CameraArea _previousArea = null;

    private bool _transitioning = false;
    private Tween _transitionTween = null;

    private Vector2 _velocitySmooth = Vector2.Zero;
    private Vector2 _lookAheadTarget = Vector2.Zero;
    private Vector2 _lookAheadCurrent = Vector2.Zero;

    private Tween _peekTween = null;

    private Vector2 _baseOffset = Vector2.Zero;
    private Vector2 _peekOffset = Vector2.Zero;
    private Vector2 _shakeOffset = Vector2.Zero;

    public override void _Ready()
    {
        playerCB2D = GetNode<CharacterBody2D>("/root/Main/World/Player");

        Godot.Collections.Array<Node> nodes = GetTree().GetNodesInGroup("camera_area");
        foreach (Node n in nodes)
        {
            if (n is CameraArea ca)
                _allAreas.Add(ca);
        }

        _baseOffset = Offset;
    }

    public override void _PhysicsProcess(double delta)
    {
        Offset = _baseOffset + _peekOffset + _shakeOffset;

        FindCurrentArea();

        Vector2 desired = GetDesiredPosition((float)delta);
        Vector2 bounded = _currentArea != null ? _currentArea.GetBoundPosition(desired) : desired;

        UpdateCameraPosition(bounded, (float)delta);
        UpdateZoom((float)delta);
    }

    // ---------------------------------------------------------
    // LOOKAHEAD
    // ---------------------------------------------------------
    private Vector2 GetLookAhead(float delta)
    {
        float w = Mathf.Clamp(LOOKAHEAD_SPEED * delta, 0f, 1f);

        _velocitySmooth = _velocitySmooth.Lerp(playerCB2D.Velocity, w);

        if (_velocitySmooth.Length() > 10f)
            _lookAheadTarget = _velocitySmooth.Normalized() * LOOKAHEAD_STRENGTH;

        _lookAheadCurrent = _lookAheadCurrent.Lerp(_lookAheadTarget, w);

        return _lookAheadCurrent;

    }

    private Vector2 GetDesiredPosition(float delta)
    {
        return playerCB2D.GlobalPosition + OFFSET_ABOVE_PLAYER + GetLookAhead(delta);
    }

    // ---------------------------------------------------------
    // CAMERA POSITION
    // ---------------------------------------------------------
    private void UpdateCameraPosition(Vector2 target, float delta)
    {
        if (_transitioning) { return; }

        if (GlobalPosition.DistanceTo(target) < SMOOTHING_SNAP_THRESHOLD)
        {
            float w = Mathf.Clamp(SMOOTHING_SPEED * delta, 0f, 1f);
            GlobalPosition = GlobalPosition.Lerp(target, w);
            GlobalPosition = target;
        }
        else
        {
            GlobalPosition = target;
        }
    }

    // ---------------------------------------------------------
    // ZOOM
    // ---------------------------------------------------------
    private void UpdateZoom(float delta)
    {
        if (_currentArea == null) { return; }

        Vector2 targetZoom = Vector2.One * _currentArea.zoomLevel;
        Zoom = Zoom.Lerp(targetZoom, ZOOM_SPEED * delta);
    }

    // ---------------------------------------------------------
    // AREA DETECTION
    // ---------------------------------------------------------
    private void FindCurrentArea()
    {
        Vector2 pos = playerCB2D.GlobalPosition;

        if (_currentArea != null && _currentArea.ContainsPoint(pos)) { return; }

        foreach (CameraArea area in _allAreas)
        {
            if (area.ContainsPoint(pos))
            {
                if (area != _currentArea)
                {
                    EnterArea(area);
                }
                return;
            }
        }
    }

    // ---------------------------------------------------------
    // INSTANT SNAP (on load)
    // ---------------------------------------------------------
    public void InstantSnap()
    {
        _velocitySmooth = Vector2.Zero;
        _lookAheadTarget = Vector2.Zero;
        _lookAheadCurrent = Vector2.Zero;

        foreach (CameraArea area in _allAreas)
        {
            if (area.ContainsPoint(playerCB2D.GlobalPosition))
            {
                _currentArea = area;
                GlobalPosition = area.GetBoundPosition(playerCB2D.GlobalPosition + OFFSET_ABOVE_PLAYER);
                Zoom = Vector2.One * area.zoomLevel;
                return;
            }
        }
    }

    // ---------------------------------------------------------
    // AREA ENTERING TRANSITION
    // ---------------------------------------------------------
    private void EnterArea(CameraArea next)
    {
        _previousArea = _currentArea;
        _currentArea = next;

        if (_previousArea == null) { return; }

        Vector2 desired = playerCB2D.GlobalPosition + OFFSET_ABOVE_PLAYER + _lookAheadCurrent;
        Vector2 newBoundPos = _currentArea.GetBoundPosition(desired);
        Vector2 zoomTarget = Vector2.One * _currentArea.zoomLevel;

        _transitionTween?.Kill();

        _transitioning = true;

        _transitionTween = CreateTween();
        _transitionTween.SetEase(_currentArea.enterEase);
        _transitionTween.SetTrans(_currentArea.enterTrans);

        _transitionTween.TweenProperty(this, "global_position", newBoundPos, _currentArea.enterDuration);
        _transitionTween.Parallel().TweenProperty(this, "zoom", zoomTarget, _currentArea.enterDuration);

        _transitionTween.Finished += () => _transitioning = false;
    }

    // ---------------------------------------------------------
    // SPECIAL EFFECTS: PEEK
    // ---------------------------------------------------------
    public void RequestPeek(int dir)
    {
        if (dir == 0)
        {
            CancelPeek();
            return;
        }

        float y = dir * PEEK_DISTANCE;
        TweenPeek(new Vector2(0, y));
    }

    public void CancelPeek()
    {
        TweenPeek(Vector2.Zero);
    }

    private void TweenPeek(Vector2 target)
    {
        _peekTween?.Kill();

        _peekTween = CreateTween();
        _peekTween.SetTrans(Tween.TransitionType.Sine);
        _peekTween.SetEase(Tween.EaseType.Out);
        _peekTween.TweenProperty(this, nameof(_peekOffset), target, PEEK_TWEEN_DURATION);
    }

    // ---------------------------------------------------------
    // SPECIAL EFFECTS: SHAKE
    // ---------------------------------------------------------
    public async void Shake(float magnitude, float duration)
    {
        float elapsed = 0f;
        float interval = 0.05f;

        while (elapsed < duration)
        {
            _shakeOffset = new Vector2(
                (float)GD.RandRange(-magnitude, magnitude),
                (float)GD.RandRange(-magnitude, magnitude)
            );

            GD.RandRange(elapsed, elapsed);

            await ToSignal(GetTree().CreateTimer(interval), "timeout");
            elapsed += interval;
        }

        _shakeOffset = Vector2.Zero;
    }
}