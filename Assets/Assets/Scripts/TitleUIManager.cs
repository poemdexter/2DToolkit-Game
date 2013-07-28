using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleUIManager : MonoBehaviour {
	
	public tk2dUIItem refreshButton;
	public tk2dUIItem createButton;
	public GameObject hostEntry;
	
	public string gameTypeName = "Killa_Kidz_SA";
	
	private bool hostsRefreshing;
	private bool hostsUpdated;
	private HostData[] hostDataList;
	private List<GameObject> hostUIElementList;
	
	void Start()
	{
		hostUIElementList = new List<GameObject>();
	}
	
    void OnEnable()
    {
        refreshButton.OnClick += RefreshHostsList;
        createButton.OnClick += StartServer;
    }

    void OnDisable()
    {
        refreshButton.OnClick -= RefreshHostsList;
        createButton.OnClick -= StartServer;
    }
	
	void RefreshHostsList()
	{
		MasterServer.RequestHostList(gameTypeName);
		hostsRefreshing = true;
		hostsUpdated = false;
	}
	
	void StartServer()
	{
		Network.InitializeServer(4, 9001, !Network.HavePublicAddress());
		// get game name
		string gameNameInput;
		GameObject input = GameObject.FindGameObjectWithTag("GameNameInput");
		if (input != null) gameNameInput = input.GetComponent<tk2dTextMesh>().text;
		else gameNameInput = "New Game";
		SetPlayerName();
		MasterServer.RegisterHost(gameTypeName, gameNameInput);
		
		// in we go!
		Application.LoadLevel("2DGame");
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent) 
	{
		if(msEvent == MasterServerEvent.RegistrationSucceeded)
			Debug.Log("Server Registered");
		
		if(msEvent == MasterServerEvent.HostListReceived)
			hostsUpdated = true;
	}
	
	void Update()
	{
		if (hostsRefreshing && hostsUpdated)
		{
			hostsRefreshing = false;
			hostsUpdated = false;
			hostDataList = MasterServer.PollHostList();
			CreateHostListUI();
		}
	}
	
	void CreateHostListUI()
	{
		if (hostUIElementList.Count > 0) ClearHostElementList();
		
		float yOffset = 0;
		foreach(HostData host in hostDataList)
		{
			if (host.connectedPlayers < host.playerLimit)
			{
				GameObject hostObj = (GameObject) Instantiate(hostEntry);
				hostObj.transform.position += new Vector3(0,yOffset,0);
				yOffset += -0.1f;
				ConfigureHostUIElement(hostObj, host);
				hostUIElementList.Add(hostObj);
			}
		}
	}
	
	void ClearHostElementList()
	{
		foreach(GameObject ui in hostUIElementList)
		{
			Destroy(ui);
		}
		hostUIElementList.Clear();
	}
	
	// customize host UI element based on the host it's representing
	void ConfigureHostUIElement(GameObject ui, HostData host)
	{
		foreach(Transform childTransform in ui.transform)
		{
			if (childTransform.gameObject.name == "GameNameLabel")
			{
				childTransform.gameObject.GetComponent<tk2dTextMesh>().text = host.gameName;
				childTransform.gameObject.GetComponent<tk2dTextMesh>().Commit();
			}
			if (childTransform.gameObject.name == "PlayerAmountLabel")
			{
				childTransform.gameObject.GetComponent<tk2dTextMesh>().text = host.connectedPlayers.ToString();
				childTransform.gameObject.GetComponent<tk2dTextMesh>().Commit();
			}
			if (childTransform.gameObject.name == "JoinServerButton")
			{
				childTransform.gameObject.GetComponent<HostDataContainer>().hostData = host;
				childTransform.gameObject.GetComponent<tk2dUIItem>().OnClickUIItem += JoinServer;
			}
		}
	}
	
	void JoinServer(tk2dUIItem clickedUIItem)
	{
		HostData host = clickedUIItem.gameObject.GetComponent<HostDataContainer>().hostData;
		SetPlayerName();
		if (Network.Connect(host) == NetworkConnectionError.NoError)
			Application.LoadLevel("2DGame");
	}
	
	void SetPlayerName()
	{
		GameObject input = GameObject.FindGameObjectWithTag("UserNameInput");
		if (input != null) PlayerInfo.playerName = input.GetComponent<tk2dTextMesh>().text;
	}
}
