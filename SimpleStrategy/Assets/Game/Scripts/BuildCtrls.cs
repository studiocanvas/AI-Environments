using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildCtrls : MonoBehaviour {
	public enum CtrlType {
		Mouse,
		Keyboard,
		Bot
	}
	public CtrlType ctrlType;

	BuildCtrls[] buildCtrls;

	public Zone zone;

	public GameObject[] buildPrefabs;
	public GameObject[] charPrefabs;
	public GameObject castlePrefab;
	public Unit[,] buildStores = new Unit[3,25];
	public Unit[] charStores = new Unit[50];
	Unit castle;
	public enum BuildUnit {
		Miner,
		Barracks,
		Turret,
		Soldier
	}
		
	BuildButton[] uiBuildButtons = new BuildButton[4];
	Text txtGold;

	int prevGold;
	int resetGold = 100;
	int gold;
	float mineTimer;
	float mineRate = 1f;
	int mineDig = 5;

	int[] costs = new int[] {100, 200,150,75};
	float[] timers = new float[] {0.01f, 0.001f,0.005f,0.01f};
	int[] dependents = new int[] {-1,0,0,1};
	int[] counters = new int[4];

	Selector selector;

	// Use this for initialization
	void Start () {
		buildCtrls = FindObjectsOfType<BuildCtrls> ();
		selector = GameObject.Find("Selector").GetComponent<Selector>();

		uiBuildButtons[(int)BuildUnit.Miner] = transform.Find("Button Miner").gameObject.GetComponent<BuildButton>();
		uiBuildButtons[(int)BuildUnit.Barracks]  = transform.Find("Button Barracks").gameObject.GetComponent<BuildButton>();
		uiBuildButtons[(int)BuildUnit.Turret]  = transform.Find("Button Turret").gameObject.GetComponent<BuildButton>();
		uiBuildButtons[(int)BuildUnit.Soldier]  = transform.Find("Button Soldier").gameObject.GetComponent<BuildButton>();
		txtGold = transform.Find("Text Gold").gameObject.GetComponent<Text>();

		for (int i = 0; i < uiBuildButtons.Length; i++) {
			uiBuildButtons [i].Init (timers[i]);
		}
		StartGame ();
	}

	void StartGame() {
		SetupCastle ();
		SetupBuildings ();
		SetupChars ();

		// Reset Counters
		gold = resetGold;
		counters = new int[4];
	}
	// Reset Castle
	void SetupCastle() {
		if (!castle) {
			GameObject newBuilding = Instantiate (castlePrefab, zone.startPlace.transform.position, Quaternion.identity);
			castle = newBuilding.GetComponent<Unit> ();
		}
		SetTarget (castle.transform, zone.startPlace.transform);
		castle.Reset (zone);
	}
	// Reset All Buildings
	void SetupBuildings() {
		for (int c = 0; c < buildStores.GetLength (0); c++) {
			for (int i = 0; i < buildStores.GetLength (1); i++) {
				if (buildStores [c, i]) {
					buildStores [c, i].Hide ();
				}
			}
		}
	}
	// Reset All Characters
	void SetupChars() {
		for (int i = 0; i < charStores.Length; i++) {
			if (charStores [i]) {
				charStores [i].Hide ();
			}
		}
	}
		
	void Update () {
		if (castle.isActive) {
			CtrlInput ();

			// Activate Buttons when dependant is built
			for (int i = 0; i < uiBuildButtons.Length; i++) {
				if (gold >= costs [i] && (dependents [i] < 0 || counters [dependents [i]] > 0)) {
					if (!uiBuildButtons [i].isActive) {
						uiBuildButtons [i].isActive = true;
						uiBuildButtons [i].StartLoad ();
					}
				} else {
					if (uiBuildButtons [i].isActive) {
						uiBuildButtons [i].isActive = false;
						uiBuildButtons [i].StopLoad ();
					}
				}
			}

			// Increase Soldier Production with more Barracks
			uiBuildButtons[(int)BuildUnit.Soldier].loadRate = GetBuildCount (BuildUnit.Barracks) * timers[(int)BuildUnit.Soldier];

			// Increase Gold Mining
			if (Time.time > mineTimer + mineRate * GameManager.globalSpeed) {
				mineTimer = Time.time;
				gold += GetBuildCount (BuildUnit.Miner) * mineDig;
			}
			if (prevGold != gold) {
				prevGold = gold;
				txtGold.text = gold.ToString ();
			}

		// Castle Destroyed, Game Over
		} else {
			// Restart Game
			foreach (BuildCtrls buildCtrl in buildCtrls) {
				buildCtrl.StartGame ();
			}
		}
	}

	/// <summary>
	/// UI Buttons input.
	/// </summary>
	/// <param name="inButton">In button.</param>
	public void ButtonInput ( Button inButton ) {
		Build (inButton.gameObject.name);
	} 
	/// <summary>
	/// Keyboard input.
	/// </summary>
	public void CtrlInput() {
		if (ctrlType == CtrlType.Keyboard) {
			if (Input.GetKeyUp (KeyCode.Alpha1) && uiBuildButtons [(int)BuildUnit.Miner].button.interactable) {
				Build ("Button Miner");
			}
			if (Input.GetKeyUp (KeyCode.Alpha2) && uiBuildButtons [(int)BuildUnit.Barracks].button.interactable) {
				Build ("Button Barracks");
			}
			if (Input.GetKeyUp (KeyCode.Alpha3) && uiBuildButtons [(int)BuildUnit.Turret].button.interactable) {
				Build ("Button Turret");
			}
			if (Input.GetKeyUp (KeyCode.Alpha4) && uiBuildButtons [(int)BuildUnit.Soldier].button.interactable) {
				Build ("Button Soldier");
			}
		}
	}
	void Build ( string inName ) {
		switch (inName) {
		case "Button Miner":
			StopAllCoroutines ();
			StartCoroutine (BuildStart (BuildUnit.Miner));
			break;
		case "Button Barracks":
			StopAllCoroutines ();
			StartCoroutine (BuildStart (BuildUnit.Barracks));
			break;
		case "Button Turret":
			StopAllCoroutines ();
			StartCoroutine (BuildStart (BuildUnit.Turret));
			break;
		case "Button Soldier":
			CreateChar (BuildUnit.Soldier);
			break;
		}
	}



	/// <summary>
	/// Creates the char.
	/// </summary>
	/// <param name="inBuildUnit">In build unit.</param>
	public void CreateChar ( BuildUnit inBuildUnit ) {
		gold -= costs[(int)inBuildUnit];
		counters [(int)inBuildUnit]++;

		int poolNum = CheckCharPool ();
		int spawnNum = GetRandomCharSpawn ();
		if (spawnNum >= 0) {
			Vector3 spawnPoint = buildStores [(int)BuildUnit.Barracks, spawnNum].transform.position;
			if (poolNum >= 0) {
				if (!charStores [poolNum]) {
					GameObject newChar = Instantiate (charPrefabs [0], spawnPoint, Quaternion.identity);
					charStores [poolNum] = newChar.GetComponent<Unit> ();
				}
				charStores [poolNum].Reset (zone);
				charStores [poolNum].transform.position = spawnPoint;
				CharBot charBot = charStores [poolNum].gameObject.GetComponent<CharBot> ();
				charBot.Walk (spawnPoint + new Vector3 (GetRandomDirection (), 0, GetRandomDirection ()));
			}
		}
	}
	/// <summary>
	/// Gets a random direction.
	/// </summary>
	/// <returns>The random direction.</returns>
	float GetRandomDirection() {
		int output = 1;
		if (Random.value < 0.5) {
			output = -1;
		}
		return output;
	}
	/// <summary>
	/// Gets the random char spawn point.
	/// </summary>
	/// <returns>Random char spawn point.</returns>
	int GetRandomCharSpawn() {
		int r = Random.Range (0, buildStores.GetLength (1));
		int i = 0;
		int t = 0;
		int s = -1;
		do {
			i++;
			i = i%(buildStores.GetLength(1)-1);
			if (buildStores [(int)BuildUnit.Barracks, i] && buildStores [(int)BuildUnit.Barracks, i].isActive) {
				t++;
				s = i;
			}
		} while (t < r);
		return s;
	}

	/// <summary>
	/// Find available slot in character pool.
	/// </summary>
	/// <returns>The char pool slot.</returns>
	int CheckCharPool() {
		for (int i = 0; i < charStores.Length; i++) {
			if (!charStores [i] || !charStores [i].isActive) {
				return i;
			}
		}
		return -1;
	}
	/// <summary>
	/// Minus character counter when destroyed
	/// </summary>
	/// <param name="inBuildUnit">In build unit.</param>
	public void CharDestroy(BuildUnit inBuildUnit) {
		counters [(int)inBuildUnit]--;
	} 


	/// <summary>
	/// Wait for building location before proceeding
	/// </summary>
	/// <param name="inBuildUnit">In build unit.</param>
	IEnumerator BuildStart( BuildUnit inBuildUnit ) {
		zone.Activate();
		selector.target = null;
		while (selector.target == null || selector.target.tag != "Pos") {
			yield return null;
		}
		BuildConfirm (inBuildUnit, selector.target);
	}

	/// <summary>
	/// Finishes Building at location
	/// </summary>
	/// <param name="inBuildUnit">In build unit.</param>
	/// <param name="inTarget">In target.</param>
	public void BuildConfirm ( BuildUnit inBuildUnit, Transform inTarget ) {
		Zone tempZone = inTarget.gameObject.GetComponentInParent<Zone> ();
		if (zone == tempZone && inTarget.childCount == 0) {
			zone.Deactivate ();
			gold -= costs [(int)inBuildUnit];
			counters [(int)inBuildUnit]++;

			int poolNum = CheckBuildPool (inBuildUnit);
			if (poolNum >= 0) {
				if (!buildStores [(int)inBuildUnit, poolNum]) {
					GameObject newBuilding = Instantiate (buildPrefabs [(int)inBuildUnit], inTarget.position, Quaternion.identity);
					buildStores [(int)inBuildUnit, poolNum] = newBuilding.GetComponent<Unit> ();
				}
				buildStores [(int)inBuildUnit, poolNum].Reset (zone);
				SetTarget (buildStores [(int)inBuildUnit, poolNum].transform, inTarget);

			}
		}
	}

	/// <summary>
	/// Find available slot in building pool.
	/// </summary>
	/// <returns>The build pool slot.</returns>
	/// <param name="inBuildUnit">In build unit.</param>
	int CheckBuildPool(BuildUnit inBuildUnit) {
		for (int i = 0; i < buildStores.GetLength (1); i++) {
			if (!buildStores [(int)inBuildUnit, i] || !buildStores [(int)inBuildUnit, i].isActive) {
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Minus building counter when destroyed.
	/// </summary>
	/// <param name="inBuildUnit">In build unit.</param>
	public void BuildDestroy(BuildUnit inBuildUnit) {
		counters [(int)inBuildUnit]--;
	} 

	/// <summary>
	/// Move unit to target location.
	/// </summary>
	/// <param name="inUnit">In unit.</param>
	/// <param name="inTarget">In target.</param>
	void SetTarget (Transform inUnit, Transform inTarget) {
		inUnit.parent = inTarget;
		inUnit.localPosition = Vector3.zero;
	}

	/// <summary>
	/// Gets the build count.
	/// </summary>
	/// <returns>The build count.</returns>
	/// <param name="inBuildUnit">In build unit.</param>
	int GetBuildCount(BuildUnit inBuildUnit) {
		int output = 0;
		for (int i = 0; i < buildStores.GetLength (1); i++) {
			if (buildStores [(int)inBuildUnit, i] && buildStores [(int)inBuildUnit, i].isActive) {
				output++;
			}
		}
		return output;
	}
}
