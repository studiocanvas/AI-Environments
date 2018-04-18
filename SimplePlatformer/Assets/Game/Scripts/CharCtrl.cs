using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCtrl : MonoBehaviour {

	private float speed = 25f;
	private float jumpStrength = 8f;
	private int maxJump = 2;
	private float maxMove = 0.2f;
	private int jumpCount = 0;
	private Rigidbody rb;
	private GameManager gameManager;
	public GameObject[] inventory = new GameObject[5];

	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody>();

		Reset ();
	}


	void Reset() {
		rb.velocity = new Vector3(0f,0f,0f);
		rb.angularVelocity = new Vector3(0f,0f,0f);
		rb.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
		transform.position = gameManager.spawnPoint.position;
		DropKey ();
		gameManager.Reset ();
	}

	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = 0;
		if (Input.GetKeyUp(KeyCode.Space) && jumpCount < maxJump) {
			moveVertical = jumpStrength;
			jumpCount++;
		}
		Vector3 movement = new Vector3 (Mathf.Clamp(moveHorizontal,-maxMove, maxMove), moveVertical, 0.0f);

		rb.AddForce (movement * speed);

		if (transform.position.y < gameManager.killzone) {
			Died ();
		}
	}

	void Died() {
		Reset ();
	}

	void OnCollisionEnter (Collision inCollision) {
		if (inCollision.gameObject.tag == "Ground") {
			jumpCount = 0;
		}
	}

	void OnTriggerEnter (Collider inCollider) {
		switch (inCollider.gameObject.tag) {
		case "Key":
			int inventorySlot1 = FindEmptyInventorySlot ();
			if (inventorySlot1 >= 0) {
				inventory [inventorySlot1] = inCollider.gameObject;
				inCollider.gameObject.SetActive (false);
			}
			break;
		case "Goal":
			int inventorySlot2 = GetKeySlot ();
			if (inventorySlot2 >= 0) {
				Debug.Log ("Goal");
				Reset ();
			}
			break;
		}
	}

	int FindEmptyInventorySlot() {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory [i] == null) {
				return i;
			}
		}
		return -1;
	}

	int GetKeySlot() {
		for (int i = 0; i < inventory.Length; i++) {
			if (inventory[i] && inventory[i] == gameManager.key.gameObject) {
				return i;
			}
		}
		return -1;
	}

	void DropKey() {
		int inventorySlot = GetKeySlot ();
		if (inventorySlot >= 0) {
			inventory [inventorySlot] = null;
		}
	}
}
