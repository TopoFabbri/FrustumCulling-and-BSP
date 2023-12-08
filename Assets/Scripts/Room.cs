using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Room : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int[] adyId;
    [SerializeField] private GameObject floor;

    MeshRenderer[] mesh;
    private bool mainRoom = false;
    private bool isContiguous = false;
    private bool onSight = false;

    private Plane[] walls = new Plane[4];

    void Start()
    {
        mesh = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (mainRoom || isContiguous)
            ShowRoom();
        else
            HideRoom();

        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i].enabled = onSight;
        }
    }

    void OnDrawGizmos()
    {
        Vector3[] corners = GetVertices();

        walls[0] = new Plane(corners[0], corners[1], corners[0] + Vector3.up);
        walls[1] = new Plane(corners[1], corners[3], corners[1] + Vector3.up);
        walls[2] = new Plane(corners[2], corners[0], corners[2] + Vector3.up);
        walls[3] = new Plane(corners[3], corners[2], corners[3] + Vector3.up);

        //DrawPlane(corners[0], walls[0].normal);
        //DrawPlane(corners[1], walls[1].normal);
        //DrawPlane(corners[2], walls[2].normal);
        //DrawPlane(corners[3], walls[3].normal);
    }

    Vector3[] GetVertices()
    {
        MeshFilter mesh = floor.GetComponent<MeshFilter>();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = mesh.sharedMesh.vertices[0];
        vertices[1] = mesh.sharedMesh.vertices[0];
        vertices[2] = mesh.sharedMesh.vertices[0];
        vertices[3] = mesh.sharedMesh.vertices[0];

        // Left up vertex
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; i++)
        {
            if (mesh.sharedMesh.vertices[i].x < vertices[0].x || mesh.sharedMesh.vertices[i].y > vertices[0].y)
                vertices[0] = mesh.sharedMesh.vertices[i];

            // Right up vertex
            if (mesh.sharedMesh.vertices[i].x > vertices[1].x || mesh.sharedMesh.vertices[i].y > vertices[1].y)
                vertices[1] = mesh.sharedMesh.vertices[i];

            // Left down vertex
            if (mesh.sharedMesh.vertices[i].x < vertices[2].x || mesh.sharedMesh.vertices[i].y < vertices[2].y)
                vertices[2] = mesh.sharedMesh.vertices[i];

            // Right down vertex
            if (mesh.sharedMesh.vertices[i].x > vertices[3].x || mesh.sharedMesh.vertices[i].y < vertices[3].y)
                vertices[3] = mesh.sharedMesh.vertices[i];
        }

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = floor.transform.TransformPoint(vertices[i]);

        return vertices;
    }

    public Plane GetWall(int index)
    {
        return walls[index];
    }

    public bool PointInRoom(Vector3 point)
    {
        bool pointInRoom = true;

        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].GetSide(point))
                pointInRoom = false;
        }

        return pointInRoom;
    }

    public void SetAsMain(bool isMain)
    {
        mainRoom = isMain;

        if (isMain)
            ShowRoom();
    }

    public bool IsMain()
    {
        return mainRoom;
    }

    public void SetAsContiguous()
    {
        if (!mainRoom)
            isContiguous = true;
        ShowRoom();
    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }

    public void HideRoom()
    {
        onSight = false;
    }

    public void ShowRoom()
    {
        onSight = true;
    }

    public int GetId()
    {
        return id;
    }

    public int[] GetAdjs()
    {
        return adyId;
    }

    public bool IsContiguous()
    {
        return isContiguous;
    }

    public bool HasRayPassed(Vector3 prev, Vector3 cur)
    {
        return true;
    }

    public void Reset()
    {
        mainRoom = false;
        isContiguous = false;
    }
}