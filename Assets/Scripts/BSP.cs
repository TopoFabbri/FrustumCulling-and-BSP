using UnityEngine;

public class BSP : MonoBehaviour
{
    private struct Line
    {
        public Vector3 start;
        public Vector3 dir;
        public Vector3 end;
    }

    [SerializeField] private Room[] rooms;

    [Header("Settings:")] [SerializeField] private float lineLength;
    [SerializeField] private float fov;
    [SerializeField] private int lineQty;
    [SerializeField] private float iterationFreq;
    [SerializeField] private bool drawLines;
    [SerializeField] private bool drawPoints;
    [SerializeField] private bool drawIntersections;

    private Line[] lines;

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
        int pointQty = (int)(lineLength * iterationFreq);

        for (int j = 1; j < pointQty; j++)
        {
            Vector3 prevPoint = line.start + line.dir * ((j - 1) / iterationFreq);
            Vector3 point = line.start + line.dir * (j / iterationFreq);

            Draw.Color = Color.black;

            if (drawPoints)
                Draw.Point(point, 0.1f);

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

            Draw.Color = Color.red;

            if (drawIntersections)
                Draw.Point(intersection, .2f);

            if (roomId == -1)
            {
                line.end = intersection;
                return;
            }

            if (rooms[prevRoomId].HasRayPassed(intersection))
            {
                Draw.Color = Color.green;

                if (drawIntersections)
                    Draw.Point(intersection, .2f);

                continue;
            }

            line.end = intersection;
            return;
        }
    }

    private Vector3 GetIntersection(Vector3 prevPoint, Vector3 point, int prevRoomId)
    {
        Vector3[] points = new Vector3[10];
        Vector3 newLine = point - prevPoint;

        Draw.Color = Color.black;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = prevPoint + newLine.normalized * i * (newLine.magnitude / 10f);
            
            if (drawPoints)
                Draw.Point(points[i], 0.1f);
        }

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
        Gizmos.color = Color.white;

        Vector3 endPos = transform.position + transform.forward * lineLength - transform.right * fov / 2;

        lines = new Line[lineQty];

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].start = transform.position;
            lines[i].end = endPos + transform.right * (fov / (lines.Length - 1)) * i;
            lines[i].dir = Vector3.Normalize(lines[i].end - lines[i].start);
            lines[i].end = lines[i].start + lines[i].dir * lineLength;
        }

        foreach (Room room in rooms)
        {
            room.Reset();
            room.SetAsMain(room.PointInRoom(transform.position));
        }

        CheckRooms();

        if (drawLines)
        {
            for (int i = 0; i < lines.Length; i++)
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