using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Transform spawnPoint;
	public Transform keyPoint;
	public BoxCollider goal;
	public SphereCollider key;
	public float killzone = -10f;

	// Use this for initialization
	void Awake () {
//		spawnPoint = transform.Find ("SpawnPoint");
//		keyPoint = transform.Find ("KeyPoint");
//		goal = transform.Find ("Goal").GetComponent<BoxCollider> ();
//		key = transform.Find ("Key").GetComponent<SphereCollider> ();
		Reset ();
	}
	
	// Update is called once per frame
	public void Reset () {
		key.gameObject.SetActive (true);
		key.transform.position = keyPoint.position;
	}
}
