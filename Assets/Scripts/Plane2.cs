using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVertices;

	void OnGUI() {
		if (GUI.Button (new Rect (100, 350, 50, 30), "Старт")) {			
			bool enableAlgorithm = false;

			getVerticesContour(plane1);

			if (enableAlgorithm) {
				// Starting algorithm	
				initialVertices = new List<Quaternion>();
				currentVertices = new List<Quaternion>();

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

				// Находим вращающий кватернион b
				float halfAngle = Mathf.Acos(mainCos) / 2.0f;
				float halfCos = Mathf.Cos(halfAngle);
				float halfSin = Mathf.Sin(halfAngle);

				Quaternion rotationQuat = new Quaternion(midNormal.x * halfSin, 
				                                         midNormal.y * halfSin, 
				                                         midNormal.z * halfSin, 
				                                         halfCos);
				Quaternion rotationQuatInvert = new Quaternion(-rotationQuat.x,
				                                               -rotationQuat.y,
				                                               -rotationQuat.z,
				                                               rotationQuat.w);

				
				List<Quaternion> newVertices = new List<Quaternion>();
				for (int i = 0; i < currentVertices.Count; ++i) {
					Quaternion currVec = currentVertices[i];
					Quaternion tempVec = getQuaternionsVectorProduct(rotationQuat, currVec);
					Quaternion newVec = getQuaternionsVectorProduct(tempVec, rotationQuatInvert);
					newVertices.Add(newVec);
				}
				currentVertices = newVertices;

				// TODO: Построить самолет по новым координатам из currentVertices
			}
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

	private List<Quaternion> getVerticesContour(MonoBehaviour gameObject) {
		List<Quaternion> contour = new List<Quaternion> ();		
		MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
		Mesh viewedModel = viewedModelFilter.mesh;
		Vector3[] vertices = viewedModel.vertices;

		for (int i = 0; i < vertices.Length; ++i) {
			Debug.Log("X = " + vertices[i].x);
			Debug.Log("Y = " + vertices[i].y);
			Debug.Log("Z = " + vertices[i].z);
		}
		return contour;
	}
}
