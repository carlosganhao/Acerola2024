using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Waypoint : MonoBehaviour
{
    public Vector3 Position { get => transform.position; }
    public Waypoint[] ConnectedWaypoits = new Waypoint[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        for (int i = 0; i < ConnectedWaypoits.Length; i++)
        {
            var waypoint = ConnectedWaypoits[i];
            if (waypoint is not null)
            {
                if(waypoint.ConnectedWaypoits[OpositeOrientation(i)] is null)
                {
                    waypoint.ConnectedWaypoits[OpositeOrientation(i)] = this;
                }
            }
        }
    }

    private int OpositeOrientation(int direction) => (direction + 2)%4;

    private Vector3 DirectionVector(int direction) => DirectionVector((Orientation)direction);
    private Vector3 DirectionVector(Orientation direction) => direction switch
    {
        Orientation.North => Vector3.forward,
        Orientation.East => Vector3.right,
        Orientation.South => Vector3.back,
        Orientation.West => Vector3.left,
        _ => Vector3.zero,
    };

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < ConnectedWaypoits.Length; i++)
        {
            var waypoint = ConnectedWaypoits[i];
            if (waypoint is not null)
            {
                Gizmos.DrawLine(transform.position, waypoint.Position);
                Gizmos.DrawCube(transform.position + DirectionVector(i)*2, new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }
}
