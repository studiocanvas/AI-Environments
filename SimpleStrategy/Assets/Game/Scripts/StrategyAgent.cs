using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyAgent : Agent
{
	BuildCtrls buildCtrl;
	BuildCtrls.BuildUnit buildUnit = BuildCtrls.BuildUnit.None;
	int charSelect = -1;
	int zoneSelect = 0;
	int prevKills;
	int prevDeaths;

	public override void InitializeAgent()
	{
		buildCtrl = GetComponent<BuildCtrls> ();
	}

	public override void CollectObservations()
	{
		// UI
		AddVectorObs(GameManager.loaders);
		AddVectorObs(GameManager.golds);

		// Building positions
		AddVectorObs(GameManager.places);

		// Character Movements
//		foreach (Vector3 charPos in GameManager.charPos) {
//			AddVectorObs (charPos);
//		}
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int action = Mathf.FloorToInt(vectorAction[0]);
		//Debug.Log (action);
		// Building
		switch (action)
		{
		case -1:
			buildUnit = BuildCtrls.BuildUnit.Miner;
			break;
		case -2:
			buildUnit = BuildCtrls.BuildUnit.Barracks;
			break;
		case -3:
			buildUnit = BuildCtrls.BuildUnit.Turret;
			break;
		case -4:
			buildCtrl.CreateChar(BuildCtrls.BuildUnit.Soldier);
			break;
		case -5:
			buildUnit = BuildCtrls.BuildUnit.None;
			int tryCount = 0;
			do {
				charSelect++;
				charSelect = charSelect%buildCtrl.charStores.Length;
				tryCount++;
				if (tryCount > buildCtrl.charStores.Length) {
					charSelect = -1;
					break;
				}
			} while (!buildCtrl.charStores [charSelect] || !buildCtrl.charStores [charSelect].isActive);
			break;
		case -6:
			zoneSelect = 0;
			break;
		case -7:
			zoneSelect = 1;
			break;
		case -8:
			zoneSelect = 2;
			break;
		case -9:
			zoneSelect = 3;
			break;
		}


		if (action > 0) {
			if (buildUnit != BuildCtrls.BuildUnit.None) {
				//Debug.Log (action);
				buildCtrl.BuildConfirm (buildUnit, buildCtrl.zone.places [action - 1].transform);
				buildUnit = BuildCtrls.BuildUnit.None;
			}

			// Attack
			if (charSelect >= 0) {
				buildCtrl.charStores [charSelect].gameObject.GetComponent<CharBot> ().Walk (GameManager.zones[zoneSelect].places [action - 1].transform.position);
			}
		}

		// Reward for destroying enemy units
		if (buildCtrl.kills > prevKills) {
			prevKills = buildCtrl.kills;
			AddReward(1f / 3000f);
		}

		// Penalty for losing units
		if (buildCtrl.deaths > prevDeaths) {
			prevDeaths = buildCtrl.deaths;
			AddReward(-1f / 3000f);
		}

		if (!buildCtrl.castle.isActive) {
			Done ();
			SetReward(-1f);
		}
	}

	public override void AgentReset()
	{
		// Castle Destroyed
		GameManager.ResetGame();
		prevKills = 0;
		prevDeaths = 0;
		charSelect = -1;
	}
}
