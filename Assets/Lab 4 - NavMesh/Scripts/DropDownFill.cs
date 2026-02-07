using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections.Generic;

public class LocationNavigator : MonoBehaviour
{
    public NavMeshAgent agent;
    public TMP_Dropdown dropdown; // Sleep hier je Dropdown in
    public List<Transform> locations; // Sleep hier je locaties in

    void Start()
    {
        SetupDropdown();
    }

    void SetupDropdown()
    {
        dropdown.ClearOptions(); // Maak de dropdown leeg
        List<string> namen = new List<string>();

        foreach (Transform t in locations)
        {
            namen.Add(t.name); // Pak de naam van het GameObject
        }

        dropdown.AddOptions(namen); // Voeg de namen toe aan de dropdown
    }

    public void OnLocationChanged(int index)
    {
        agent.SetDestination(locations[index].position);
    }
}