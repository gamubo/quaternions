using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVertices;

	private float zOffset = -2;

	void OnGUI() {
		if (GUI.Button (new Rect (100, 350, 50, 30), "Старт")) {
			initialVertices = getVerticesContour(plane1, true);	
			currentVertices = getVerticesContour(this, false);

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

				hypc.x += normal.x * factor;
				hypc.y += normal.y * factor;
				hypc.z += normal.z * factor;
				mainCos += dotProduct;
			}
			// Правая часть НСП
			hypc.x /= lenProduct; 
			hypc.y /= lenProduct;
			hypc.z /= lenProduct; 

			// Косинус (левая часть НСП)
			mainCos /= lenProduct; 

			// Находим усредненную нормаль
			float hypcLen = getVectorLength(hypc);
			Quaternion midNormal = hypc; 
			midNormal.x /= hypcLen;
			midNormal.y /= hypcLen;
			midNormal.z /= hypcLen;
							
			Vector3 rotationVector = new Vector3(-midNormal.x, -midNormal.y, -midNormal.z);
			float rotationAngle = -(Mathf.Acos(mainCos) * 180.0f / Mathf.PI);
			this.transform.Rotate(rotationVector, rotationAngle);
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
	
	private Quaternion getQuaternionsVectorProduct(Quaternion quat1, Quaternion quat2) {	
		Quaternion result = new Quaternion();	

		result.x = quat1.w * quat2.x + quat1.x * quat2.w + quat1.y * quat2.z - quat1.z * quat2.y;
		result.y = quat1.w * quat2.y - quat1.x * quat2.z + quat1.y * quat2.w + quat1.z * quat2.x;
		result.z = quat1.w * quat2.z + quat1.x * quat2.y - quat1.y * quat2.x + quat1.z * quat2.w;
		result.w = quat1.w * quat2.w - quat1.x * quat2.x - quat1.y * quat2.y - quat1.z * quat2.z;

		return result;
	}
	
	
	List<Quaternion> getVerticesContour(MonoBehaviour gameObject, bool fixOffset) {
		List<Quaternion> contour = new List<Quaternion> ();		
		MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
		Mesh viewedModel = viewedModelFilter.mesh;
		Vector3[] vertices = viewedModel.vertices;		
		
		for (int i = 0; i < vertices.Length; ++i) {
			Vector3 vertex = gameObject.transform.TransformPoint(vertices[i]);
			float zValue = (fixOffset) ? vertex.z + zOffset : vertex.z;
			contour.Add (new Quaternion(vertex.x, vertex.y, zValue , 0.0f));
		}
		return contour;
	}

}
