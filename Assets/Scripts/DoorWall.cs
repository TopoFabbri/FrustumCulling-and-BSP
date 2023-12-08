using UnityEngine;

public class DoorWall : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private float doorPos;
    [SerializeField] private float doorWidth;

    [Header("Refs")]
    [SerializeField] private Transform[] wall;
    
    private Plane doorStartPlane;
    private Plane doorEndPlane;

    void OnDrawGizmos()
    {
        const float wallSize = 10;

        Vector3 scale = wall[0].localScale;
        scale.x = doorPos - doorWidth / 2;
        wall[0].localScale = scale;

        Vector3 pos = wall[0].localPosition;
        pos.x = wallSize / 2 - scale.x / 2;
        wall[0].localPosition = pos;

        scale = wall[1].localScale;
        scale.x = -doorPos - (doorWidth - wallSize * 2) / 2;
        wall[1].localScale = scale;

        pos = wall[1].localPosition;
        pos.x = -wallSize / 2 + scale.x / 2;
        wall[1].localPosition = pos;
        
        Door();
    }

    private void Door()
    {
        Vector3 doorStart = wall[0].position - wall[0].right * wall[0].localScale.x / 2;
        Vector3 doorEnd = wall[1].position + wall[1].right * wall[1].localScale.x / 2;

        doorStartPlane = new Plane(doorEnd - doorStart, doorStart);
        doorEndPlane = new Plane(doorStart - doorEnd, doorEnd);
    }
    
    public bool PointInDoor(Vector3 point)
    {
        Vector3 doorStart = wall[0].position - wall[0].right * wall[0].localScale.x / 2;
        Vector3 doorEnd = wall[1].position + wall[1].right * wall[1].localScale.x / 2;

        bool passed = doorStartPlane.GetSide(point) && doorEndPlane.GetSide(point);
        
        FrustumCulling.DrawPlane(doorStart, doorStartPlane.normal);
        FrustumCulling.DrawPlane(doorEnd, doorEndPlane.normal);
        
        Draw.Point(point, .5f);
        
        return passed;
    }
}