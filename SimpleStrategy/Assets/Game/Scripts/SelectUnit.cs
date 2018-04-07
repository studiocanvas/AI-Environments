using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectUnit : MonoBehaviour {
	Selector selector;
	RaycastHit hit;
	bool OnScreen = false;
	public Vector2 ScreenPos;

	void Awake () {
		selector = GameObject.Find("Selector").GetComponent<Selector>();
	}

	void Update () {
		ScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		if(selector.UnitWithinScreenSpace(ScreenPos))
		{
			OnScreen = true;
			if (!selector.UnitsOnScreenSpace.Contains (this.gameObject)) {
				selector.UnitsOnScreenSpace.Add (this.gameObject);
			}
		} else {
			if(OnScreen)
			{
				selector.UnitsOnScreenSpace.Remove(this.gameObject);
				OnScreen = false;
			}
		}

		if (selector.selectedunit == this.gameObject || selector.selectedunits.Contains(this.gameObject))
		{
			if (Input.GetMouseButtonDown (0)) 
			{
				Select ();
			}
			if (Input.GetMouseButtonDown(1))
			{
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				{
					Confirm (hit);
				}
				Deselect ();
			}
		}
	}
	public virtual void Deselect() {
		
	}
	public virtual void Select () {
		
	}
	public virtual void Confirm( RaycastHit inHit ) {
		
	}
}


