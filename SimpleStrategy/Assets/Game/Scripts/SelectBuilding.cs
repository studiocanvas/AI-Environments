using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectBuilding : SelectUnit {
	public BuildCtrls buildCtrls;
	public BuildCtrls.BuildUnit buildUnit;
	public GameObject marker;

	void Start () {
		buildCtrls = FindObjectOfType <BuildCtrls> ();
		marker = transform.Find("Marker").gameObject;
	}

	public override void Deselect() {
		marker.SetActive (false);
	}
	public override void Select () {
		marker.SetActive (true);
	}
	public override void Confirm( RaycastHit inHit ) {
		if(inHit.transform.tag == "Pos") {
			buildCtrls.BuildConfirm (buildUnit, inHit.transform);
		}
		if(inHit.transform.tag == "Building" || inHit.transform.tag == "SelectableUnit")
		{
			gameObject.SendMessage ("ForceShoot", inHit.transform, SendMessageOptions.DontRequireReceiver);
		}
	}
}


