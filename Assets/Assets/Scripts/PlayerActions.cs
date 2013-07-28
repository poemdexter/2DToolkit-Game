using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour 
{
	public double secondsToOpen = 3;
	
//	private bool canOpenChest = true;
//	private bool openingChest = false;
	private ChestController attemptedChest;
//	private double openTime = 0;
	
	public double maxSwingTime = 0.2; // this must be equal to length of swing animation
	private double currentSwingTime = 0.0;
	private bool swinging = false; // controls animation and part of timing logic
	private bool needNewPress = false; // ensures we're pressing the swing key again after a full length swing
	
	void Update()
	{
		if (networkView.isMine)
		{
			// swing button; we current force player to tap swing everytime he wants to swing again and he has to go for entire animation
			if (PlayerInfo.gameStarted && Input.GetButton("Open"))
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
					CheckCollisionsWithOtherPlayers();
					
					currentSwingTime += Time.deltaTime;
				}
				else // we're at max swing time
				{
					swinging = false;
					currentSwingTime = 0;
					needNewPress = true;
				}
			}
			
//			if (canOpenChest && Input.GetButton("Open"))
//			{
//				// swing animation
//				openingChest = true;
//				
//				// we swung bat, let's see if we hit another player
//				if (true)
//				{
//					
//				}
//				else // lets see if we hit child
//				{
//					if (!openingChest) // haven't started opening yet, so let's start
//					{
//						openingChest = true;
//					}
//					else // we're already opening so keep going until unlocked
//					{
//						// we have chest to attempt to open and it's closed so 'pick lock' for X sec then pop!
//						if (attemptedChest != null && attemptedChest.GetChestState() == ChestState.closed)
//						{
//							openTime += Time.deltaTime;
//							if (openTime >= secondsToOpen) attemptedChest.OpenChest();
//						}
//					}
//				}
//			}
//			else if (openingChest) // we stopped holding the open button
//			{
//				openingChest = false;
//				openTime = 0;
//			}
		}
	}
	
	void GetOtherPlayerBounds()
	{
		
	}
	
	void CheckCollisionsWithOtherPlayers()
	{
//		if (collider.bounds.Intersects( ))
//		{
//			// collision
//		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			attemptedChest = collider.GetComponent<ChestController>();
//			canOpenChest = (attemptedChest.GetChestState() == ChestState.closed) ? true : false;
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			attemptedChest = null;
//			canOpenChest = false;
//			openingChest = false;
//			openTime = 0;
		}
	}
	
	bool PlayerCanOpen()
	{
		return !GetComponent<PlayerMovement>().IsMoving();
	}
	
	public bool IsSwinging()
	{
		return swinging;
	}
}