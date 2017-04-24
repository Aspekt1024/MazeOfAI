using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLine {

    private const float verticalLineGradient = float.MaxValue;

    private float gradient;
    private float yIntercept;
    private Vector2 pointOnLine1;
    private Vector2 pointonLine2;

    private float gradientPerpendicular;

    private bool approachSide;

    public PathLine(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
            gradientPerpendicular = verticalLineGradient;
        else
            gradientPerpendicular = dy / dx;

        if (gradientPerpendicular == 0)
            gradient = verticalLineGradient;
        else
            gradient = -1 / gradientPerpendicular;

        yIntercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine1 = pointOnLine;
        pointonLine2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    private bool GetSide(Vector2 p)
    {
        // TODO learn this later.
        return ((p.x - pointOnLine1.x) * (pointonLine2.y - pointOnLine1.y)) > ((p.y - pointOnLine1.y) * (pointonLine2.x - pointOnLine1.x));
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine1.x, 0, pointOnLine1.y) + Vector3.up * 0.5f;
        Gizmos.DrawLine(lineCenter - lineDirection * length / 2, lineCenter + lineDirection * length / 2);
    }

    public float DistanceFromPoint(Vector2 point)
    {
        float yIntPerp = point.y - gradientPerpendicular * point.x;
        float intersectX = (yIntPerp - yIntercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + yIntercept;
        return Vector2.Distance(point, new Vector2(intersectX, intersectY));
    }
}
