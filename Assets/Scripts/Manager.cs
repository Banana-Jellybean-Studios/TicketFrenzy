using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Manager : MonoBehaviour
{
    public GameObject pathObject;
    public GameObject prefab;
    public float distance = 0;
	public float distanceTickets = 0.5f;

	private List<GameObject> spawnedTickets;
    private VertexPath path;

    void Start()
    {
        path = pathObject.GetComponent<PathCreator>().path;
		spawnedTickets = new List<GameObject>();

        for (float i = 0; i < path.length - distanceTickets; i+= distanceTickets)
        {
			GameObject spawned = Instantiate(prefab, path.GetPointAtDistance(distance + i), path.GetRotationAtDistance(distance + i));
            spawnedTickets.Add(spawned);
		}
    }

    private void Update()
    {
        for (int i = 0; i < spawnedTickets.Count; i++)
        {
			spawnedTickets[i].transform.position = path.GetPointAtDistance(distance + i * distanceTickets);
			spawnedTickets[i].transform.rotation = path.GetRotationAtDistance(distance + i * distanceTickets);
		}
    }
}
