using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    public Vector3 PlayerPosition { get => _playerTransform.position; }
    [SerializeField] private Transform _playerTransform;
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
        var possibleWayPoints = _waypoints.Where(w => !Physics.Raycast(position, (w.Position - position).normalized, out RaycastHit hit, Vector3.Distance(position, w.Position)));

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

    public Waypoint GetClosestWaypointToPointFromOther(Vector3 position, Vector3 from)
    {
        var possibleWayPoints = _waypoints.Where(w => !Physics.Raycast(from, (w.Position - from).normalized, out RaycastHit hit, Vector3.Distance(from, w.Position)));
        
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

    public Stack<Waypoint> GetPathToDestination(Waypoint to, Waypoint from)
    {
        var result = new Stack<Waypoint>();
        var allWaypoints = new List<PathWaypoint>();
        var possibleWayPoints = new Queue<PathWaypoint>();

        var startingPoint = new PathWaypoint(from)
            {
                searched = true
            };
        allWaypoints.Add(startingPoint);
        possibleWayPoints.Enqueue(startingPoint);
        PathWaypoint resultWaypoint = null;

        while(possibleWayPoints.Count() > 0)
        {
            var testingWaypoint = possibleWayPoints.Dequeue();
            
            if(testingWaypoint.position == to.Position)
            {
                resultWaypoint = testingWaypoint;
                break;
            }

            foreach(var waypoint in testingWaypoint.connectedWaypoints)
            {
                if(waypoint is not null && !allWaypoints.Any(w => w.position == waypoint.Position && w.searched))
                {
                    var pathWaypoint = new PathWaypoint(waypoint, testingWaypoint)
                        {
                            searched = true,
                        };
                    possibleWayPoints.Enqueue(pathWaypoint);
                }
            }
        }

        while(resultWaypoint is not null)
        {
            Debug.Log($"Pathfinding Path: {resultWaypoint.waypoint.gameObject}");
            result.Push(resultWaypoint.waypoint);
            resultWaypoint = resultWaypoint.parent;
        }

        return result;
    }

    public float GetDistanceToPlayer(Vector3 position)
    {
        return Vector3.Distance(_playerTransform.position, position);
    }

    public float GetDistanceToPlayer(Waypoint waypoint)
    {
        return GetDistanceToPlayer(waypoint.Position);
    }

    public bool IsPlayerVisible(Vector3 position)
    {
        if(Physics.Raycast(position, _playerTransform.position - position, out RaycastHit hit, GetDistanceToPlayer(position), LayerMask.GetMask("Player", "Terrain")))
        {
            // Debug.Log($"Hit at: {hit.point}, {hit.collider.gameObject}, {hit.collider.gameObject.layer}");
            return hit.collider.tag == "Player";
        }
        return false;
    }

    private bool HasLayer(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    private class PathWaypoint
    {
        public Waypoint[] connectedWaypoints => waypoint.ConnectedWaypoits;
        public Vector3 position => waypoint.Position;
        public Waypoint waypoint;
        public PathWaypoint parent;
        public bool searched = false;

        public PathWaypoint(Waypoint waypoint)
        {
            this.waypoint = waypoint;
            this.parent = null;
        }

        public PathWaypoint(Waypoint waypoint, PathWaypoint parent)
        {
            this.waypoint = waypoint;
            this.parent = parent;
        }
    }
}

public enum Orientation
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
