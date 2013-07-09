using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public void Move(Vector3 deltaPosition)
	{
		transform.position += deltaPosition;
	}
}
