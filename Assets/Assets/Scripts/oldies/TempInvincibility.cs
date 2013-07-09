using UnityEngine;
using System.Collections;

public class TempInvincibility : MonoBehaviour {
	
	public bool activated = false;
	public double invLength = 1.5;
	private double currentLength = 0;
	public double blinkDelay = .05;
	private double blinkTimer = 0;
	
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
				renderer.enabled = true;
				currentLength = 0;
			}
			else
			{
				if (blinkTimer > blinkDelay)
				{
					renderer.enabled = !renderer.enabled;
					blinkTimer = 0;
				}
				blinkTimer += Time.deltaTime;
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
