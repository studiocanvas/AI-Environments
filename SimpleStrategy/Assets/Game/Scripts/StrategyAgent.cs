using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategyAgent : Agent
{
	BuildCtrls buildCtrl;
	BuildCtrls.BuildUnit buildUnit = BuildCtrls.BuildUnit.None;
	int charSelect = -1;
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
//		foreach (Vector3 charPos in buildCtrl.gameManager.charPos) {
//			AddVectorObs (charPos.x);
//			AddVectorObs (charPos.z);
//		}
	}

	/// <summary>
	/// Snap the specified inFloat to inSnap.
	/// </summary>
	/// <param name="inFloat">In float.</param>
	/// <param name="inSnap">In snap.</param>
	int Snap (float inFloat, float inSnap) {
		float f = Mathf.Clamp01(inFloat); 
		float snap = 1f / inSnap; 
		int i = Mathf.RoundToInt(f / snap); 
		return i;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		
		//txtDebug.text = "raw:" + Snap (vectorAction [0], 5f).ToString () +"\n";

		int selectAction = Snap (vectorAction [0], 5f);
		int buildAction = Snap (vectorAction [1], 24f);
		int charSelect = Snap (vectorAction [2], 50f);
		int attackAction = Snap (vectorAction [3], 99f);

		// Building
		switch (selectAction) {
		case 1:
			buildUnit = BuildCtrls.BuildUnit.Miner;
			break;
		case 2:
			buildUnit = BuildCtrls.BuildUnit.Barracks;
			break;
		case 3:
			buildUnit = BuildCtrls.BuildUnit.Turret;
			break;
		case 4:
			buildCtrl.CreateChar (BuildCtrls.BuildUnit.Soldier);
			break;
		case 5:
			buildUnit = BuildCtrls.BuildUnit.None;
			break;

		}
		if (attackAction > 0 && charSelect > 0) {
			// Control single soldier
			if (buildCtrl.charStores [charSelect - 1] && buildCtrl.charStores [charSelect - 1].isActive) {
				int zoneSelect = Snap (vectorAction [1], 4f) - 1;
				if (zoneSelect >= 0) {
					int placesLength = buildCtrl.gameManager.zones [zoneSelect].places.Length;
					int attackPlace = attackAction - (placesLength * zoneSelect);
					attackPlace = Mathf.Clamp (attackPlace,0 , placesLength - 1);
					//txtDebug.text = "z:"+ zoneSelect.ToString ()+" p:"+attackPlace.ToString();

					buildCtrl.charStores [charSelect - 1].gameObject.GetComponent<CharBot> ().Walk (buildCtrl.gameManager.zones [zoneSelect].places [attackPlace].transform.position);
				}
			}
		}


		if (buildAction > 0) {
			//Debug.Log (action);
			//txtDebug.text = "place:"+ buildAction.ToString ();

			if (buildUnit != BuildCtrls.BuildUnit.None) {
				//Debug.Log (action);
				buildCtrl.BuildConfirm (buildUnit, buildCtrl.zone.places [buildAction - 1].transform);
				buildUnit = BuildCtrls.BuildUnit.None;
			}
		}

		AddReward(-0.01f);

		// Reward for destroying enemy units
		if (buildCtrl.kills != prevKills) {
			prevKills = buildCtrl.kills;
			AddReward(1f / 3f);
			currentSteps = 0;
		}
		if (buildCtrl.creates != prevCreates) {
			prevCreates = buildCtrl.creates;
			//AddReward(1f / 4f);
			currentSteps = 0;
		}

		// Penalty for losing units
		if (buildCtrl.deaths != prevDeaths) {
			prevDeaths = buildCtrl.deaths;
			AddReward(-1f / 3f);
			currentSteps = 0;
		}

//		// Timeout
//		if (buildCtrl.castle.isActive && action != 0) {
//			currentSteps++;
//			txtDebug.text += "\nsteps: "+currentSteps.ToString ();
//			//AddReward(-1f / 100f);
//			if (currentSteps > maxSteps) {
//				//Done ();
//				buildCtrl.castle.Died ();
//				buildCtrl.SetupBuildings ();
//			}
//		}

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
