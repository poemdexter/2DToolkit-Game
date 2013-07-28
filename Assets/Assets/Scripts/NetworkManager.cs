using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	public GameObject player1Prefab;
	public GameObject player2Prefab;
	public GameObject player3Prefab;
	public GameObject player4Prefab;
	
	public Transform spawnPoint_P1;
	public Transform spawnPoint_P2;
	public Transform spawnPoint_P3;
	public Transform spawnPoint_P4;
	
	int position = 1; // for server to increment and hand out
	public bool spawned = false;
	
	void Start()
	{
		// we're server and just came in, spawn us
		if (Network.isServer)
		{
			SpawnPlayer();
			position++;
		}
	}

	void Update()
	{
		
	}
	
	// on new players connecting to the server
	void OnPlayerConnected()
	{
		// we're server and someone else just came in, tell player where to spawn
		if (Network.isServer)
		{
			networkView.RPC("TellIncomingPlayersWhereToSpawn", RPCMode.Others, position);
			position++;
		}
	}
	
	[RPC]
	void TellIncomingPlayersWhereToSpawn(int newPosition)
	{
		position = newPosition;
		
		if (Network.isClient && !spawned && !PlayerInfo.gameStarted)
		{
			spawned = true;
			SpawnPlayer();
		}
	}
	
	void SpawnPlayer()
	{
		switch (position)
		{
		case 1: Network.Instantiate(player1Prefab, spawnPoint_P1.position, Quaternion.identity, 0); break;
		case 2: Network.Instantiate(player2Prefab, spawnPoint_P2.position, Quaternion.identity, 0); break;
		case 3: Network.Instantiate(player3Prefab, spawnPoint_P3.position, Quaternion.identity, 0); break;
		case 4: Network.Instantiate(player4Prefab, spawnPoint_P4.position, Quaternion.identity, 0); break;
		default: break; // spectating
		}
	}
}
