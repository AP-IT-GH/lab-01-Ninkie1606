using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// een classe die de nodes voorstelt.
// helpt bij het markeren van open,closed,path nodes
public class PathMarker {

    public MapLocation location;
    public float G, H, F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject m, PathMarker p) {

        location = l;
        G = g;
        H = h;
        F = f;
        marker = m;
        parent = p;
    }

    public override bool Equals(object obj) {

        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }

   
}
// het brein.
// gaat met het A* alg opzoek naar het snelste path.
// tijdens het alg markeert het open nodes en wanneer nodes sluiten.
// na afloop markeert het het juiste path.
public class FindPathAStar : MonoBehaviour {

    public Maze maze;
    // kleuren voor overzicht
    public Material closedMaterial;
    public Material openMaterial;
    public Material pathMaterial;

    // de game objecten waar je A* op uitvoert
    // hier onder als pathmarkers
    public GameObject start;
    public GameObject end;
    public GameObject pathP;

    // de drie punten voor A* om te calculeren 
    // bevatten G,H,F waarden
    PathMarker startNode;
    PathMarker goalNode;
    PathMarker lastPos; // of current in powerpoint
    bool done = false; // goalbreak
    bool hasStarted = false;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    void RemoveAllMarkers() {

        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");

        foreach (GameObject m in markers) Destroy(m);

        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
       // Destroy(goal);
        if (player != null) Destroy(player);
    }

    void BeginSearch() {

        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new List<MapLocation>();

        for (int z = 1; z < maze.depth - 1; ++z) {
            for (int x = 1; x < maze.width - 1; ++x) {

                if (maze.map[x, z] != 1) {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }
        locations.Shuffle();

        Vector3 startLocation = new Vector3(1*maze.scale, 0.5f, 1*maze.scale);
        startNode = new PathMarker(new MapLocation(1, 1),
            0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);

        MapLocation goalLoc = locations[0];
        Vector3 endLocation = new Vector3(goalLoc.x * maze.scale, 0.5f, goalLoc.z * maze.scale);
        goalNode = new PathMarker(new MapLocation((int)endLocation.x, (int)endLocation.z),
            0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(startNode);
        lastPos = startNode;
    }

    void Search(PathMarker thisNode) {

        if (thisNode.location.Equals(goalNode.location)) {

              done = true;
            
            return;
        }

        foreach (MapLocation dir in maze.directions) {

            MapLocation neighbour = dir + thisNode.location;

            if (neighbour.x < 1 || neighbour.x > maze.width || neighbour.z < 1 || neighbour.z > maze.depth) continue;

            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (IsClosed(neighbour)) continue;

            float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float f = g + h;


            if (!UpdateMarker(neighbour, g, h, f, thisNode)) {
                GameObject pathBlock = Instantiate(pathP, new Vector3(neighbour.x * maze.scale, 0.0f, neighbour.z * maze.scale), Quaternion.identity);
                pathBlock.GetComponent<Renderer>().material = openMaterial; // marks all open material
                open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
            }
        }
        if (open.Count > 0)
        {
            open = open.OrderBy(p => p.F).ToList<PathMarker>();
            PathMarker pm = (PathMarker)open.ElementAt(0);
            closed.Add(pm);
            open.RemoveAt(0);
            pm.marker.GetComponent<Renderer>().material = closedMaterial; // marks all closed material
            lastPos = pm;

        }


    }

    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt) {

        foreach (PathMarker p in open) {

            if (p.location.Equals(pos)) {

                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(MapLocation marker) {

        foreach (PathMarker p in closed) {

            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void Start() {
        BeginSearch(); // setup
        StartCoroutine(Searching());

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.P)) {

            BeginSearch();
            hasStarted = true;          
        }
        

        if (hasStarted)
            if (Input.GetKeyDown(KeyCode.C)) Search(lastPos);
    }

    // The coroutine function
    bool searchingHasFinished = false;
    IEnumerator Searching()
    {
        Debug.Log("searching started!");

        while (!done)
        {
            // Perform some task
            Debug.Log("Coroutine is running...");
            Search(lastPos);
            // Wait for the next frame
            yield return true;
        }

        searchingHasFinished = true;
        ReconstructPath();
        Debug.Log("Coroutine finished!");
    }

    bool PathHasConstructed = false;
    List<PathMarker> path = new List<PathMarker>();
    void ReconstructPath()
    {
        
        path.Add(closed[closed.Count-1]);
        var p = closed[closed.Count-1].parent;
        while(p!= startNode)
        {
            path.Insert(0, p);
            p = p.parent;
        }
        path.Insert(0,startNode);
        PathHasConstructed = true;

        foreach(PathMarker pm in path) // mark the path with pathmaterial
        {
            if (pm.parent != null)
            {
                pm.marker.GetComponent<Renderer>().material = pathMaterial;
            }
        }

    }
   
}
// list extension om een random node te pakken uit de lijst als goal.
// zo blijft er randomnes in het maze zodat je dit niet op 1 situatie ziet
public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}