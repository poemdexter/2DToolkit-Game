using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 100.0f;	
	public float jumpPower = 20.0f;
	public float gravity = 3.0f;
	public float terminalVelocity = 50.0f;
	public float extraJumpPowerTime = 1.0f;
	private float upVelocity = 0;
	private bool canMove = true;
	private bool canJump = false;
	private bool isJumping = false; 
	private bool isFalling = true; // start off falling to ground
	private Vector3 position;
	private Vector3 moveDirection;
	private float gravityTotal;
	private float jumpTotalTime;
	private tk2dSprite sprite;
	private tk2dSpriteAnimator anim;
	private bool walking = false;
	private bool spriteFlipped = false;
	private string animationClip;
	private CharacterController controller;
	
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
				controller = GetComponent<CharacterController>();
				moveDirection = Vector3.zero;
				
				if (Input.GetButton("Horizontal"))
				{
					moveDirection.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
					sprite.FlipX = (Input.GetAxis("Horizontal") < 0) ? true : false;
					spriteFlipped = sprite.FlipX;
					
					if (!walking) walking = true;
				}
				else 
					if (walking) walking = false;
				
				if (controller.isGrounded)
				{
					gravityTotal = gravity;
					isJumping = false;
					jumpTotalTime = 0;
					isFalling = false;
					canJump = true;
					upVelocity = 0;
				}
				else
				{
					isFalling = true;
				}
				
				ApplyJumping();
				
				ApplyGravity();
				
				// jumpPower determined by ApplyJumping()
				moveDirection.y += upVelocity * Time.deltaTime;
				
				controller.Move(moveDirection);
				anim.Play(animationClip);
				
				networkView.RPC("BroadcastPosition", RPCMode.AllBuffered, transform.position);
				
				if (isJumping) animationClip = "jumping";
				else if (walking) animationClip = "walking";
				else animationClip = "standing";
				
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
	
	void ApplyJumping()
	{
		// if grounded and jump
		if (canJump && Input.GetButton("Jump"))
		{
			jumpTotalTime = 0;
			isJumping = true;
			canJump = false;
		}
		// else if still holding jump
		else if (isJumping && Input.GetButton("Jump"))
		{
			jumpTotalTime += Time.deltaTime;
		}
		else isJumping = false; // else let go of jump
		
		// hit ceiling? then fall now
		if (IsTouchingCeiling()) isJumping = false;
		
		if (isJumping) upVelocity = jumpPower; 
	}
	
	void ApplyGravity()
	{
		// not jumping or not super jumping or falling, so apply gravity
		if (!isJumping || jumpTotalTime >= extraJumpPowerTime)
		{
			if (gravityTotal >= terminalVelocity) gravityTotal = terminalVelocity;
			if (isFalling) gravityTotal += gravity;
			moveDirection.y -= gravityTotal * Time.deltaTime;
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
		if (hit.collider.tag == "Ground") canMove = true;
	}
	
	public void DisableMovement()
	{
		canMove = false;
	}
	
	bool IsTouchingCeiling()
	{
		return (controller.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
}
