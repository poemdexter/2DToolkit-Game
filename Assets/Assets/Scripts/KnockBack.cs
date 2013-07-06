using UnityEngine;
using System.Collections;

public class KnockBack : MonoBehaviour 
{	
	public float knockbackPower = 100.0f;
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("e hit");
		DoKnockback(collision);
	}
	
	void OnCollisionStay(Collision collision)
	{
		Debug.Log("s hit");
		DoKnockback(collision);
	}
	
	void DoKnockback(Collision collision)
	{
		if (collision.collider.tag == "Player")
		{
			Debug.Log("stay collision");
			if(!collision.collider.GetComponent<TempInvincibility>().activated)
			{
				
				collision.collider.GetComponent<TempInvincibility>().Activate();
				collision.collider.transform.rigidbody.velocity = Vector3.zero;
				int x = GetDirectionOfKnockback(collision.collider.transform.position.x, this.transform.position.x);
				collision.collider.transform.rigidbody.AddForce(new Vector3(x,1,0) * knockbackPower);
				collision.collider.GetComponent<PlayerMovement>().DisableMovement();
			}
		}
	}
	
	int GetDirectionOfKnockback(float playerX, float enemyX)
	{
		return (playerX < enemyX) ? -1 : 1;
	}
}
