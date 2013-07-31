using UnityEngine;
using System.Collections;

public class DoctorSpawner : MonoBehaviour {

	public GameObject L_doctorPrefab;
	public GameObject R_doctorPrefab;
	private bool gameJustStarted = false;
	private bool canSpawnDoctors = false;
	public double delayBetweenSpawns = 1.0;
	public double delayBeforeStartSpawns = 1.0;
	public double delayBeforeTimer = 0;
	public double delayBetweenTimer = 0;
	
	void Start () 
	{
		
	}
	
	void Update () 
	{	
		if (PlayerInfo.gameStarted)
		{
			if (!gameJustStarted) gameJustStarted = true;
			else if ((delayBeforeTimer += Time.deltaTime) >= delayBeforeStartSpawns) 
				canSpawnDoctors = true;
			
			if (canSpawnDoctors && (delayBetweenTimer += Time.deltaTime) >= delayBetweenSpawns)
			{
				delayBetweenTimer = 0;
				if (Network.isServer)
				{
					if (Random.Range(-10,10) >= 0)
						Network.Instantiate(L_doctorPrefab, transform.position, Quaternion.identity, 0);
					else
						Network.Instantiate(R_doctorPrefab, transform.position, Quaternion.identity, 0);
				}
			}
		}
	}
}
