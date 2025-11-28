using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class CameraArea : Node2D
{
    public enum AreaType { STANDARD, FOCUS }

    // ---- Area Settings ----
    [Export] public AreaType areaType = AreaType.STANDARD;
    [Export] public float zoomLevel = 1.0f;

    // ---- Zone Shapes ----
    [Export] public Polygon2D jurisdiction;
    [Export] public Polygon2D centerBoundary;
    [Export] public Marker2D focusPoint;

    // ---- Transition Settings ----
    [Export] public float enterDuration = 1.0f;
    [Export] public Tween.TransitionType enterTrans = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType enterEase = Tween.EaseType.Out;

    private Rect2 _jurisdictionAabb;
    private List<Node2D> _debugMarkers = new List<Node2D>();


    public override void _Ready()
    {
        PrecomputeAabb();

        if (!Engine.IsEditorHint())
            ClearMarkers();
    }


    // ============================================================
    // ===============   CONTAINMENT / LIMIT LOGIC   ==============
    // ============================================================

    public bool ContainsPoint(Vector2 point)
    {
        if (jurisdiction == null || !_jurisdictionAabb.HasPoint(point))
            return false;

        Vector2[] worldPoints = GetWorldPolygon(jurisdiction);
        return Geometry2D.IsPointInPolygon(point, worldPoints);
    }


    public Vector2 GetBoundPosition(Vector2 desired)
    {
        if (areaType == AreaType.FOCUS && focusPoint != null)
            return focusPoint.GlobalPosition;

        return ClampToBoundary(desired);
    }


    // ============================================================
    // =================     HELPER METHODS     ====================
    // ============================================================

    private void PrecomputeAabb()
    {
        if (jurisdiction == null || jurisdiction.Polygon.Length == 0)
            return;

        Vector2[] worldPoints = GetWorldPolygon(jurisdiction);

        _jurisdictionAabb = new Rect2(worldPoints[0], Vector2.Zero);

        foreach (Vector2 p in worldPoints)
            _jurisdictionAabb = _jurisdictionAabb.Expand(p);
    }


    private Vector2[] GetWorldPolygon(Polygon2D poly)
    {
        if (poly == null || poly.Polygon.Length == 0)
            return Array.Empty<Vector2>();

        Vector2[] pts = poly.Polygon;
        Vector2[] world = new Vector2[pts.Length];

        for (int i = 0; i < pts.Length; i++)
            world[i] = poly.ToGlobal(pts[i]);

        return world;
    }


    private Vector2 ClampToBoundary(Vector2 worldPos)
    {
        if (centerBoundary == null)
            return worldPos;

        Vector2[] worldPoints = GetWorldPolygon(centerBoundary);

        if (Geometry2D.IsPointInPolygon(worldPos, worldPoints))
            return worldPos;

        Vector2 closest = worldPoints[0];
        float minDistSq = worldPos.DistanceSquaredTo(closest);

        for (int i = 0; i < worldPoints.Length; i++)
        {
            Vector2 a = worldPoints[i];
            Vector2 b = worldPoints[(i + 1) % worldPoints.Length];

            Vector2 candidate = Geometry2D.GetClosestPointToSegment(worldPos, a, b);
            float distSq = worldPos.DistanceSquaredTo(candidate);

            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closest = candidate;
            }
        }

        return closest;
    }


    // ============================================================
    // =======================  DEBUGGING  =========================
    // ============================================================

    private void VisualizeBoundary()
    {
        ClearMarkers();

        if (centerBoundary == null)
            return;

        Vector2 viewportSize = new Vector2(1920, 1080);
        Vector2 halfSize = viewportSize * 0.5f / zoomLevel;

        Vector2[] cornerOffsets = new Vector2[]
        {
            new Vector2( halfSize.X,  halfSize.Y ),
            new Vector2(-halfSize.X,  halfSize.Y ),
            new Vector2(-halfSize.X, -halfSize.Y ),
            new Vector2( halfSize.X, -halfSize.Y )
        };

        foreach (Vector2 point in centerBoundary.Polygon)
        {
            Vector2 worldPos = centerBoundary.ToGlobal(point);
            Line2D line = new Line2D();

            foreach (var offset in cornerOffsets)
                line.AddPoint(ToLocal(worldPos + offset));

            line.AddPoint(ToLocal(worldPos + cornerOffsets[0]));

            line.DefaultColor = Colors.White;
            line.Width = 15f;
            line.ZIndex = 100;

            AddChild(line);
            _debugMarkers.Add(line);
        }
    }

    private void ClearMarkers()
    {
        foreach (var marker in _debugMarkers)
        {
            if (GodotObject.IsInstanceValid(marker))
                marker.QueueFree();
        }

        _debugMarkers.Clear();
    }
}