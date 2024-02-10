using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float speed = 5f;
    private List<Vector3> pathPoints;
    private int currentPointIndex = 0;

    public void SetPath(List<Vector3> points, bool loop)
    {
        pathPoints = points;
        if (loop && points.Count > 1)
        {
            pathPoints.Add(points[0]); // Add the first point at the end to loop
        }
        currentPointIndex = 0;
        if (pathPoints.Count > 0)
        {
            transform.position = pathPoints[currentPointIndex]; // Move to the first point
        }
    }

    private void Update()
    {
        if (pathPoints == null || pathPoints.Count == 0) return;

        Vector3 targetPoint = pathPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (transform.position == targetPoint)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Count)
            {
                currentPointIndex = 0; // Loop back to the start if looping is enabled
                if (pathPoints.Count > 1 && pathPoints[pathPoints.Count - 1] == pathPoints[0])
                {
                    // If the last point is the same as the first (looping), skip to the second point
                    currentPointIndex = 1;
                }
            }
        }
    }
}
