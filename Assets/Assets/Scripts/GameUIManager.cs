using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {
	
	public tk2dUIItem startButton;
	public tk2dUIItem exitButton;
	public tk2dTextMesh waitLabel;
	
	void Start()
	{
		exitButton.enabled = false;
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
		exitButton.OnClick += ExitGame;
    }

    void OnDisable()
    {
        startButton.OnClick -= StartGame;
		exitButton.OnClick -= ExitGame;
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
	
	void ExitGame()
	{
		Network.Disconnect();
		Application.LoadLevel("TitleScreen");
	}
}
