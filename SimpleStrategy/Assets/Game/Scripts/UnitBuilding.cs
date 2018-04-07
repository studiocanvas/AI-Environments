using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuilding : Unit {


	public SelectBuilding selectBuilding;

	// Use this for initialization
	void Start () {
		selectBuilding = GetComponent<SelectBuilding> ();
	}
	public override void Died() {
		selectBuilding.buildCtrls.BuildDestroy (selectBuilding.buildUnit);
	}
}
