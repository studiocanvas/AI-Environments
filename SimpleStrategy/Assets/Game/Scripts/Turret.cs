using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

	Weapon weapon;
	Unit unit;
	BuildCtrls[] buildCtrls;
	float attackRange = 3f;
	float blastRange = 0.5f;
	float shootSpeed = 9f;
	float hitStrength = 0.1f;
	bool isShooting = false;

	// Use this for initialization
	void Start () {
		weapon = GetComponentInChildren<Weapon> ();
		unit = GetComponent<Unit> ();
		buildCtrls = FindObjectsOfType<BuildCtrls> ();
	}
	void Update() {
		foreach (BuildCtrls buildCtrl in buildCtrls) {
			foreach (Unit charStore in buildCtrl.charStores) {
				if (CheckShootTarget(charStore)) {
					Shoot(charStore);
				}
			}
			for (int c = 0; c < buildCtrl.buildStores.GetLength (0); c++) {
				for (int i = 0; i < buildCtrl.buildStores.GetLength (1); i++) {
					Unit buildStore = buildCtrl.buildStores [c, i];
					if (CheckShootTarget(buildStore)) {
						Shoot(buildStore);
					}
				}
			}
		}
	}
	/// <summary>
	/// Checks the shoot target for distance, zone and if active.
	/// </summary>
	/// <returns><c>true</c>, if shoot target was checked, <c>false</c> otherwise.</returns>
	/// <param name="inUnit">In unit.</param>
	bool CheckShootTarget( Unit inUnit ) {
		return inUnit 
			&& inUnit.zone != unit.zone && inUnit.isActive
			&& Vector3.Distance (transform.position, inUnit.transform.position) < attackRange;
	}
	/// <summary>
	/// Forces a shot.
	/// </summary>
	/// <param name="inTarget">In target.</param>
	void ForceShoot( Transform inTarget ) {
		Unit newUnit = inTarget.gameObject.GetComponent<Unit> ();
		if (CheckShootTarget(newUnit)) {
			isShooting = false;
			Shoot (newUnit);
		}
	}
	/// <summary>
	/// Shoot the specified inTarget.
	/// </summary>
	/// <param name="inTarget">In target.</param>
	void Shoot( Unit inTarget ) {
		if (!isShooting) {
			StartCoroutine(ShootUpdate(inTarget));
		}
	}
	IEnumerator ShootUpdate( Unit inTarget ) {
		isShooting = true;
		weapon.transform.localPosition = Vector3.zero;
		Vector3 relativePos = inTarget.transform.position - weapon.transform.position;
		Quaternion rotation = Quaternion.LookRotation (relativePos);
		Vector3 target = inTarget.transform.position;
		weapon.transform.rotation = rotation;
		while (weapon.transform.position != target) {
			float step = shootSpeed * Time.deltaTime * (1/GameManager.globalSpeed);
			weapon.transform.position = Vector3.MoveTowards (weapon.transform.position, target, step);
			yield return null;
		}
		if (Vector3.Distance (weapon.transform.position, inTarget.transform.position) < blastRange) {
			inTarget.Damage (hitStrength);
		}
		weapon.transform.localPosition = Vector3.zero;
		yield return new WaitForSeconds (1f * GameManager.globalSpeed);
		isShooting = false;
		Debug.Log ("endshoot");
	}
//	// Update is called once per frame
//	void OnTriggerStay( Collider inCollider ) {
//		if (inCollider.gameObject != gameObject) {
//			Unit newUnit = inCollider.gameObject.GetComponent<Unit> ();
//			if (newUnit && (newUnit.zone != unit.zone || newUnit.zone == null)) {
//				Debug.Log ("Intruder");
//			}
//		}
//	}
}
