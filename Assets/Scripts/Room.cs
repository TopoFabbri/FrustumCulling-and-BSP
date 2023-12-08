using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Room : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int[] adyId;
    [SerializeField] private GameObject floor;
    [SerializeField] private DoorWall[] doorWalls;

    MeshRenderer[] mesh;
    private bool mainRoom = false;
    private bool isContiguous = false;
    private bool onSight = false;

    private Plane[] walls = new Plane[4];

    void Start()
    {
        mesh = GetComponentsInChildren<MeshRenderer>();
    }

    public void UpdateRoom()
    {
        if (mesh == null)
            return;
        
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

    public void SetAsContiguous(bool contiguous = true)
    {
        if (!mainRoom)
            isContiguous = contiguous;
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

    public bool HasRayPassed(Vector3 intersection)
    {
        int closest = 0;

        for (int i = 0; i < doorWalls.Length; i++)
        {
            if (Vector3.Distance(doorWalls[i].transform.position, intersection) < Vector3.Distance(doorWalls[closest].transform.position, intersection))
                closest = i;
        }

        return doorWalls[closest].PointInDoor(intersection);
    }

    public void Reset()
    {
        mainRoom = false;
        isContiguous = false;
    }
}