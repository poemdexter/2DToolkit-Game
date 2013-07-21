using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	public string gameName = "Rogue_Tower_1";
	public GameObject playerPrefab;
	public Transform spawnPoint;
	
	private bool hostsRefreshing;
	private bool hostsUpdated;
	private HostData[] hostDataList;
	
	void Start()
	{
		hostDataList = new HostData[] {};
	}
	
	void OnGUI()
	{
		if (!Network.isServer && !Network.isClient)
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
				if(GUILayout.Button(host.comment))
				{
					Network.Connect(host);
				}
			}
		}
	}
	
	void StartServer()
	{
		Network.InitializeServer(4, 9001, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, "Rogue Tower", "poem's game");
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
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initialized");
		SpawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		SpawnPlayer();
	}
	
	void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity, 0);
	}
}
