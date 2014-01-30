using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVerices;

	void Start () {
		initialVertices = new List<Quaternion>();
		currentVerices = new List<Quaternion>();

		Quaternion quaternion1 = new Quaternion (2, 1, 3, 0);
		Quaternion quaternion2 = new Quaternion (-1.667f, 0.333f, 0.333f, 0.0f);

		Quaternion quaternion3 = new Quaternion (1, 2, 3, 0);
		Quaternion quaternion4 = new Quaternion (-1, 1, -1, 0);

		currentVerices.Add (quaternion1);
		currentVerices.Add (quaternion2);

		initialVertices.Add (quaternion3);
		initialVertices.Add (quaternion4);

		Quaternion nDotProduct = new Quaternion ();


	}

	void Update () {

	}

	private void NSP()
	{

	}
}
