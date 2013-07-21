using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour 
{
	public double secondsToOpen = 3;
	
	private bool canOpenChest = false;
	private bool openingChest = false;
	private ChestController attemptedChest;
	private double openTime = 0;
	
	void Update()
	{
		if (networkView.isMine)
		{
			if (canOpenChest && PlayerCanOpen() && Input.GetButton("Open"))
			{
				if (!openingChest) // haven't started opening yet, so let's start
				{
					openingChest = true;
				}
				else // we're already opening so keep going until unlocked
				{
					// we have chest to attempt to open and it's closed so 'pick lock' for X sec then pop!
					if (attemptedChest != null && attemptedChest.GetChestState() == ChestState.closed)
					{
						openTime += Time.deltaTime;
						if (openTime >= secondsToOpen) attemptedChest.OpenChest();
					}
				}
			}
			else if (openingChest) // we stopped holding the open button
			{
				openingChest = false;
				openTime = 0;
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			attemptedChest = collider.GetComponent<ChestController>();
			canOpenChest = (attemptedChest.GetChestState == ChestState.closed) ? true : false;
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			attemptedChest = null;
			canOpenChest = false;
			openingChest = false;
			openTime = 0;
		}
	}
	
	bool PlayerCanOpen()
	{
		return !GetComponent<PlayerMovement>().IsMoving();
	}
	
	public bool IsPicking()
	{
		return openingChest;
	}
}