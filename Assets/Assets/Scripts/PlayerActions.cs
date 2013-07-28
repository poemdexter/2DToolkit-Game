using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour 
{
	public double maxSwingTime = 0.2; // this must be equal to length of swing animation
	private double currentSwingTime = 0.0;
	private bool swinging = false; // controls animation and part of timing logic
	private bool needNewPress = false; // ensures we're pressing the swing key again after a full length swing
	
	public double maxStunnedTime = 1.0;
	private double currentStunnedTime = 0.0;
	private bool isStunned = false;
	
	void Update()
	{
		if (networkView.isMine)
		{
			// swing button; we current force player to tap swing everytime he wants to swing again and he has to go for entire animation
			if (PlayerInfo.gameStarted && !isStunned && Input.GetButton("Open"))
			{
				// we can swing and now waiting for fresh press, lets do it
				if (!swinging && !needNewPress) swinging = true;
			}
			else // let go of button
			{
				needNewPress = false;
			}
			
			// we're midswing
			if (swinging)
			{
				// and haven't hit cap yet
				if (currentSwingTime <= maxSwingTime)
				{
					// check for collisions with other players
					CheckHittingOtherPlayers();
					CheckHittingAChild();
					
					currentSwingTime += Time.deltaTime;
				}
				else // we're at max swing time
				{
					swinging = false;
					currentSwingTime = 0;
					needNewPress = true;
				}
			}
			
			if (isStunned)
			{
				if (currentStunnedTime <= maxStunnedTime)
				{
					currentStunnedTime += Time.deltaTime;
				}
				else
				{
					isStunned = false;
					currentStunnedTime = 0;
					networkView.RPC("TellOthersImNotStunned", RPCMode.AllBuffered, tag);
				}
			}
		}
	}
	
	void CheckHittingOtherPlayers()
	{
		foreach(GameObject player in PlayerInfo.GetOtherPlayers(tag))
		{
			if (collider.bounds.Intersects(player.collider.bounds))
			{
				if (!player.GetComponent<PlayerActions>().isStunned)
				{
					networkView.RPC("TellOthersAboutStunningSomeone", RPCMode.AllBuffered, player.tag);
				}
			}
		}
	}
	
	void CheckHittingAChild()
	{
		foreach(GameObject child in PlayerInfo.GetChildren())
		{
			if (collider.bounds.Intersects(child.collider.bounds))
			{
				child.GetComponent<ChildController>().KillChild(tag);
			}
		}
	}
	
	[RPC]
	void TellOthersAboutStunningSomeone(string stunnedTag)
	{
		GameObject player = GameObject.FindGameObjectWithTag(stunnedTag);
		player.GetComponent<PlayerActions>().StunMe();
	}
	
	[RPC]
	void TellOthersImNotStunned(string myTag)
	{
		GameObject player = GameObject.FindGameObjectWithTag(myTag);
		player.GetComponent<PlayerActions>().Unstunned();
	}
	
	bool PlayerCanOpen()
	{
		return !GetComponent<PlayerMovement>().IsMoving();
	}
	
	public bool IsSwinging()
	{
		return swinging;
	}
	
	public bool IsStunned()
	{
		return isStunned;
	}
	
	public void StunMe()
	{
		swinging = false;
		isStunned = true;
	}
	
	public void Unstunned()
	{
		isStunned = false;
	}
}