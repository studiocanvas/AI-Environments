using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharBot : MonoBehaviour {
	NavMeshAgent agent;
	Unit unit;
	float attackRange = 1.5f;
	float detectRange = 2f;
	float stopRange = 1f;
	float hitStrength = 0.03f;
	float walkSpeed = 3.5f;
	Animator anim;
	Transform prevAttackTarget;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
		unit = GetComponent<Unit> ();
		agent = this.gameObject.GetComponent<NavMeshAgent>();
		agent.stoppingDistance = stopRange;
		agent.speed = walkSpeed * (1/GameManager.globalSpeed);
	}

	void OnDisable() {
		StopAllCoroutines ();
	}

	// Update is called once per frame
	public void Attack ( Transform inTarget ) {
		if (prevAttackTarget != inTarget) {
			prevAttackTarget = inTarget;
			// Check if within attack range
			StopAllCoroutines ();
			StartCoroutine (AttackUpdate (inTarget));
		}
	}
	IEnumerator AttackUpdate( Transform inTarget ) {
		Unit target = inTarget.gameObject.GetComponent<Unit> ();
		if (target && target.zone != unit.zone) {
			agent.destination = inTarget.position;
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isIdle", true);
			while (Vector3.Distance (transform.position, inTarget.position) > attackRange) {
				yield return null;
			}
			//Debug.Log ("Attack");
			anim.SetBool ("isAttacking", true);
			anim.SetBool ("isIdle", false);
			while (target.isActive) {
				//Debug.Log ("Hit");
				yield return new WaitForSeconds(1f * GameManager.globalSpeed);
				target.Damage (unit, hitStrength);
			}
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isIdle", true);
			StartCoroutine (DetectUpdate ());
		}
	}

	/// <summary>
	/// Walk to the specified position.
	/// </summary>
	/// <param name="inPos">In position.</param>
	public void Walk ( Vector3 inPos ) {
		//Debug.Log ("Walk");
		if (unit.isActive) {
			StopAllCoroutines ();
			agent.enabled = false;
			agent.enabled = true;
			agent.ResetPath ();
			agent.destination = inPos;
			anim.SetBool ("isAttacking", false);
			anim.SetBool ("isIdle", true);
			StartCoroutine (DetectUpdate ());
		}
	}
	/// <summary>
	/// Detects the nearby enemy update.
	/// </summary>
	/// <returns>The update.</returns>
	IEnumerator DetectUpdate() {
		while (true) {
			yield return null;
			Unit target = DetectNearbyEnemy ();
			if (target) {
				Attack (target.transform);
			}
		}
	}
	/// <summary>
	/// Detects the nearby enemy.
	/// </summary>
	Unit DetectNearbyEnemy () {
		Collider[] enemies = Physics.OverlapSphere(transform.position, detectRange);
		foreach (Collider enemy in enemies) {
			Unit target = enemy.gameObject.GetComponent<Unit> ();
			if (target && target.zone != unit.zone && target.isActive) {
				return target;
				break;
			}
		}
		return null;
	}
	/// <summary>
	/// Attacks the nearby enemy.
	/// </summary>
	void AttackNearbyEnemy () {
		Unit target = DetectNearbyEnemy ();
		if (target) {
			Attack (target.transform);
		}
	}
}
