using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVertices;

	void OnGUI() {
		if (GUI.Button (new Rect (100, 350, 50, 30), "Старт")) {
			// Starting algorithm	
			initialVertices = new List<Quaternion>();
			currentVertices = new List<Quaternion>();
			
			Quaternion quaternion1 = new Quaternion (2, 1, 3, 0);
			Quaternion quaternion2 = new Quaternion (-1.667f, 0.333f, 0.333f, 0.0f);
			
			Quaternion quaternion3 = new Quaternion (1, 2, 3, 0);
			Quaternion quaternion4 = new Quaternion (-1, 1, -1, 0);
			
			currentVertices.Add (quaternion1);
			currentVertices.Add (quaternion2);
			
			initialVertices.Add (quaternion3);
			initialVertices.Add (quaternion4);


			// Algorithm
			if (currentVertices.Count != initialVertices.Count)
				throw new System.Exception("Contours must have the same size!");
			
			float lenProduct = getContourLength(currentVertices) * getContourLength(initialVertices);

			float mainCos = 0;
			Quaternion hypc = new Quaternion();
			for (int i = 0; i < currentVertices.Count; ++i) {
				Quaternion vec1 = currentVertices[i];
				Quaternion vec2 = initialVertices[i];
				
				float vec1Length = getVectorLength(vec1);
				float vec2Length = getVectorLength(vec2);

				float dotProduct = getVectorsDotProduct(vec1, vec2);

				float currCos = dotProduct / (vec1Length * vec2Length);
				float currSin = Mathf.Sin(Mathf.Acos(currCos));

				float factor = currSin * vec1Length * vec2Length;
				Quaternion normal = getNormal(vec1, vec2);

				hypc.x += -normal.x * factor;
				hypc.y += -normal.y * factor;
				hypc.z += -normal.z * factor;
				mainCos += dotProduct;
			}
			hypc.x /= lenProduct; // Правая часть НСП
			hypc.y /= lenProduct;
			hypc.z /= lenProduct; 

			mainCos /= lenProduct; // Косинус (левая часть НСП)

			Debug.Log("Main cos = " + mainCos);
			Debug.Log("Vec.X = " + hypc.x);
			Debug.Log("Vec.Y = " + hypc.y);
			Debug.Log("Vec.Z = " + hypc.z);

			//float halfAngle = Mathf.Acos(mainCos) / 2.0f;
			//float halfCos = Mathf.Cos(halfAngle);
			//float halfSin = Mathf.Sin(halfAngle);

			//Quaternion directionalVec = hypc; // Не знаю, как найти усредненную нормаль из гиперкомплексной части (второй лист Фурмана, а40)

			//Quaternion rotationQuat = new Quaternion(hypc.x * halfSin, hypc.y * halfSin, hypc.z * halfSin, halfCos);
			//Quaternion rotationQuatInvert = new Quaternion(-hypc.x * halfSin, -hypc.y * halfSin, -hypc.z * halfSin, halfCos);
		}
	}

	void Start () {
	}

	void Update () {
	}

	private float getContourLength(List<Quaternion> contour) {
		float length = 0;
		for (int i = 0; i < contour.Count; ++i)
			length += getVectorsDotProduct(contour[i], contour[i]);
		return Mathf.Sqrt(length);
	}

	private float getVectorsDotProduct(Quaternion vec1, Quaternion vec2) {
		return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
	}

	private float getVectorLength(Quaternion vec) {
		return Mathf.Sqrt(getVectorsDotProduct(vec, vec));
	}

	private Quaternion getNormal(Quaternion vec1, Quaternion vec2) {	
		Quaternion normal = new Quaternion();		

		normal.x = ((vec1.y * vec2.z) - (vec1.z * vec2.y));
		normal.y = ((vec1.z * vec2.x) - (vec1.x * vec2.z));
		normal.z = ((vec1.x * vec2.y) - (vec1.y * vec2.x));

		float len = getVectorLength(normal);

		normal.x /= len;
		normal.y /= len;
		normal.z /= len;

		return normal;  // Возвращаем результат (направление, куда направлен полигон - нормаль)
	}
}
