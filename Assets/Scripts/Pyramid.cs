using UnityEngine;

public class Pyramid : MonoBehaviour
{
    [SerializeField] private Vector3[] vector;
    [SerializeField] private Vector3 pyrOffset;
    [SerializeField] private Vector3[] pyramid;

    [SerializeField] private float facesArea;

    private void OnDrawGizmos()
    {
        vector[1] = GetPerpendicular(vector[0]);

        vector[2] = CrossProduct(vector[0], vector[1]);

        Draw.Color = Color.red;
        Draw.Line(Vector3.zero, vector[0]);

        Draw.Color = Color.green;
        Draw.Line(Vector3.zero, vector[1]);

        Draw.Color = Color.blue;
        Draw.Line(Vector3.zero, vector[2]);

        float shortestVec = GetShortest(vector);

        for (int i = 0; i < pyramid.Length; i++)
            pyramid[i] = SetLength(vector[i], shortestVec);

        Draw.Color = Color.red;
        Draw.Line(Vector3.zero + pyrOffset, pyramid[0] + pyrOffset);

        Draw.Color = Color.green;
        Draw.Line(Vector3.zero + pyrOffset, pyramid[1] + pyrOffset);

        Draw.Color = Color.blue;
        Draw.Line(Vector3.zero + pyrOffset, pyramid[2] + pyrOffset);

        CalculateArea();
    }

    private static Vector3 GetPerpendicular(Vector3 vec)
    {
        return new Vector3(vec.y, -vec.x, 0);
    }

    private static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
    {
        Vector3 cross;

        cross.x = v1.y * v2.z - v1.z * v2.y;
        cross.y = v1.z * v2.x - v1.x * v2.z;
        cross.z = v1.x * v2.y - v1.y * v2.x;

        return cross;
    }

    private static float GetShortest(Vector3[] vec)
    {
        float length = vec[0].magnitude;

        for (int i = 1; i < vec.Length; i++)
        {
            if (vec[i].magnitude < length)
                length = vec[i].magnitude;
        }

        return length;
    }

    private static Vector3 SetLength(Vector3 vec, float length)
    {
        return vec.normalized * length;
    }

    private void CalculateArea()
    {
        float area1 = GetTriangleArea(Vector3.zero, pyramid[0], pyramid[1]);
        float area2 = GetTriangleArea(Vector3.zero, pyramid[0], pyramid[2]);
        float area3 = GetTriangleArea(Vector3.zero, pyramid[1], pyramid[2]);

        facesArea = area1 + area2 + area3;
    }

    private static float GetTriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
            float side1 = Vector3.Distance(a, b);
            float side2 = Vector3.Distance(b, c);
            float side3 = Vector3.Distance(c, a);

            float s = (side1 + side2 + side3) / 2;

            float area = Mathf.Sqrt(s * (s - side1) * (s - side2) * (s - side3));

            return area;
    }
}