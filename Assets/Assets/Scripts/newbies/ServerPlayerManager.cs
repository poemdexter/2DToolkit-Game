using UnityEngine;
using System.Collections;

[RequireComponent(typeof (NetworkView))]
public class ServerPlayerManager : MonoBehaviour {

	public float speed = 5;
	private PlayerController controller;
	private bool aPress;
	private bool dPress;
	private bool spacePress;
	
	void Start()
	{
		if (Network.isServer)
		{
			controller = GetComponent<PlayerController>();
		}
	}
	
	void Update()
	{
		if (Network.isServer)
		{
			Vector3 left = Vector3.zero;
			Vector3 right = Vector3.zero;
			if (aPress) left = Vector3.left * speed * Time.deltaTime;
			if (dPress) right = Vector3.right * speed * Time.deltaTime;
			controller.Move(left + right);
		}
	}
	
	[RPC]
	public void UpdateClientMotion(bool a, bool d, bool space) 
	{
		aPress = a;
		dPress = d;
		spacePress = space;
	}
}
