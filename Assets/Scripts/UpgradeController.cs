using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour {

	#region Variables
	public enum Upgrade{MaxHealth, HealthDrop, CoinDrop, InvulTime, UPGRADE_COUNT}

	float[] upgradeValues = new float[(int)Upgrade.UPGRADE_COUNT];
	#endregion

	void Start () {
		
	}
	

	void Update () {
		
	}


	public void UpgradePlayer(int upgrade)
	{
		upgradeValues[upgrade]++;
	}

	public void UpgradePlayer(Upgrade u)
	{
		upgradeValues[(int)u]++;
	}
}
