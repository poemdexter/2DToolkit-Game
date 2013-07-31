using UnityEngine;
using System.Collections;

public class DoctorDespawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			if (Network.isServer) Network.Destroy(other.gameObject);
		}
	}
}
