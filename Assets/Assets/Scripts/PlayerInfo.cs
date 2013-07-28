using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo : MonoBehaviour {

	public static string playerName = "ConcernedDad";
	public static bool gameStarted = false;
	
	// returns a list of all players that aren't us
	public static List<GameObject> GetOtherPlayers(string myTag)
	{
		GameObject p1 = GameObject.FindGameObjectWithTag("Player1");
		GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
		GameObject p3 = GameObject.FindGameObjectWithTag("Player3");
		GameObject p4 = GameObject.FindGameObjectWithTag("Player4");
		
		List<GameObject> playerList = new List<GameObject>();
		if (p1 != null && p1.tag != myTag) playerList.Add(p1);
		if (p2 != null && p2.tag != myTag) playerList.Add(p2);
		if (p3 != null && p3.tag != myTag) playerList.Add(p3);
		if (p4 != null && p4.tag != myTag) playerList.Add(p4);
		
		return playerList;
	}
	
	public static GameObject[] GetChildren()
	{
		return GameObject.FindGameObjectsWithTag("Child");
	}
}
