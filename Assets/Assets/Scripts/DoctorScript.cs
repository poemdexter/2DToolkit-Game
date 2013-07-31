using UnityEngine;
using System.Collections;

public class DoctorScript : MonoBehaviour {
	
	private tk2dSprite sprite;
	private tk2dSpriteAnimator anim;
	
	public float speed = 10;
	public int direction = 1; // right
	private float distToGround;
	
	public float gravity = 3.0f;
	private float gravityTotal;
	private bool isFalling = true; // start off falling to ground
	public float terminalVelocity = 50.0f;
	private Vector3 moveDirection;
	
	private double wallCollideTimer = 0;
	private double wallCollideMaxWait = 1;
	
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<tk2dSpriteAnimator>();
		anim.Play("walking");
		distToGround = collider.bounds.extents.y;
		gravityTotal = gravity;
		sprite = GetComponent<tk2dSprite>();
	}
	
	// Update is called once per frame
	void Update () 
	{	
		// move
		moveDirection = Vector3.zero;
		moveDirection.x += direction * speed * Time.deltaTime;
		
		if (IsGrounded())
		{
			gravityTotal = gravity;
			isFalling = false;
		}
		else
		{
			isFalling = true;
		}
		
		ApplyGravity();
		
		rigidbody.MovePosition(rigidbody.position + moveDirection);
		
		// sprite
		sprite.FlipX = (direction < 0) ? true : false;
		
		// wall collision timer
		wallCollideTimer += Time.deltaTime;
	}
	
	void ApplyGravity()
	{
		if (gravityTotal >= terminalVelocity) gravityTotal = terminalVelocity;
		if (isFalling)
		{
			gravityTotal += gravity;
			moveDirection.y -= gravityTotal * Time.smoothDeltaTime;
		}
		
	}
	
	bool IsGrounded()
	{
	    return Physics.Raycast(transform.position, - Vector3.up, distToGround + 0.1f);
	}
	
	 void OnCollisionEnter(Collision collision)
	{
		if (CanHitWallByTimer() && collision.collider.gameObject.layer == 11)
		{
			SwitchDirection();
			wallCollideTimer = 0;
		}
	}
	
	bool CanHitWallByTimer()
	{
		return (wallCollideTimer >= wallCollideMaxWait);
	}
	
	void SwitchDirection()
	{
		direction = -direction;
	}
}
