using System.Collections.Generic;
using UnityEngine;

public static class PhysicsHelper
{
    public static Vector3 GetCenter(ICollection<Vector3> points)
    {
        var center = Vector3.zero;

        foreach(var point in points)
            center += point / points.Count;

        return center;
    }
}
