using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {

    public readonly Vector3[] lookPoints;
    public readonly PathLine[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] waypoints, Vector3 startPos, float turnDist, float stoppingDist)
    {
        lookPoints = waypoints;
        turnBoundaries = new PathLine[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = V3ToV2(startPos);
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = V3ToV2(lookPoints[i]);
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDist;
            turnBoundaries[i] = new PathLine(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDist);
            previousPoint = turnBoundaryPoint;
        }

        float distFromEndPoint = 0;
        for (int i = lookPoints.Length - 1; i > 0; i--)
        {
            distFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if (distFromEndPoint > stoppingDist)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    private Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 point in lookPoints)
        {
            Gizmos.DrawCube(point + Vector3.up * 0.5f, Vector3.one * 0.2f);
        }

        Gizmos.color = Color.white;
        foreach (PathLine line in turnBoundaries)
        {
            line.DrawWithGizmos(1);
        }
    }
}
