using UnityEngine;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    [Header("References")]
    public WPManager wpManager; // Drag WPManager here

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float rotationSpeed = 2.0f;
    public float nearEnoughDistance = 2.0f; // The "range" from the tree

    GameObject currentNode;
    int currentWaypointIndex = 0;
    bool isMoving = false;

    void Start()
    {
        // Spawn logic: y+1 and shifted left as requested
        if (wpManager.waypoints.Length > 0)
        {
            currentNode = wpManager.waypoints[0];
            transform.position = currentNode.transform.position + Vector3.up + (Vector3.right * 2);
        }
    }

    // Called by your Dropdown
    public void GoToTarget(int index)
    {
        GameObject targetNode = wpManager.waypoints[index];

        // Run the A* math
        if (wpManager.graph.AStar(currentNode, targetNode))
        {
            currentWaypointIndex = 0;
            isMoving = true;
        }
    }

    void Update()
    {
        // Stop if we don't have a path
        if (!isMoving || wpManager.graph.pathList.Count == 0) return;

        // Get current target from pathList
        GameObject target = wpManager.graph.getPathPoint(currentWaypointIndex);

        float distance = Vector3.Distance(transform.position, target.transform.position);

        // Movement Logic
        if (distance > nearEnoughDistance)
        {
            // Rotate towards waypoint
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0; // Keep the tank level
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move forward
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            // Range reached! Move to next node or stop if it was the final tree
            currentNode = target;
            currentWaypointIndex++;

            if (currentWaypointIndex >= wpManager.graph.pathList.Count)
            {
                isMoving = false; // Final destination reached
            }
        }
    }
}