using UnityEngine;
using System.Collections;

public enum ChestState
{
	closed,
	opened,
	taken
}

public class ChestController : MonoBehaviour 
{
	// Link to the animated sprite
    private tk2dSpriteAnimator anim;
	private ChestState state = ChestState.closed;
	
    void Start () 
	{
        anim = GetComponent<tk2dSpriteAnimator>();
		anim.Play(state.ToString());
    }
	
	public ChestState GetChestState()
	{
		return state;
	}
	
    public void OpenChest()
	{
		anim.Play("opened");
		state = ChestState.opened;
		networkView.RPC("BroadcastChestState", RPCMode.AllBuffered, "opened");
	}
	
	 public void CloseChest()
	{
		anim.Play("closed");
		state = ChestState.closed;
		networkView.RPC("BroadcastChestState", RPCMode.AllBuffered, "closed");
	}
	
	public void TakeContents()
	{
		anim.Play("taken");
		state = ChestState.taken;
		networkView.RPC("BroadcastChestState", RPCMode.AllBuffered, "taken");
	}
	
	[RPC]
	void BroadcastChestState(string newState)
	{
		anim.Play(newState);
		state = ConvertToState(newState);
	}
	
	ChestState ConvertToState(string state)
	{
		switch(state)
		{
			case "opened": return ChestState.opened;
			case "closed": return ChestState.closed;
			case "taken": return ChestState.taken;
			default: return ChestState.closed;
		}
	}
}
