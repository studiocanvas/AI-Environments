using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategyAgent : Agent
{
	BuildCtrls buildCtrl;
	BuildCtrls.BuildUnit buildUnit = BuildCtrls.BuildUnit.None;
	int charSelect = -1;
	int zoneSelect = 0;
	int prevKills;
	int prevDeaths;
	int prevCreates;
	int maxSteps = 2000;
	int currentSteps = 0;

	Text txtDebug;

	public override void InitializeAgent()
	{
		buildCtrl = GetComponent<BuildCtrls> ();
		txtDebug = transform.Find("Text Debug").gameObject.GetComponent<Text>();
	}

	public override void CollectObservations()
	{
		// UI
		AddVectorObs(buildCtrl.gameManager.loaders);
		AddVectorObs(buildCtrl.gameManager.golds);

		// Building positions
		AddVectorObs(buildCtrl.gameManager.places);

		// Character Movements
//		foreach (Vector3 charPos in GameManager.charPos) {
//			AddVectorObs (charPos);
//		}
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int action = Mathf.FloorToInt(vectorAction[0]);

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
			// Control all soldiers
			buildUnit = BuildCtrls.BuildUnit.None;

			// Control single soldier
//			int tryCount = 0;
//			do {
//				charSelect++;
//				charSelect = charSelect%buildCtrl.charStores.Length;
//				tryCount++;
//				if (tryCount > buildCtrl.charStores.Length) {
//					charSelect = -1;
//					break;
//				}
//			} while (!buildCtrl.charStores [charSelect] || !buildCtrl.charStores [charSelect].isActive);
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
			//Debug.Log (action);
			txtDebug.text = "action:"+ action.ToString ();

			if (buildUnit != BuildCtrls.BuildUnit.None) {
				//Debug.Log (action);
				buildCtrl.BuildConfirm (buildUnit, buildCtrl.zone.places [action - 1].transform);
				buildUnit = BuildCtrls.BuildUnit.None;
			}

			// Attack
			// Control all soldiers
			if (buildUnit == BuildCtrls.BuildUnit.None) {
				foreach (Unit charStore in buildCtrl.charStores) {
					if (charStore && charStore.isActive) {
						charStore.gameObject.GetComponent<CharBot> ().Walk (buildCtrl.gameManager.zones [zoneSelect].places [action - 1].transform.position);
					}
				}
			}
			// Control single soldier
//			if (charSelect >= 0) {
//				buildCtrl.charStores [charSelect].gameObject.GetComponent<CharBot> ().Walk (buildCtrl.gameManager.zones[zoneSelect].places [action - 1].transform.position);
//			}
		}

		//AddReward(-0.01f);

		// Reward for destroying enemy units
		if (buildCtrl.kills != prevKills) {
			prevKills = buildCtrl.kills;
			AddReward(1f / 3f);
			currentSteps = 0;
		}
		if (buildCtrl.creates != prevCreates) {
			prevCreates = buildCtrl.creates;
			AddReward(1f / 4f);
			currentSteps = 0;
		}

		// Penalty for losing units
		if (buildCtrl.deaths != prevDeaths) {
			prevDeaths = buildCtrl.deaths;
			AddReward(-1f / 3f);
			currentSteps = 0;
		}

		// Timeout
		if (buildCtrl.castle.isActive && action != 0) {
			currentSteps++;
			txtDebug.text += "\nsteps: "+currentSteps.ToString ();
			//AddReward(-1f / 100f);
			if (currentSteps > maxSteps) {
				//Done ();
				buildCtrl.castle.Died ();
				buildCtrl.SetupBuildings ();
			}
		}

		// lose game
		if (!buildCtrl.castle.isActive) {
			AddReward (-2f);
			currentSteps = 0;
		}

		// Reset Game
		int aliveCount = 0;
		for (int i = 0; i < buildCtrl.gameManager.buildCtrls.Length; i++) {
			if (buildCtrl.gameManager.buildCtrls [i].castle.isActive) {
				aliveCount++;
			}
		}
		if (aliveCount <= 1) {
			if (buildCtrl.castle.isActive) {
				AddReward (2f);
			}
			currentSteps = 0;
			Done ();
		}
	}

	public override void AgentReset()
	{
		// Castle Destroyed
		buildCtrl.gameManager.ResetGame();
		prevKills = 0;
		prevDeaths = 0;
		charSelect = -1;
		currentSteps = 0;
	}
}
