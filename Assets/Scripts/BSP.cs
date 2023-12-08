using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    struct Line
    {
        public Vector3 start;
        public Vector3 dir;
        public Vector3 end;

        public void Draw()
        {
            Debug.DrawLine(start, end);
        }
    }

    [SerializeField] private Room[] rooms;
    [SerializeField] private float lineLength;
    [SerializeField] private float fov;
    [SerializeField] private int lineQty;
    [SerializeField] private float iterationFreq;

    private Line[] lines = new Line[11];

    void CheckRooms()
    {
        ResetRooms();

        int[] firstContiguousIds = GetMainRoom().GetAdjs();

        for (int i = 0; i < firstContiguousIds.Length; i++)
            rooms[firstContiguousIds[i]].SetAsContiguous();

        Vector3 endPos = transform.position + transform.forward * lineLength - transform.right * 20 * 5;

        for (int i = 0; i < lines.Length; i++)
            CheckLine(ref lines[i]);

        foreach (Room room in rooms)
            room.UpdateRoom();
    }

    void CheckLine(ref Line line)
    {
        for (int j = 1; j < lineLength; j++)
        {
            Vector3 prevPoint = line.start + line.dir * (j - 1);
            Vector3 point = line.start + line.dir * j;

            int prevRoomId = CheckPoint(prevPoint);
            int roomId = CheckPoint(point);

            if (prevRoomId == roomId && roomId != -1)
            {
                rooms[roomId].ShowRoom();

                foreach (int adj in rooms[roomId].GetAdjs())
                    rooms[adj].SetAsContiguous();

                continue;
            }

            if (prevRoomId == -1)
                continue;

            Vector3 intersection = GetIntersection(prevPoint, point, prevRoomId);

            if (roomId == -1)
            {
                line.end = intersection;
                return;
            }

            if (rooms[prevRoomId].HasRayPassed(intersection)) continue;

            line.end = intersection;
            return;
        }
    }

    private Vector3 GetIntersection(Vector3 prevPoint, Vector3 point, int prevRoomId)
    {
        Vector3[] points = new Vector3[10];
        Vector3 newLine = point - prevPoint;

        for (int i = 0; i < points.Length; i++)
            points[i] = prevPoint + newLine.normalized * i * (newLine.magnitude / 10f);

        for (int i = 0; i < points.Length; i++)
        {
            if (CheckPoint(points[i]) == prevRoomId)
                continue;

            return points[i] - newLine.normalized * (newLine.magnitude / 10f);
        }

        return prevPoint;
    }

    int CheckPoint(Vector3 point)
    {
        foreach (Room room in rooms)
        {
            if (!room.IsContiguous() && !room.IsMain()) continue;
            if (!room.PointInRoom(point)) continue;

            return room.GetId();
        }

        return -1;
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

        Vector3 endPos = transform.position + transform.forward * lineLength - transform.right * fov / 2;

        lines = new Line[lineQty];
        
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].start = transform.position;
            lines[i].end = endPos + transform.right * (fov / (lines.Length - 1)) * i;
            lines[i].dir = Vector3.Normalize(lines[i].end - lines[i].start);
            lines[i].end = lines[i].start + lines[i].dir * lineLength;
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

    private void ResetRooms()
    {
        foreach (Room room in rooms)
        {
            room.HideRoom();
            room.SetAsMain(room.PointInRoom(transform.position));
            room.SetAsContiguous(false);
        }
    }
}