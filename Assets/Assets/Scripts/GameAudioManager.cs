using UnityEngine;
using System.Collections;

public class GameAudioManager : MonoBehaviour {
	
	bool startMusic = false;
	bool startedMusic = false;
	
	public float musicVolume = 1;
	
	// Update is called once per frame
	void Update () {
		if (PlayerInfo.gameStarted) startMusic = true;
		if (startMusic && !startedMusic)
		{
			audio.volume = musicVolume;
			audio.Play();
			startedMusic = true;
		}
	}
}
