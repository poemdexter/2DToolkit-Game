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
	private float gravityTotal;
	private float jumpTotalTime;
	private bool canMove = true;
	private bool canJump = false;
	private bool isJumping = false; 
	private bool isFalling = true; // start off falling to ground
	private bool walking = false;
	private bool spriteFlipped = false;
	private Vector3 position;
	private Vector3 moveDirection;
	private tk2dSprite sprite;
	private tk2dSpriteAnimator anim;	
	private string animationClip;
	private CharacterController controller;
	private PlayerActions actions;
	
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
			actions = GetComponent<PlayerActions>();
			
			if (canMove)
			{
				controller = GetComponent<CharacterController>();
				moveDirection = Vector3.zero;
				
				if (PlayerInfo.gameStarted && !actions.IsStunned() && Input.GetButton("Horizontal"))
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
				
				// tell network of new position
				networkView.RPC("BroadcastPosition", RPCMode.AllBuffered, transform.position);
				
				if (actions.IsStunned()) animationClip = "stunned";
				else if (actions.IsSwinging()) animationClip = "picking";
				else if (walking) animationClip = "walking";
				else if (isJumping) animationClip = "jumping";
				else animationClip = "standing";
				
				anim.Play(animationClip);
				
				networkView.RPC("BroadcastAnimation", RPCMode.AllBuffered, spriteFlipped, animationClip);
				
				// keeps us on the correct z
				Vector3 pos = transform.position;
				pos.z = 0;
				transform.position = pos;
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
		if (PlayerInfo.gameStarted && !actions.IsStunned() && canJump && Input.GetButton("Jump"))
		{
			jumpTotalTime = 0;
			isJumping = true;
			canJump = false;
		}
		// else if still holding jump
		else if (PlayerInfo.gameStarted && !actions.IsStunned() && isJumping && Input.GetButton("Jump"))
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
	
	public bool IsMoving()
	{
		return walking || isJumping || isFalling;
	}
	
	public bool IsSpriteFlipped()
	{
		return spriteFlipped;
	}
}
