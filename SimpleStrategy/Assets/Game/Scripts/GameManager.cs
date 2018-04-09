using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	/// <summary>
	/// The global speed.
	/// </summary>
	public static float globalSpeed = 0.0001f;


	// Recorders
	public Zone[] zones;
	public BuildCtrls[] buildCtrls;
	public float[] golds;
	public float[] loaders;
	public float[] places;
	public Vector3[] charPos;

	float startTime;
	float timeLimit = 5f;

	// Use this for initialization
	void Awake() {
		golds = new float[buildCtrls.Length];
		places = new float[zones[0].places.Length * zones.Length];
		loaders = new float[buildCtrls.Length * buildCtrls[0].uiBuildButtons.Length];
	}
	void Start() {
		ResetPlaces ();
	}

	/// <summary>
	/// Resets the game.
	/// </summary>
	public void ResetGame() {
		foreach (BuildCtrls buildCtrl in buildCtrls) {
			buildCtrl.StartGame ();
		}
		startTime = Time.time;
	}

	/// <summary>
	/// Resets the places.
	/// </summary>
	public void ResetPlaces () {
		for (int i = 0; i < places.Length; i++) {
			places [i] = -1f;
		}
	}

	/// <summary>
	/// Records the places.
	/// </summary>
	/// <param name="inZone">In zone.</param>
	/// <param name="inPlace">In place.</param>
	/// <param name="inBuildUnit">In build unit.</param>
	public void RecordPlaces( Zone inZone, int inPlace, BuildCtrls.BuildUnit inBuildUnit) {
		int zoneCount = (int)inZone.clan * inZone.places.Length;
		places [inPlace + zoneCount] = (float)inBuildUnit;
	}

	public void RemovePlaces( Zone inZone, Unit inUnit ) {
		int zoneCount = (int)inZone.clan * inZone.places.Length;
		if (inUnit.transform.parent != null) {
			int placeNum = int.Parse (inUnit.transform.parent.gameObject.name);
			places [placeNum + zoneCount] = -1f;
		}
	}

	void Update() {
		// Record Loader state
		int b = -1;
		for (int i = 0; i < loaders.Length; i++) {
			int j = i % buildCtrls [0].uiBuildButtons.Length;
			if (j == 0) {
				b++;
			}
			loaders [i] = buildCtrls[b].uiBuildButtons[j].loadValue;
		}

		// Record Gold state
		for (int g = 0; g < buildCtrls.Length; g++) {
			golds [g] = buildCtrls [g].gold;
		}

		CharBot[] charBots = FindObjectsOfType<CharBot> ();
		if (charBots.Length > 0) {
			charPos = new Vector3[charBots.Length];
			for (int c = 0; c < charBots.Length; c++) {
				charPos [c] = charBots [c].transform.position;
			}
		}
	}
}
