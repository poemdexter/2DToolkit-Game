using UnityEngine;
using System.Collections;

public class GameAudioManager : MonoBehaviour {
	
	bool startMusic = false;
	bool startedMusic = false;
	bool startMusic2 = false;
	bool startedMusic2 = false;
	
	public float musicVolume = 1;
	
	public AudioSource playingMusic;
	public AudioSource waitingMusic;
	
	// Update is called once per frame
	void Update () {
		
		if (!PlayerInfo.gameStarted) 
			startMusic2 = true;
		if (startMusic2 && !startedMusic2)
		{
			waitingMusic.volume = musicVolume;
			waitingMusic.Play();
			startedMusic2 = true;
		}
		
		if (PlayerInfo.gameStarted)
		{
			startMusic = true;
			waitingMusic.Stop();
		}
		if (startMusic && !startedMusic)
		{
			playingMusic.volume = musicVolume;
			playingMusic.Play();
			startedMusic = true;
		}
	}
}
