using UnityEngine;
using System.Collections;

public class IgnoreCollisions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Physics.IgnoreLayerCollision(8,8,true); // player on player
		//Physics.IgnoreLayerCollision(8,10,true); // player on chests
	}
}
