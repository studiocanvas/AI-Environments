using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {

	public Image loadBar;
	public float loadValue;
	public Button button;
	public bool isActive = false;

	public float loadRate = 0.0001f;

	void Awake() {
		loadBar = transform.Find("Image").gameObject.GetComponent<Image>();
		button = GetComponent<Button> ();
	}
	public void Init( float inValue ) {
		loadRate = inValue;
		StopLoad ();
	}

	void UpdateLoadBar (float inValue) {
		loadBar.fillAmount = inValue;
		button.interactable = (inValue >= 1f && isActive);
	}

	public void StartLoad() {
		StopCoroutine (Loader ());
		StartCoroutine (Loader ());
	}

	public void StopLoad() {
		StopCoroutine (Loader ());
		UpdateLoadBar (0f);
	}

	IEnumerator Loader() {
		if (isActive) {
			while (loadValue < 1f) {
				loadValue += loadRate;
				loadValue = Mathf.Clamp01 (loadValue);
				UpdateLoadBar (loadValue);
				yield return new WaitForSeconds (0.001f * GameManager.globalSpeed);
			}
		}
		UpdateLoadBar (loadValue);
	} 
}
