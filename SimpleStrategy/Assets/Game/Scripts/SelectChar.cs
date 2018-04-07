using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class SelectChar : SelectUnit {
	
	CharBot charBot;

	void Start () {
		
		charBot = GetComponent<CharBot>();
	}


	public override void Confirm( RaycastHit inHit ) {
		if(inHit.transform.tag == "Floor")
		{
			charBot.Walk (inHit.point);
		}
		if(inHit.transform.tag == "Building" || inHit.transform.tag == "SelectableUnit")
		{
			charBot.Attack (inHit.transform);
		}
	}
}


