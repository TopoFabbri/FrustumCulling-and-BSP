using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frustum : MonoBehaviour
{
    struct Plane
    {
        private Vector2 point;
        private Vector2 normal;
    }

    [SerializeField] private GameObject farPlane;
    [SerializeField] private GameObject nearPlane;
    [SerializeField] private Transform fovTransform;

    [Header("ASPECT")] 
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("FOV")] 
    [SerializeField] private float farPlaneDistance;


    private Plane rightWall;
    private Plane leftWall;

    private Vector3 nearLeftUp;
    private Vector3 nearRightUp;
    private Vector3 nearLeftLow;
    private Vector3 nearRightLow;

    private Vector3 farLeftUp;
    private Vector3 farRightUp;
    private Vector3 farLeftLow;
    private Vector3 farRightLow;

    void OnDrawGizmos()
    {
        nearLeftUp = GetLeftUpVertex(nearPlane.GetComponent<MeshFilter>());
        farLeftUp = GetLeftUpVertex(farPlane.GetComponent<MeshFilter>());

        nearRightUp = GetRightUpVertex(nearPlane.GetComponent<MeshFilter>());
        farRightUp = GetRightUpVertex(farPlane.GetComponent<MeshFilter>());

        nearLeftLow = GetLeftLowVertex(nearPlane.GetComponent<MeshFilter>());
        farLeftLow = GetLeftLowVertex(farPlane.GetComponent<MeshFilter>());

        nearRightLow = GetRightLowVertex(nearPlane.GetComponent<MeshFilter>());
        farRightLow = GetRightLowVertex(farPlane.GetComponent<MeshFilter>());

        UpdateFov();

        Gizmos.color = Color.red;

        // Draw frustum sides
        Gizmos.DrawLine(nearLeftUp, farLeftUp);
        Gizmos.DrawLine(nearRightUp, farRightUp);
        Gizmos.DrawLine(nearLeftLow, farLeftLow);
        Gizmos.DrawLine(nearRightLow, farRightLow);

        // Draw near plane
        Gizmos.DrawLine(nearLeftUp, nearRightUp);
        Gizmos.DrawLine(nearRightUp, nearRightLow);
        Gizmos.DrawLine(nearRightLow, nearLeftLow);
        Gizmos.DrawLine(nearLeftLow, nearLeftUp);

        // Draw far plane
        Gizmos.DrawLine(farLeftUp, farRightUp);
        Gizmos.DrawLine(farRightUp, farRightLow);
        Gizmos.DrawLine(farRightLow, farLeftLow);
        Gizmos.DrawLine(farLeftLow, farLeftUp);
    }

    Vector3 GetLeftUpVertex(MeshFilter plane)
    {
        Vector3 leftUpVertex = new Vector3(0, 0, 0);

        for (int i = 0; i < plane.sharedMesh.vertices.Length; i++)
        {
            if (plane.sharedMesh.vertices[i].x < leftUpVertex.x || plane.sharedMesh.vertices[i].y < leftUpVertex.y || plane.sharedMesh.vertices[i].z < leftUpVertex.z)
                leftUpVertex = plane.sharedMesh.vertices[i];
        }

        leftUpVertex = plane.transform.TransformPoint(leftUpVertex);

        return leftUpVertex;
    }

    Vector3 GetRightUpVertex(MeshFilter plane)
    {
        Vector3 rightUpVertex = new Vector3(0, 0, 0);

        for (int i = 0; i < plane.sharedMesh.vertices.Length; i++)
        {
            if (plane.sharedMesh.vertices[i].x > rightUpVertex.x || plane.sharedMesh.vertices[i].y < rightUpVertex.y || plane.sharedMesh.vertices[i].z < rightUpVertex.z)
                rightUpVertex = plane.sharedMesh.vertices[i];
        }

        rightUpVertex = plane.transform.TransformPoint(rightUpVertex);

        return rightUpVertex;
    }

    Vector3 GetLeftLowVertex(MeshFilter plane)
    {
        Vector3 leftLowVertex = new Vector3(0, 0, 0);

        for (int i = 0; i < plane.sharedMesh.vertices.Length; i++)
        {
            if (plane.sharedMesh.vertices[i].x < leftLowVertex.x || plane.sharedMesh.vertices[i].y < leftLowVertex.y || plane.sharedMesh.vertices[i].z > leftLowVertex.z)
                leftLowVertex = plane.sharedMesh.vertices[i];
        }

        leftLowVertex = plane.transform.TransformPoint(leftLowVertex);

        return leftLowVertex;
    }

    Vector3 GetRightLowVertex(MeshFilter plane)
    {
        Vector3 rightLowVertex = new Vector3(0, 0, 0);

        for (int i = 0; i < plane.sharedMesh.vertices.Length; i++)
        {
            if (plane.sharedMesh.vertices[i].x > rightLowVertex.x || plane.sharedMesh.vertices[i].y < rightLowVertex.y || plane.sharedMesh.vertices[i].z > rightLowVertex.z)
                rightLowVertex = plane.sharedMesh.vertices[i];
        }

        rightLowVertex = plane.transform.TransformPoint(rightLowVertex);

        return rightLowVertex;
    }

    void UpdateFov()
    {
        farPlane.transform.localPosition = new Vector3(farPlane.transform.localPosition.x,
            farPlane.transform.localPosition.y, farPlaneDistance);

        fovTransform.localScale = new Vector3(1, height/width, fovTransform.localScale.z);

        farPlane.transform.localScale =
            new Vector3(farPlaneDistance / 5, farPlane.transform.localScale.z, farPlaneDistance / 5);
    }

    public Vector3[] GetFarPlane()
    {
        return new Vector3[]{farLeftUp, farRightUp, farLeftLow, farRightLow};
    }

    public Vector3[] GetNearPlane()
    {
        return new Vector3[]{nearLeftUp, nearRightUp, nearLeftLow, nearRightLow};
    }
}