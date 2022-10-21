using PathCreation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TicketFlow : MonoBehaviour
{
	public bool isFlowing = false;
	public GameObject prefab;
	public GameObject ticketEffectSpawnPos;
	public GameObject ticketsParentObj;
	public float flowSpeed = 1;
	public float distance = 0;
	public float distanceTickets = 0.5f;

	[HideInInspector] public List<GameObject> spawnedTickets;
	private VertexPath path;
	private Player player;
	private float ticketInMachineDistance;

	private void Start()
    {
		player = Player.player;

		path = GetComponent<PathCreator>().path;

		spawnedTickets = new List<GameObject>();

		for (float i = 0; i < path.length - distanceTickets; i += distanceTickets)
        {
            GameObject spawned = Instantiate(prefab, path.GetPointAtDistance(distance + i), path.GetRotationAtDistance(distance + i), ticketsParentObj.transform);
            spawnedTickets.Add(spawned);
        }
	}

	private void Update()
	{
		if (!isFlowing) return;

		for (int i = 0; i < spawnedTickets.Count; i++)
		{
			spawnedTickets[i].transform.position = path.GetPointAtDistance(distance + i * distanceTickets);
			spawnedTickets[i].transform.rotation = path.GetRotationAtDistance(distance + i * distanceTickets);
		}
	}

	private void FixedUpdate()
	{
		if (!isFlowing) return;

		distance += flowSpeed * Time.deltaTime;
		ticketInMachineDistance += flowSpeed * Time.deltaTime;

		if (ticketInMachineDistance > distanceTickets)
		{
			ticketInMachineDistance = 0;
			player.OnTicketInMachine(ticketEffectSpawnPos.transform.position);
		}
	}
}
