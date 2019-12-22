using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
    public static Vector2 PointOnCircle(float yAng, float r) {
        var x = r * Mathf.Cos(yAng);
        var y = r * Mathf.Sin(yAng);
        return new Vector2(x, y);
    }

    public static Vector3 PointOnSphere(float yAng, float xAng, float r) {
        float x = r * Mathf.Cos(yAng) * Mathf.Sin(xAng);
        float y = r * Mathf.Sin(yAng) * Mathf.Sin(xAng);
        float z = r * Mathf.Cos(xAng);
        return new Vector3(x, y, z);
    }
}
