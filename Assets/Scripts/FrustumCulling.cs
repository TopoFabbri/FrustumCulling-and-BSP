using Unity.VisualScripting;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    [SerializeField] private Frustum frustum;
    [SerializeField] private GameObject[] objects;

    private Vector3[] farPlaneCorners;
    private Vector3[] nearPlaneCorners;

    private Plane leftPlane;
    private Plane rightPlane;
    private Plane topPlane;
    private Plane lowPlane;
    private Plane nearPlane;
    private Plane farPlane;

    private void OnDrawGizmos()
    {
        farPlaneCorners = frustum.GetFarPlane();
        nearPlaneCorners = frustum.GetNearPlane();

        leftPlane = new Plane(farPlaneCorners[0], farPlaneCorners[2], nearPlaneCorners[0]);
        rightPlane = new Plane(farPlaneCorners[1], farPlaneCorners[3], nearPlaneCorners[1]);
        topPlane = new Plane(farPlaneCorners[0], farPlaneCorners[1], nearPlaneCorners[0]);
        lowPlane = new Plane(farPlaneCorners[2], farPlaneCorners[3], nearPlaneCorners[2]);
        nearPlane = new Plane(nearPlaneCorners[0], nearPlaneCorners[1], nearPlaneCorners[2]);
        farPlane = new Plane(farPlaneCorners[0], farPlaneCorners[1], farPlaneCorners[2]);

        rightPlane.Flip();
        topPlane.Flip();
        nearPlane.Flip();

        CheckObjects();
    }

    public void DrawPlane(Vector3 position, Vector3 normal)
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
        
        Draw.Color = Color.green;
        Draw.Line(corner0, corner2);
        Draw.Line(corner1, corner3);
        Draw.Line(corner0, corner1);
        Draw.Line(corner1, corner2);
        Draw.Line(corner2, corner3);
        Draw.Line(corner3, corner0);
        
        Debug.DrawRay(position, normal, Color.red);
    }

    private void CheckObjects()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject curObj = objects[i].GameObject();

            if (!curObj.CompareTag("Obj"))
                continue;

            curObj.SetActive(false);

            Vector3[] boundingBox = GenerateBox(curObj);

            DrawBox(boundingBox);

            bool shouldBeChecked = false;

            for (int j = 0; j < boundingBox.Length; j++)
            {
                if (PointInFrustum(boundingBox[j]))
                {
                    shouldBeChecked = true;

                    Gizmos.DrawSphere(boundingBox[j], 0.1f);
                }
            }

            if (!shouldBeChecked)
                continue;

            for (int j = 0; j < curObj.GetComponent<MeshFilter>().sharedMesh.vertices.Length; j++)
            {
                Vector3 vertex = curObj.GetComponent<MeshFilter>().sharedMesh.vertices[j];

                if (PointInFrustum(curObj.transform.TransformPoint(vertex)))
                {
                    curObj.SetActive(true);
                    break;
                }
            }
        }
    }

    private bool PointInFrustum(Vector3 point)
    {
        bool isVisible = leftPlane.GetSide(point) && rightPlane.GetSide(point) && topPlane.GetSide(point) &&
                         lowPlane.GetSide(point) && farPlane.GetSide(point) && nearPlane.GetSide(point);

        return isVisible;
    }

    private static Vector3[] GenerateBox(GameObject obj)
    {
        Vector3[] vertices = obj.GetComponent<MeshFilter>().sharedMesh.vertices;

        float minX = obj.transform.TransformPoint(vertices[0]).x;
        float maxX = obj.transform.TransformPoint(vertices[0]).x;

        float minY = obj.transform.TransformPoint(vertices[0]).y;
        float maxY = obj.transform.TransformPoint(vertices[0]).y;

        float minZ = obj.transform.TransformPoint(vertices[0]).z;
        float maxZ = obj.transform.TransformPoint(vertices[0]).z;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (obj.transform.TransformPoint(vertices[i]).x < minX)
                minX = obj.transform.TransformPoint(vertices[i]).x;

            if (obj.transform.TransformPoint(vertices[i]).x > maxX)
                maxX = obj.transform.TransformPoint(vertices[i]).x;

            if (obj.transform.TransformPoint(vertices[i]).y < minY)
                minY = obj.transform.TransformPoint(vertices[i]).y;

            if (obj.transform.TransformPoint(vertices[i]).y > maxY)
                maxY = obj.transform.TransformPoint(vertices[i]).y;

            if (obj.transform.TransformPoint(vertices[i]).z < minZ)
                minZ = obj.transform.TransformPoint(vertices[i]).z;

            if (obj.transform.TransformPoint(vertices[i]).z > maxZ)
                maxZ = obj.transform.TransformPoint(vertices[i]).z;
        }

        Vector3[] boxVertices = new Vector3[8];

        boxVertices[0] = new Vector3(minX, minY, minZ);
        boxVertices[1] = new Vector3(maxX, minY, minZ);
        boxVertices[2] = new Vector3(minX, maxY, minZ);
        boxVertices[3] = new Vector3(maxX, maxY, minZ);
        boxVertices[4] = new Vector3(minX, minY, maxZ);
        boxVertices[5] = new Vector3(maxX, minY, maxZ);
        boxVertices[6] = new Vector3(minX, maxY, maxZ);
        boxVertices[7] = new Vector3(maxX, maxY, maxZ);

        return boxVertices;
    }

    private static void DrawBox(Vector3[] box)
    {
        Draw.Color = Color.blue;

        Draw.Line(box[0], box[1]);
        Draw.Line(box[0], box[2]);
        Draw.Line(box[1], box[3]);
        Draw.Line(box[2], box[3]);
        Draw.Line(box[0], box[4]);
        Draw.Line(box[1], box[5]);
        Draw.Line(box[2], box[6]);
        Draw.Line(box[3], box[7]);
        Draw.Line(box[4], box[5]);
        Draw.Line(box[4], box[6]);
        Draw.Line(box[5], box[7]);
        Draw.Line(box[6], box[7]);

        Draw.Color = Color.green;
    }
}