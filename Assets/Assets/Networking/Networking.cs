using UnityEngine;
using System.Collections;

public class Networking : MonoBehaviour {
	
	string ServerIP = "localhost";
	string ServerPort = "9001";
	
	void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if(GUILayout.Button("Connect"))
			{
				Network.Connect(ServerIP, int.Parse(ServerPort));
			}
			if(GUILayout.Button("New Server"))
			{
				Network.InitializeServer(4,int.Parse(ServerPort), false);
			}
		}
		else
		{
			if (GUILayout.Button("Disconnect"))
			{
				Network.Disconnect();
			}
		}
	}
}
