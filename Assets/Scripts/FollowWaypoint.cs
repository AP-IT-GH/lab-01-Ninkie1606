using UnityEngine;
using System.Collections.Generic;

public class TankWaypointFollower : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;
    public float nearEnoughDistance = 0.5f;

    private int currentWaypointIndex = 0;
    private bool isNearEnough = false;

    void Update()
    {   
        // break
        if (waypoints.Count == 0)
            return;

        Transform target = waypoints[currentWaypointIndex];

        // Check if near enough
        float distance = Vector3.Distance(transform.position, target.position);
        isNearEnough = distance <= nearEnoughDistance;

        if (isNearEnough)
        {
            // Go to next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
                currentWaypointIndex = 0;
        }
        else
        {
            // Rotate towards waypoint
            Vector3 direction = waypoints[currentWaypointIndex].transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}