using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour 
{
	// Link to the animated sprite
    private tk2dSpriteAnimator anim;
	int timer = 0;

    void Start () 
	{
        anim = GetComponent<tk2dSpriteAnimator>();
    }

    void Update () 
	{
		if (timer++ == 100) timer = 0;
		
		if (timer < 33) anim.Play("closed");
		else if (timer < 66) anim.Play("opened");
		else anim.Play ("taken");
	}
}
