using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

	#region Variables
	public GameObject waveMaker;

	#endregion

	void Start () {
		ArtificialWave();
	}
	

	void Update () {
		
	}

	void ArtificialWave()
	{
		Destroy(Instantiate(waveMaker, Camera.main.ViewportToWorldPoint(new Vector2(1, 0.5f)), Quaternion.identity), 10f);
	}
}
