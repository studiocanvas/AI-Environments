using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

	public float maxHealth = 1f; 
	float minHealth = 0f;
	public float health = 1f;
	public float recoveryRate = 0.025f;
	public bool isActive = true;
	string prevHit;
	UnitUI UI;
	MeshRenderer[] meshRenderers;

	public Zone zone;

	void Awake() {
		UI = GetComponentInChildren<UnitUI> (true);
		meshRenderers = transform.Find ("Model").gameObject.GetComponentsInChildren<MeshRenderer> (true);
	}

	/// <summary>
	/// Sets the material for all units assigns to this zone.
	/// </summary>
	/// <param name="inClan">In clan.</param>
	void SetMaterial( Zone.Clan inClan ) {
		foreach (MeshRenderer meshRenderer in meshRenderers) {
			meshRenderer.material = zone.unitMats [(int)inClan];
		}
	}

	/// <summary>
	/// Hide this instance when resetting. This will automatically be added to the object pool.
	/// </summary>
	public void Hide() {
		gameObject.SetActive (false);
		isActive = false;
	}

	/// <summary>
	/// Reset ths unit.
	/// </summary>
	/// <param name="inZone">In zone.</param>
	public void Reset( Zone inZone ) {
		zone = inZone;
		SetMaterial (zone.clan);
		health = maxHealth;
		UpdateHealthUI ();
		StopAllCoroutines ();
		gameObject.SetActive (true);
		isActive = true;
	}

	/// <summary>
	/// Damage applied to this unit.
	/// </summary>
	/// <param name="inDamage">In damage.</param>
	public void Damage( float inDamage ) {
		health -= inDamage;
		health = Mathf.Clamp (health, minHealth, maxHealth);
		UpdateHealthUI ();

		if (health <= minHealth) {
			Died ();
			gameObject.SetActive (false);
			isActive = false;
		} else {
			StopAllCoroutines ();
			StartCoroutine (Recover ());
			isActive = true;
		}
	}
	/// <summary>
	/// Auto Recovery for unit.
	/// </summary>
	IEnumerator Recover() {
		yield return new WaitForSeconds (2f * GameManager.globalSpeed);
		//Debug.Log ("Recover");
		while (health < maxHealth) {
			health += recoveryRate;
			UpdateHealthUI ();
			yield return new WaitForSeconds (1f * GameManager.globalSpeed);
		}
		health = Mathf.Clamp (health, minHealth, maxHealth);
	}

	/// <summary>
	/// Updates the healthbar.
	/// </summary>
	void UpdateHealthUI() {
		UI.healthBar.fillAmount = health/maxHealth;
	}

	public virtual void Died() {
	}
}
