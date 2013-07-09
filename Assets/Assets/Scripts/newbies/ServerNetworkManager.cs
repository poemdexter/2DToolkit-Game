using UnityEngine;
using System.Collections.Generic;

public enum NetworkGroup
{
	DEFAULT = 0,
	PLAYER = 1,
	SERVER = 2
}

public class ServerNetworkManager : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawnPoint;
	public static string levelName;
	
	private List<ClientPlayerManager> playerTracker = new List<ClientPlayerManager>();
	private List<NetworkPlayer> scheduledSpawns = new List<NetworkPlayer>();
	
	private bool processSpawnRequests;
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		scheduledSpawns.Add(player);
		processSpawnRequests = true;
	}
	
	[RPC]
	void RequestSpawn(NetworkPlayer requester)
	{
		if (Network.isServer && processSpawnRequests)
		{
			foreach(NetworkPlayer spawn in scheduledSpawns)
			{
				if (spawn == requester)
				{
					GameObject handle = (GameObject) Network.Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity, 0);
					var scm = handle.GetComponent<ClientPlayerManager>();
					if (!scm) Debug.Log("Prefab has no ClientPlayerManager attached!");
					playerTracker.Add(scm);
					NetworkView netView = handle.GetComponent<NetworkView>();
					netView.RPC("SetOwner", RPCMode.AllBuffered, spawn);
				}
			}
			scheduledSpawns.Remove(requester);
			if(scheduledSpawns.Count == 0)
			{
				Debug.Log("Spawn List empty");
				processSpawnRequests = false;
			}
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log ("Player " + player.guid + " disconnected.");
		ClientPlayerManager found = null;
		foreach(ClientPlayerManager man in playerTracker)
		{
			if (man.GetOwner() == player)
			{
				Network.RemoveRPCs(man.gameObject.networkView.viewID);
				Network.Destroy(man.gameObject);
				found = man;
			}
		}
		if (found) playerTracker.Remove(found);
	}
}
