using UnityEngine;
using System.Collections;

public class ChildController : MonoBehaviour 
{
    public void KillChild(string killerTag)
	{
		// need to despawn this chest and decriment counter on spawner
		networkView.RPC("BroadcastDeadChild", RPCMode.AllBuffered, killerTag);
		Network.Destroy(this.gameObject);
	}
	
	[RPC]
	void BroadcastDeadChild(string killerTag)
	{
		// increase killer's score
		GameObject.Find("GlobalScripts").GetComponent<NetworkManager>().IncreaseKillerScore(killerTag);
		
		// tell server to decrement counter so another respawns
		GameObject go = GameObject.Find("ChildSpawnPoints");
		go.GetComponent<ChildSpawner>().MinusOneChild();
	}
}
