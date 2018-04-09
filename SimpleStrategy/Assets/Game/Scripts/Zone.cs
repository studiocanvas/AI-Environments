using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

	public enum Clan {
		Red,
		Blue,
		Yellow,
		Green
	}
	public Clan clan;

	public Material[] unitMats;
	public Material[] zoneMats;

	public BoxCollider startPlace;
	public BoxCollider[] places; 

	MeshRenderer meshRenderer;

	// Use this for initialization
	void Awake () {
		places = GetComponentsInChildren<BoxCollider> ();
		meshRenderer = GetComponent<MeshRenderer> ();
		meshRenderer.material = zoneMats [(int)clan];
		Deactivate ();
	}
	/// <summary>
	/// Activate the grid for deploying building.
	/// </summary>
	public void Activate() {
		foreach (BoxCollider place in places) {
			place.enabled = true;
		}
	}
	/// <summary>
	/// Deactivate the grid after building has been deployed.
	/// </summary>
	public void Deactivate() {
		foreach (BoxCollider place in places) {
			place.enabled = false;
		}
	}

	/// <summary>
	/// Reset the zone by clearing all the child objects.
	/// </summary>
	public void Reset() {
		foreach (BoxCollider place in places) {
			if (place.transform.childCount > 0) {
				place.transform.GetChild(0).parent = null;
			}
		}
	}
}
