using UnityEngine;
using System.Collections;

public class Plane1 : MonoBehaviour {
	public float rotationSpeed;
	public MainMenu menu;

	private Vector3 currentAngles;

	void Start () {
		currentAngles = Vector3.zero;
	}

	void Update () {
		currentAngles.x = menu.sliderValues [2];
		currentAngles.y = menu.sliderValues [1];
		currentAngles.z = menu.sliderValues [0];

		transform.eulerAngles = -currentAngles;
	}
}
