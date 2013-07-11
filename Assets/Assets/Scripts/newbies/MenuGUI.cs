using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	
	public string gameName = "Rogue_Tower_0.1.0";
	private HostData[] hostDataList;
	private bool hostsRefreshing;
	private bool hostsUpdated;
	
	void Start()
	{
		hostDataList = new HostData[] {};
		ServerNetworkManager.levelName = "2DGame";
	}
	
	void OnGUI() 
	{	
		if(GUILayout.Button("Start Server"))
		{
			Debug.Log("Starting Server");
			StartServer();
		}
		
		if(GUILayout.Button("Refresh Hosts"))
		{
			Debug.Log("Requesting Hosts...");
			RefreshHostsList();
		}
		
		foreach(HostData host in hostDataList)
		{
			if(GUILayout.Button(host.gameName))
			{	
				Network.Connect(host);
			}
		}
	}
	
	void StartServer()
	{
		Network.InitializeServer(4, 9001, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, "Rogue Tower");
	}
	
	void RefreshHostsList()
	{
		MasterServer.RequestHostList(gameName);
		hostsRefreshing = true;
		hostsUpdated = false;
	}
	
	void Update()
	{
		if (hostsRefreshing && hostsUpdated)
		{
			hostsRefreshing = false;
			hostsUpdated = false;
			Debug.Log("Found " + MasterServer.PollHostList().Length);
			hostDataList = MasterServer.PollHostList();
		}
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent) 
	{
		if(msEvent == MasterServerEvent.RegistrationSucceeded)
			Debug.Log("Server Registered");
		
		if(msEvent == MasterServerEvent.HostListReceived)
			hostsUpdated = true;
	}
	
	void OnServerInitialized() {
		Application.LoadLevel(ServerNetworkManager.levelName);
	}
}
