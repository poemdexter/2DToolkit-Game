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
	public int maxScore = 3;
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
		
		// tell people what your name is
		networkView.RPC("TellOtherPlayersMyName", RPCMode.AllBuffered, position, PlayerInfo.playerName);
	}
	
	[RPC]
	void TellOtherPlayersMyName(int position, string name)
	{
		tk2dTextMesh nameText = null;
		switch (position)
		{
		case 1: 
			GameObject.Find("Player1Info").transform.position += new Vector3(0,0,-0.2f);
			nameText = GameObject.Find("p1Name").GetComponent<tk2dTextMesh>(); 
			break;
		case 2: 
			GameObject.Find("Player2Info").transform.position += new Vector3(0,0,-0.2f);
			nameText = GameObject.Find("p2Name").GetComponent<tk2dTextMesh>(); 
			break;
		case 3: 
			GameObject.Find("Player3Info").transform.position += new Vector3(0,0,-0.2f);
			nameText = GameObject.Find("p3Name").GetComponent<tk2dTextMesh>(); 
			break;
		case 4: 
			GameObject.Find("Player4Info").transform.position += new Vector3(0,0,-0.2f);
			nameText = GameObject.Find("p4Name").GetComponent<tk2dTextMesh>(); 
			break;
		default: break; // spectating
		}
		
		if (nameText != null)
		{
			nameText.text = name;
			nameText.Commit();
		}
	}
	
	public void IncreaseKillerScore(string killerTag)
	{
		tk2dTextMesh scoreText = null;
		switch(killerTag)
		{
		case "Player1":
			scoreText = GameObject.Find("p1Score").GetComponent<tk2dTextMesh>();
			break;
		case "Player2":
			scoreText = GameObject.Find("p2Score").GetComponent<tk2dTextMesh>();
			break;
		case "Player3":
			scoreText = GameObject.Find("p3Score").GetComponent<tk2dTextMesh>();
			break;
		case "Player4":
			scoreText = GameObject.Find("p4Score").GetComponent<tk2dTextMesh>();
			break;
		}
		
		int currentScore = int.Parse(scoreText.text);
		currentScore++;
		scoreText.text = currentScore.ToString();
		scoreText.Commit();
		
		if (currentScore == maxScore) networkView.RPC("GameOver", RPCMode.AllBuffered, killerTag);
	}
	
	[RPC]
	void GameOver(string winnerTag)
	{
		// 1. stop people from moving
		PlayerInfo.gameStarted = false;
		
		// 2. inform people of who the winner is
		// get winner name
		string name = "";
		switch(winnerTag)
		{
		case "Player1": name = GameObject.Find("p1Name").GetComponent<tk2dTextMesh>().text; break;
		case "Player2": name = GameObject.Find("p2Name").GetComponent<tk2dTextMesh>().text; break;
		case "Player3": name = GameObject.Find("p3Name").GetComponent<tk2dTextMesh>().text; break;
		case "Player4": name = GameObject.Find("p4Name").GetComponent<tk2dTextMesh>().text; break;
		}
		// change winner name and show it
		tk2dTextMesh nameText = GameObject.Find("WinnerName").GetComponent<tk2dTextMesh>();
		nameText.text = name;
		nameText.Commit();
		GameObject.Find("WinnerText").transform.position += new Vector3(0,0,-0.2f);
		
		// 3. allow next round to start from host via button
		GameObject.Find("ExitGameButton").GetComponent<tk2dUIItem>().enabled = true;
	}
}
