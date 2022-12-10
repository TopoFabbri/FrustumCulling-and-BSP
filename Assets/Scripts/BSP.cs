using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    struct Line
    {
        public Vector3 start;
        public Vector3 end;
        public Vector3 dir;
        public int index;

        public void Draw() { Debug.DrawLine(start, end); }
    }

    [SerializeField] private Room[] rooms;
    [SerializeField] private float lineLength;
    [SerializeField] private float iterationFreq;

    private Line[] lines = new Line[11];

    void Update()
    {
    }

    void CheckRooms()
    {
        Vector3 endPos = transform.position + transform.forward * lineLength - transform.right * 20 * 5;

        //for (int i = 0; i < lines.Length; i++)
        //{
        //    lines[i].start = transform.position;
        //    lines[i].end = endPos;
        //    lines[i].dir = Vector3.Normalize(lines[i].end - lines[i].start);
        //    endPos += transform.right * 20;
        //}

        for (int i = 0; i < lines.Length; i++)
        {
            CheckLine(lines[i]);
        }
    }

    void CheckLine(Line line)
    {
        for (int i = 0; i < iterationFreq; i++)
        {
            for (int j = 1; j < lineLength; j++)
            {
                Vector3 prevPoint = line.start + line.dir * (j - 1);
                Vector3 point = line.start + line.dir * j;

                CheckPoint(point);
            }
        }
    }

    void CheckPoint(Vector3 point)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (IsAdjacent(rooms[i]))
            {
                if (rooms[i].PointInRoom(point))
                    rooms[i].SetAsContiguous();
            }
        }
    }

    public Vector3 CheckRayPlaneCollision(Vector3 a, Vector3 b, Plane currentPlane)
    {
        Vector3 ba = b - a;
        float nDotA = Vector3.Dot(currentPlane.normal, a);
        float nDotBA = Vector3.Dot(currentPlane.normal, ba);

        return a + (((currentPlane.distance - nDotA) / nDotBA) * ba);
    }

    bool IsAdjacent(Room room)
    {
        int[] adjId = room.GetAdjs();

        for (int i = 0; i < adjId.Length; i++)
        {
            if (rooms[adjId[i]].IsContiguous() || rooms[adjId[i]].IsMain())
                return true;
        }

        return false;
    }

    bool CheckContiguousRoom(Room room, int id)
    {
        int[] adys = room.GetAdjs();

        for (int i = 0; i < adys.Length; i++)
        {
            if (id == adys[i])
                return true;
        }

        return false;
    }

    Room GetMainRoom()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].IsMain())
                return rooms[i];
        }

        return rooms[0];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 endPos = transform.position + transform.forward * lineLength - transform.right * 20 * 5;

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].start = transform.position;
            lines[i].end = endPos;
            lines[i].dir = Vector3.Normalize(lines[i].end - lines[i].start);
            lines[i].index = i;
            endPos += transform.right * 20;
        }

        for (int k = 0; k < rooms.Length; k++)
        {
            rooms[k].Reset();
            rooms[k].SetAsMain(rooms[k].PointInRoom(transform.position));
        }

        CheckRooms();

        for (int i = 0; i < lines.Length; i++)
        {
            Gizmos.DrawLine(lines[i].start, lines[i].end);
        }
    }
}