using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ClientPlayerManager : MonoBehaviour {
	
	private NetworkPlayer owner;
	
	[RPC]
	void SetOwner(NetworkPlayer player)
	{
		owner = player;
		
		if (player == Network.player) 
			enabled = true;
		else
		{
			if (GetComponent<Camera>()) GetComponent<Camera>().enabled = false;
			if (GetComponent<AudioListener>()) GetComponent<AudioListener>().enabled = false;
			if (GetComponent<GUILayer>()) GetComponent<GUILayer>().enabled = false;
		}
	}
	
	[RPC]
	public NetworkPlayer GetOwner()
	{
		return owner;
	}
	
	void Awake()
	{
		if (Network.isClient) enabled = false;
	}
	
	void Update()
	{
		if (Network.isClient)
		{
			if ((owner != null) && (Network.player == owner))
			{
				if (InputDetected())
				{
					networkView.RPC ("UpdateClientMotion", RPCMode.Server, 
					Input.GetKeyDown(KeyCode.A),
					Input.GetKeyDown(KeyCode.D),
					Input.GetKeyDown(KeyCode.Space));
				}
			}
		}
	}
	
	bool InputDetected()
	{
		return  Input.GetKeyDown(KeyCode.A) ||
				Input.GetKeyDown(KeyCode.D) ||
				Input.GetKeyDown(KeyCode.Space);
	}
}
