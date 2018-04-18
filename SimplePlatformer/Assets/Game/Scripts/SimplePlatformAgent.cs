using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlatformAgent : Agent {

	private float timer = 0;
	private float speed = 25f;
	private float jumpStrength = 4f;
	private int maxJump = 2;
	private float maxMove = 0.2f;
	private int jumpCount = 0;
	private Rigidbody rb;
	public GameManager gameManager;
	public GameObject[] inventory = new GameObject[5];
	RayPerception rayPer;

	public override void InitializeAgent()
	{
		//gameManager = GetComponentInParent<GameManager>();
		rb = GetComponent<Rigidbody>();
		rayPer = GetComponent<RayPerception>();
		Reset ();
	}

    public override void CollectObservations()
    {
		float rayDistance = 12f;
		float[] rayAngles = { 20f, 60f, 90f, 120f, 160f };
		string[] detectableObjects = { "Key", "Goal", "Ground" };
		//AddVectorObs((float)GetStepCount() / (float)agentParameters.maxStep);
		//AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
		AddVectorObs(gameObject.transform.localPosition);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
	{
		float moveHorizontal = Mathf.Clamp(vectorAction[0], -1f, 1f);
		float moveVertical = 0;
		if (vectorAction[1] > 0.5f && jumpCount < maxJump) {
			moveVertical = jumpStrength;
			jumpCount++;
		}
		Vector3 movement = new Vector3 (Mathf.Clamp(moveHorizontal,-maxMove, maxMove), moveVertical, 0.0f);

		rb.AddForce (movement * speed);


    }

    public override void AgentReset()
    {
		Reset ();
    }

    public override void AgentOnDone()
    {
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
				AddReward(0.5f);
			}
			break;
		case "Goal":
			int inventorySlot2 = GetKeySlot ();
			if (inventorySlot2 >= 0) {
				Debug.Log ("Goal");
				Reset ();
				SetReward(1f);
				Done();
			}
			break;
		}
	}
	void Update() {
		if (transform.localPosition.y < gameManager.killzone) {
			Died ();
			SetReward(-1f);
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
