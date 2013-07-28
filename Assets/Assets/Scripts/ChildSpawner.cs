using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildSpawner : MonoBehaviour
{	
	public GameObject childPrefab;
	public int maxChildren = 3;
	private int childCount = 0;
	private List<Transform> spawnList;
	
	void Start ()
	{
		spawnList = new List<Transform>();
		foreach(Transform spawnpoint in transform)
		{
			spawnList.Add(spawnpoint);
		}
	}
	
	void Update ()
	{
		if (PlayerInfo.gameStarted)
		{
			if (childCount < maxChildren)
			{
				if (Network.isServer)
				{
					Transform t = spawnList[Random.Range(0, spawnList.Count)];
					Network.Instantiate(childPrefab, t.position, Quaternion.identity, 0);	
					childCount++;
				}
			}
		}
	}
	
	public void MinusOneChild()
	{
		if (Network.isServer) childCount--;
	}
}
