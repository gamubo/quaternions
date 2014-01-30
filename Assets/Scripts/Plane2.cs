using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVerices;

	void Start () {
		Quaternion quaternion1 = new Quaternion (2, 1, 3);
		Quaternion quaternion2 = new Quaternion (-1.667, 0.333, 0.333, 0);

		Quaternion quaternion3 = new Quaternion (1, 2, 3);
		Quaternion quaternion4 = new Quaternion (-1, 1, -1);

		currentVerices.Add (quaternion1);
		currentVerices.Add (quaternion2);

		initialVertices.Add (quaternion3);
		initialVertices.Add (quaternion4);

		Quaternion result = new Quaternion ();


	}

	void Update () {

	}

	private void NSP();
}
