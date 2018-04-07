using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {

	public Image healthBar;

	public bool isBillboard;

	void Awake() {
		if (isBillboard) {
			StartCoroutine (BillboardUpdate());
		}
	}

	IEnumerator BillboardUpdate () {
		while (true) {
			healthBar.transform.rotation = Quaternion.identity;
			yield return null;
		}
	}
}
