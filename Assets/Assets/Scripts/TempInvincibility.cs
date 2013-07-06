using UnityEngine;
using System.Collections;

public class TempInvincibility : MonoBehaviour {
	
	public bool activated = false;
	private double invLength = 3;
	private double currentLength = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (activated)
		{
			if (currentLength > invLength)
			{
				DeActivate();
				currentLength = 0;
			}
			else
			{
				currentLength += Time.deltaTime;
			}
		}
	}
	
	public void Activate()
	{
		activated = true;
		Physics.IgnoreLayerCollision(8,9,true);
	}
	
	public void DeActivate()
	{
		activated = false;
		Physics.IgnoreLayerCollision(8,9,false);
		this.rigidbody.WakeUp();
	}
}
