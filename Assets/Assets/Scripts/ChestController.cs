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
	
    public void OpenChest()
	{
		anim.Play("opened");
		state = ChestState.opened;
	}
	
	 public void CloseChest()
	{
		anim.Play("closed");
		state = ChestState.closed;
	}
	
	public void TakeContents()
	{
		anim.Play("taken");
		state = ChestState.taken;
	}
	
	public ChestState GetChestState()
	{
		return state;
	}
}
