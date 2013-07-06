using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	
	public Transform PlayerPrefab;
	
	void OnServerInitialized()
	{
		SpawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		SpawnPlayer();
	}
	
	void SpawnPlayer()
	{
		Network.Instantiate(PlayerPrefab, transform.position, transform.rotation, 0);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		Application.LoadLevel(Application.loadedLevel);
	}
}
