using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct Link {

    public enum direction {

        UNI,
        BI
    }

    public GameObject node1, node2;
    public direction dir;
}
// een waypoint is een node
public class WPManager : MonoBehaviour {

    public GameObject[] waypoints;
    public Link[] links;
    public Graph graph = new Graph();
    public TMP_Dropdown dropdown;
    void Start()
    {

        if (waypoints.Length > 0)
        {
            // First, add all nodes to the graph
            foreach (GameObject wp in waypoints)
            {
                graph.AddNode(wp);
            }

            // SECOND, add the edges once the nodes exist
            foreach (Link l in links)
            {
                graph.AddEdge(l.node1, l.node2);
                if (l.dir == Link.direction.BI)
                {
                    graph.AddEdge(l.node2, l.node1);
                }
            }
        }
        PopulateDropdown();
    }


    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (GameObject wp in waypoints)
        {
            options.Add("Go to " + wp.name);
        }
        dropdown.AddOptions(options);
    }
}
