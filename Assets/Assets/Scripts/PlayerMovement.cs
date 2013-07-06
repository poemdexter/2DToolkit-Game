using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float speed = 1.0f;	
	public float jumpPower = 100.0f;
	private bool canJump = false;
	private bool canMove = true;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (canMove)
		{
			if (Input.GetKeyDown(KeyCode.Space) && canJump)
			{
				rigidbody.AddForce(Vector3.up * jumpPower);
				canJump = false;
			}
			if (Input.GetKey(KeyCode.A))
			{
				transform.position += Vector3.left * speed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				transform.position += Vector3.right * speed * Time.deltaTime;
			}
		}
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
