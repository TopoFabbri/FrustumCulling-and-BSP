using UnityEngine;

/// <summary>
/// Debug draw tools
/// </summary>
public class Draw : MonoBehaviour
{
    private static float pointDestroyTime;
    private static Vector3 pointPos;
    
    public static Color Color { private get; set; } = Color.white;

    /// <summary>
    /// Draw a cube only edges
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    public static void WireCube(Vector3 center, Vector3 size)
    {
        Vector3 halfSize = size / 2f;

        Vector3 topLeftFront = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        Vector3 topRightFront = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        Vector3 bottomRightFront = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        Vector3 bottomLeftFront = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        Vector3 topLeftBack = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        Vector3 topRightBack = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
        Vector3 bottomRightBack = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        Vector3 bottomLeftBack = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);

        Debug.DrawLine(topLeftFront, topRightFront, Color.white);
        Debug.DrawLine(topRightFront, bottomRightFront, Color.white);
        Debug.DrawLine(bottomRightFront, bottomLeftFront, Color.white);
        Debug.DrawLine(bottomLeftFront, topLeftFront, Color.white);

        Debug.DrawLine(topLeftFront, topLeftBack, Color.white);
        Debug.DrawLine(topRightFront, topRightBack, Color.white);
        Debug.DrawLine(bottomRightFront, bottomRightBack, Color.white);
        Debug.DrawLine(bottomLeftFront, bottomLeftBack, Color.white);

        Debug.DrawLine(topLeftBack, topRightBack, Color.white);
        Debug.DrawLine(topRightBack, bottomRightBack, Color.white);
        Debug.DrawLine(bottomRightBack, bottomLeftBack, Color.white);
        Debug.DrawLine(bottomLeftBack, topLeftBack, Color.white);
    }

    /// <summary>
    /// Draw a cross to represent a point
    /// </summary>
    /// <param name="point"></param>
    /// <param name="size"></param>
    public static void Point(Vector3 point, float size)
    {
        Debug.DrawLine(point - Vector3.up * size / 2f, point + Vector3.up * size / 2f, Color);
        Debug.DrawLine(point - Vector3.left * size / 2f, point + Vector3.left * size / 2f, Color);
        Debug.DrawLine(point - Vector3.forward * size / 2f, point + Vector3.forward * size / 2f, Color);
    }

    /// <summary>
    /// Draw a line from a ray
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="color"></param>
    public static void Ray(Ray ray, Color color)
    {
        Debug.DrawRay(ray.origin, ray.direction, color);
    }

    /// <summary>
    /// Draw a line with length
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    /// <param name="length"></param>
    public static void Line(Vector3 start, Vector3 dir, float length)
    {
        Debug.DrawLine(start, start + dir * length, Color);
    }
    
    /// <summary>
    /// Draw a line with start and end
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static void Line(Vector3 start, Vector3 end)
    {
        Debug.DrawLine(start, end, Color);
    }
    
    /// <summary>
    /// Draw a plane on given position
    /// </summary>
    /// <param name="position">Position to draw the plane</param>
    /// <param name="normal">Normal of the plane</param>
    public static void Plane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
        
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        
        v3 = q * v3;
        
        var corner1 = position + v3;
        var corner3 = position - v3;
        
        Color = Color.green;
        Line(corner0, corner2);
        Line(corner1, corner3);
        Line(corner0, corner1);
        Line(corner1, corner2);
        Line(corner2, corner3);
        Line(corner3, corner0);
        
        Debug.DrawRay(position, normal, Color.red);
    }

}