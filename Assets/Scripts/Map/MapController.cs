using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    [SerializeField] private List<Waypoint> _waypoints = new List<Waypoint>();

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Waypoint GetClosestWaypointToPoint(Vector3 position)
    {
        var possibleWayPoints = _waypoints.Where(w => !Physics.Raycast(position, w.Position - position, 100, LayerMask.NameToLayer("Terrain")));
        float distance = Mathf.Infinity;
        Waypoint result = null;

        foreach (var waypoint in possibleWayPoints)
        {
            float newDistance = Vector3.Distance(position, waypoint.Position);
            if(newDistance < distance)
            {
                distance = newDistance;
                result = waypoint;
            }
        }

        return result;
    }
}

public enum Orientation
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
