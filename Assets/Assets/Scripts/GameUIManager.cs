using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {
	
	public tk2dUIItem startButton;
	public tk2dTextMesh waitLabel;
	
	void Start()
	{
		
	}
	
	void OnLevelWasLoaded(int level)
	{
		// hide ui depending on server/client connection
		if (Network.isServer) HideWaitLabel();
		else HideStartButton();
		
		// late connectors, if started hide the wait label
		if (PlayerInfo.gameStarted) HideWaitLabel();
	}
	
	public void HideStartButton()
	{
		startButton.transform.position += new Vector3(0,0,.5f);
		startButton.enabled = false;
	}
	
	void HideWaitLabel()
	{
		waitLabel.transform.position += new Vector3(0,0,.5f);
	}
	
    void OnEnable()
    {
        startButton.OnClick += StartGame;
    }

    void OnDisable()
    {
        startButton.OnClick -= StartGame;
    }
	
	void StartGame()
	{
		if (Network.isServer) HideStartButton();
		PlayerInfo.gameStarted = true;
		networkView.RPC("TellOthersToStartGame", RPCMode.OthersBuffered);
	}
	
	[RPC]
	void TellOthersToStartGame()
	{
		HideWaitLabel();
		PlayerInfo.gameStarted = true;
	}
}
