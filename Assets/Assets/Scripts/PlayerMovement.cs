using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 100.0f;	
	public float jumpPower = 8.0f;
	public float gravity = 5.0f;
	public float terminalVelocity = 50.0f;
	private bool canMove = true;
	private bool canJump = false;
	private Vector3 position;
	private Vector3 moveDirection;
	private float gravityTotal;
	private float jumpTotal;
	
	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () 
	{	
		// it's us, lets do things legit
		if(networkView.isMine)
		{
			if (canMove)
			{
				CharacterController controller = GetComponent<CharacterController>();
				moveDirection = Vector3.zero;
				
				if (controller.isGrounded)
				{
					canJump = true;
					jumpTotal = jumpPower;
					gravityTotal = 0;
				}
				
				if (canJump && Input.GetButton("Jump"))
				{
					Debug.Log("jump");
					moveDirection.y = jumpPower;
					jumpPower -= gravity * Time.deltaTime;
				}
				else canJump = false;
				
				if (Input.GetButton("Horizontal"))
				{
					moveDirection.x += Input.GetAxis("Horizontal") * speed;
				}
				
				// todo acceleration due to gravity calculations
				gravityTotal += gravity * Time.deltaTime;
				if (gravityTotal >= terminalVelocity) gravityTotal = terminalVelocity;
				moveDirection.y -= gravityTotal;
				
				controller.Move(moveDirection * Time.deltaTime);
				networkView.RPC("TellTheWorld", RPCMode.AllBuffered, transform.position);
			}
		}
		else
		{
			// other player, just update position
			transform.position = position;
		}
	}
	
	[RPC]
	void TellTheWorld(Vector3 myPosition)
	{
		position = myPosition;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Ground")
		{
			canMove = true;
		}
	}
	
	public void DisableMovement()
	{
		canMove = false;
	}
}
