using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour {
	
	public tk2dSprite jamLogo;
	public tk2dSprite teamLogo;
	public tk2dTextMesh warningTitle;
	public tk2dTextMesh warningText;
	public tk2dTextMesh skipText;
	public tk2dTextMesh storyText;
	public tk2dTextMesh story2Text;
	
	float jamAlpha = 0;
	bool jamStart = true;
	bool jamEnd = false;
	
	float teamAlpha = 0;
	bool teamStart = false;
	bool teamEnd = false;
	
	bool showWarning = false;
	bool warningSkippable = false;
	double showWarningTime = 0;
	double showWarningMinTime = 3;
	
	bool startedMusic = false;
	bool showStory = false;
	bool storySkippable = false;
	double showStoryTime = 0;
	double showStoryMinTime = 8;
	double showStory2Point = 6;
	double endStoryPoint = 12;
	float story1Alpha = 0;
	float story2Alpha = 0;
	
	void Update()
	{
		if (jamStart || jamEnd) ShowJam();
		if (teamStart || teamEnd) ShowTeam();
		if (showWarning) ShowWarning();
		if (showStory) ShowStory();
	}
	
	void ShowJam()
	{
		if (jamStart)
		{
			if (jamAlpha < 1)
			{
				
				jamAlpha += .01f;
				if (jamAlpha > 1) jamAlpha = 1;
				jamLogo.color = new Color(255,255,255,jamAlpha);
			}
			else
			{
				jamStart = false;
				jamEnd = true;
			}
		}
		
		if (jamEnd)
		{
			if (jamAlpha > 0)
			{
				jamAlpha -= .01f;
				if (jamAlpha < 0) jamAlpha = 0;
				jamLogo.color = new Color(255,255,255,jamAlpha);
			}
			else
			{
				jamEnd = false;
				teamStart = true;
			}
		}
	}
	
	void ShowTeam()
	{
		if (teamStart)
		{
			if (teamAlpha < 1)
			{
				teamAlpha += .01f;
				if (teamAlpha > 1) teamAlpha = 1;
				teamLogo.color = new Color(255,255,255,teamAlpha);
			}
			else
			{
				teamStart = false;
				teamEnd = true;
			}
		}
		
		if (teamEnd)
		{
			if (teamAlpha > 0)
			{
				teamAlpha -= .01f;
				if (teamAlpha < 0) teamAlpha = 0;
				teamLogo.color = new Color(255,255,255,teamAlpha);
			}
			else
			{
				teamEnd = false;
				showWarning = true;
			}
		}
	}
	
	void ShowWarning()
	{
		warningText.color = new Color(1,1,1,1);
		warningTitle.color = new Color(1,0,0,1);
		warningText.Commit();
		warningTitle.Commit();
		
		if ((showWarningTime += Time.deltaTime) > showWarningMinTime)
		{
			skipText.color = new Color(1,1,1,1);
			skipText.Commit();
			warningSkippable = true;
		}
		
		if (warningSkippable && Input.anyKey)
		{
			warningText.color = new Color(1,1,1,0);
			warningTitle.color = new Color(1,0,0,0);
			warningText.Commit();
			warningTitle.Commit();
			skipText.color = new Color(1,1,1,0);
			skipText.Commit();
			showWarning = false;
			warningSkippable = false;
			showStory = true;
		}	
	}
	
	void ShowStory()
	{
		if (!startedMusic)
		{
			audio.Play();
			startedMusic = true;
		}
		
		// fade in story 1
		if (story1Alpha < 1)
		{
			story1Alpha += .02f;
			if (story1Alpha > 1) story1Alpha = 1;
			storyText.color = new Color(1,1,1,story1Alpha);
			storyText.Commit();
		}
		
		// after some point, fade in story 2
		if ((showStoryTime += Time.deltaTime) > showStory2Point)
		{
			if (story2Alpha < 1)
			{
				story2Alpha += .02f;
				if (story2Alpha > 1) story2Alpha = 1;
				story2Text.color = new Color(1,0,0,story2Alpha);
				story2Text.Commit();
			}
		}
		
		// just show skippable after 3 sec
		if (showStoryTime > showStoryMinTime)
		{
			skipText.color = new Color(1,1,1,1);
			skipText.Commit();
			storySkippable = true;
		}
		// skipped or end of song
		if ((storySkippable && Input.anyKey) || showStoryTime > endStoryPoint)
		{	
			audio.Stop();
			Application.LoadLevel("TitleScreen");
		}	
	}
}