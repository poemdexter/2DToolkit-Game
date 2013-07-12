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
	private tk2dSprite sprite;
	private tk2dSpriteAnimator anim;
	private bool walking = false;
	private bool spriteFlipped = false;
	private string animationClip;
	
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<tk2dSpriteAnimator>();
		animationClip = "standing";
	}
	
	// Update is called once per frame
	void Update () 
	{	
		sprite = GetComponent<tk2dSprite>();
		
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
					moveDirection.y = jumpTotal;
					jumpTotal -= gravity * Time.deltaTime;
					if (jumpTotal < 0) jumpTotal = 0;
				}
				else 
				{
					canJump = false;
				}
				
				if (Input.GetButton("Horizontal"))
				{
					moveDirection.x += Input.GetAxis("Horizontal") * speed;
					sprite.FlipX = (Input.GetAxis("Horizontal") < 0) ? true : false;
					spriteFlipped = sprite.FlipX;
					
					if (!walking)
					{
						animationClip = "walking";
						walking = true;
					}
				}
				else
				{
					if (walking)
					{
						animationClip = "standing";
						walking = false;
						
					}
				}
				
				// todo acceleration due to gravity calculations
				gravityTotal += gravity * Time.deltaTime;
				if (gravityTotal >= terminalVelocity) gravityTotal = terminalVelocity;
				moveDirection.y -= gravityTotal;
				
				controller.Move(moveDirection * Time.deltaTime);
				anim.Play(animationClip);
				
				networkView.RPC("BroadcastPosition", RPCMode.AllBuffered, transform.position);
				networkView.RPC("BroadcastAnimation", RPCMode.AllBuffered, spriteFlipped, animationClip);
			}
		}
		else // other player
		{
			//update position
			transform.position = position;
			
			//update animation
			sprite.FlipX = spriteFlipped;
			anim.Play(animationClip);
		}
	}
	
	[RPC]
	void BroadcastPosition(Vector3 myPosition)
	{
		position = myPosition;
	}
	
	[RPC]
	void BroadcastAnimation(bool flipped, string clip)
	{
		spriteFlipped = flipped;
		animationClip = clip;
	}
	
	// none of this works!
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.collider.tag == "Ground")
		{
			canMove = true;
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			ChestController chest = collider.GetComponent<ChestController>();
			if (chest.GetChestState() == ChestState.closed) chest.OpenChest();
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Treasure")
		{
			ChestController chest = collider.GetComponent<ChestController>();
			if (chest.GetChestState() == ChestState.opened) chest.CloseChest();
		}
	}
	
	public void DisableMovement()
	{
		canMove = false;
	}
}
