using UnityEngine;

public class DoorWall : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private float doorPos;
    [SerializeField] private float doorWidth;

    [Header("Refs")]
    [SerializeField] private Transform[] wall;

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
    }
}