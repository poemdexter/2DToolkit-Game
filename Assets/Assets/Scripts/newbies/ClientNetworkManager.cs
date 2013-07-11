using UnityEngine;
using System.Collections;

public class ClientNetworkManager : MonoBehaviour {

	void OnConnectedToServer()
	{
		Network.isMessageQueueRunning = false; // ignore network messages until we're in level
		Application.LoadLevel(ServerNetworkManager.levelName);
	}
	
	void OnLevelWasLoaded(int level)
	{
		if (level != 0 && Network.isClient) // 0 is menu scene
		{
			Network.isMessageQueueRunning = true;
			Debug.Log("Level loaded, requesting spawn");
			networkView.RPC("RequestSpawn", RPCMode.Server, Network.player);
		}
	}
}
