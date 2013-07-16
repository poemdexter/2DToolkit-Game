using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 100.0f;	
	public float jumpPower = 40.0f;
	public float shortJumpPower = 20.0f;
	public float gravity = 3.0f;
	public float terminalVelocity = 50.0f;
	private bool canMove = true;
	private bool canJump = false;
	private bool isJumping = false; 
	private bool isFalling = true; // start off falling to ground
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
		gravityTotal = gravity;
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
				
				if (Input.GetButton("Horizontal"))
				{
					moveDirection.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
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
				
				if (controller.isGrounded)
				{
					gravityTotal = gravity;
					isJumping = false;
					isFalling = false;
					canJump = true;
				}
				
				// if grounded and jump
				if (canJump && Input.GetButton("Jump"))
				{
					jumpTotal = jumpPower;
					isJumping = true;
					isFalling = true;
					canJump = false;
				}
				// else if still holding jump
				else if (isJumping && Input.GetButton("Jump"))
				{
					
				}
				// else let go of jump
				else 
				{
					if (isJumping) // still in the air
					{
						if (jumpTotal - gravityTotal > 0) // we're going up still
						{
							if (jumpTotal > shortJumpPower) jumpTotal = shortJumpPower;
						}
					}
				}
				
				if (isFalling)
				{
					moveDirection.y += jumpTotal * Time.deltaTime;
					gravityTotal += gravity;
				}
				
				if (gravityTotal >= terminalVelocity) gravityTotal = terminalVelocity;
				moveDirection.y -= gravityTotal * Time.deltaTime;
				
				controller.Move(moveDirection);
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
