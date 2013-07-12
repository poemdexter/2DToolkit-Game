using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 1.0f;	
	public float jumpPower = 100.0f;
	private bool canJump = false;
	private bool canMove = true;
	private Vector3 movement;
	private Vector3 force;
	
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () 
	{	
		if(networkView.isMine)
		{
			movement = Vector3.zero;
			if (canMove)
			{
				if (Input.GetKeyDown(KeyCode.Space) && canJump)
				{
					force = Vector3.up * jumpPower;
					canJump = false;
				}
				if (Input.GetKey(KeyCode.A))
				{
					movement += Vector3.left * speed * Time.deltaTime;
				}
				if (Input.GetKey(KeyCode.D))
				{
					movement += Vector3.right * speed * Time.deltaTime;
				}
				
				networkView.RPC("TellTheWorld", RPCMode.OthersBuffered, movement, force);
			}
		}
		
		if (movement != Vector3.zero || force != Vector3.zero)
		{
			transform.position += movement;
			rigidbody.AddForce(force);
			force = Vector3.zero;
		}
	}
	
	[RPC]
	void TellTheWorld(Vector3 myMovement, Vector3 myForce)
	{
		movement = myMovement;
		force = myForce;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Ground")
		{
			canJump = true;
			canMove = true;
		}
	}
	
	public void DisableMovement()
	{
		canMove = false;
	}
}
