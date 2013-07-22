using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureSpawner : MonoBehaviour
{	
	public GameObject chestPrefab;
	public Transform spawnPoint;
	public int maxChests = 1;
	private int chestCount = 0;
	private List<Transform> spawnList;
	
	void Start ()
	{
		spawnList = new List<Transform>();
		foreach(Transform child in transform)
		{
			spawnList.Add(child);
		}
	}
	
	void Update () 
	{
		if (chestCount < maxChests)
		{
			if (Network.isServer)
			{
				Transform t = spawnList[Random.Range(0, spawnList.Count)];
				Network.Instantiate(chestPrefab, t.position, Quaternion.identity, 0);	
				chestCount++;
			}
		}
	}
	
	public void RemoveChest()
	{
		if (Network.isServer) chestCount--;
	}
}
