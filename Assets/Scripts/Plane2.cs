using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plane2 : MonoBehaviour {
	public Plane1 plane1;
	public GameObject plane2Point;
	
	private List<Quaternion> initialVertices;
	private List<Quaternion> currentVertices;

	// Отступ текущей модели от наблюдаемой
	private Vector3 correctionVect/* = new Vector3(0, 0, -2.0f)*/;

	// Последнее полученное значение угла между моделями (для проверки возможности дальшейшего уменьшения угла)
	private float lastAngle = 0;
	// Минимальное значение угла между моделями, при его достижении, угловое согласование прекращается
	private float minAngle = 0.5f;
	// Флаг, показывающий, нужно ли делать еще поворот
	private bool needToMakeAnotherIteration;

	void OnGUI() {
		if (GUI.Button (new Rect (10, 340, 100, 35), "Согласовать")) {
			needToMakeAnotherIteration = true;
			while (needToMakeAnotherIteration) {
				doAngularCoordination();
			}
		} else if (GUI.Button (new Rect (120, 340, 115, 35), "Одна итерация")) {
			doAngularCoordination();
		}
	}

	private void movePlaneBehindTarget() {
		transform.position = plane2Point.transform.position;

		updateCorrectionVect();
	}

	private void updateCorrectionVect() {
		correctionVect = new Vector3 (transform.position.x - plane1.transform.position.x, 
		                              transform.position.y - plane1.transform.position.y, 
		                              transform.position.z - plane1.transform.position.z);
	}

	private void doAngularCoordination() {
		movePlaneBehindTarget();

		// Считываем вершины мешей в массив, как кватернионы 
		// (при этом корректируем координаты модели-образца в соответствии с отступом от текущей модели)
		initialVertices = getVerticesContour(plane1, correctionVect);	
		currentVertices = getVerticesContour(this);
		
		if (currentVertices.Count != initialVertices.Count)
			throw new System.Exception("Contours must have the same size!");
		
		// Алгоритм углового согласования
		float lenProduct = getContourLength(currentVertices) * getContourLength(initialVertices);
		
		float mainCos = 0;
		Quaternion hypc = new Quaternion();
		for (int i = 0; i < currentVertices.Count; ++i) {
			// i-е кватернионы контуров
			Quaternion vec1 = currentVertices[i];
			Quaternion vec2 = initialVertices[i];
			
			float vec1Length = getVectorLength(vec1);
			float vec2Length = getVectorLength(vec2);

			// Скалярное произведение i-ых кватернионов
			float dotProduct = getVectorsDotProduct(vec1, vec2);

			// Косинус и синус угла между i-ми кватернионами (находим из скалярного произведения)
			float currCos = dotProduct / (vec1Length * vec2Length);
			float currSin = Mathf.Sin(Mathf.Acos(currCos));

			// Парциальный нормирующий множитель e = |dw(n)| * |dl(n)| 
			// (без деления на произведение длин контуров - это делается после цикла)
			float factor = vec1Length * vec2Length;
			Quaternion normal = getNormal(vec1, vec2);

			// Правая часть НСП - r(n) * e(n) * sin(ф(n))
			hypc.x += normal.x * factor * currSin;
			hypc.y += normal.y * factor * currSin;
			hypc.z += normal.z * factor * currSin;

			// Левая часть НСП - скалярное произведение i-тых кватернионов
			// (делим на произведение длин контуров после цикла)
			mainCos += dotProduct;
		}
		// Делим левую (косинус угла между моделями) и правую (гиперкомплексную) части НСП на произведение длин контуров
		mainCos /= lenProduct;
		hypc.x /= lenProduct; 
		hypc.y /= lenProduct;
		hypc.z /= lenProduct; 
		
		// Угол между моделями (угол поворота)
		float rotationAngle = -(Mathf.Acos(mainCos) * 180.0f / Mathf.PI);
		Debug.Log ("angle = " + rotationAngle);

		// Если угол больше минимального и он не равен старому значению - делаем поворот
		if (rotationAngle != lastAngle && Mathf.Abs (rotationAngle) > minAngle) {
			lastAngle = rotationAngle;

			// Находим усредненную нормаль (ось поворота)
			float hypcLen = getVectorLength (hypc);
			Quaternion midNormal = hypc; 
			midNormal.x /= hypcLen;
			midNormal.y /= hypcLen;
			midNormal.z /= hypcLen;

			// Поворот модели вокруг усредненной нормали на заданный угол
			Vector3 rotationVector = new Vector3 (-midNormal.x, -midNormal.y, -midNormal.z);
			this.transform.Rotate (rotationVector, rotationAngle);
		} else {
			needToMakeAnotherIteration = false;
		}
	}

	// Норма контура
	private float getContourLength(List<Quaternion> contour) {
		float length = 0;
		for (int i = 0; i < contour.Count; ++i)
			length += getVectorsDotProduct(contour[i], contour[i]);
		return Mathf.Sqrt(length);
	}

	// Скалярное произведение векторов
	private float getVectorsDotProduct(Quaternion vec1, Quaternion vec2) {
		return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
	}

	// Норма вектора
	private float getVectorLength(Quaternion vec) {
		return Mathf.Sqrt(getVectorsDotProduct(vec, vec));
	}

	// Нахождение нормали плоскости, образованной двумя векторами
	private Quaternion getNormal(Quaternion vec1, Quaternion vec2) {	
		Quaternion normal = new Quaternion();

		normal.x = ((vec1.y * vec2.z) - (vec1.z * vec2.y));
		normal.y = ((vec1.z * vec2.x) - (vec1.x * vec2.z));
		normal.z = ((vec1.x * vec2.y) - (vec1.y * vec2.x));

		float len = getVectorLength(normal);

		normal.x /= len;
		normal.y /= len;
		normal.z /= len;

		return normal;
	}

	// Нахождение векторного произведения кватернионов
	private Quaternion getQuaternionsVectorProduct(Quaternion quat1, Quaternion quat2) {	
		Quaternion result = new Quaternion();

		result.x = quat1.w * quat2.x + quat1.x * quat2.w + quat1.y * quat2.z - quat1.z * quat2.y;
		result.y = quat1.w * quat2.y - quat1.x * quat2.z + quat1.y * quat2.w + quat1.z * quat2.x;
		result.z = quat1.w * quat2.z + quat1.x * quat2.y - quat1.y * quat2.x + quat1.z * quat2.w;
		result.w = quat1.w * quat2.w - quat1.x * quat2.x - quat1.y * quat2.y - quat1.z * quat2.z;

		return result;
	}

	// Считывание вершин MeshFilter-а в контур кватернионов с корректировкой значений (если требуется)
	List<Quaternion> getVerticesContour(MonoBehaviour gameObject, Vector3 correctionVect = new Vector3()) {
		MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
		Mesh viewedModel = viewedModelFilter.mesh;
		Vector3[] vertices = viewedModel.vertices;		
		
		List<Quaternion> contour = new List<Quaternion> ();	
		for (int i = 0; i < vertices.Length; ++i) {
			Vector3 vertex = gameObject.transform.TransformPoint(vertices[i]);
			Quaternion quaternion = new Quaternion(vertex.x + correctionVect.x,
			                                       vertex.y + correctionVect.y,
			                                       vertex.z + correctionVect.z,
			                                       0.0f);
			contour.Add(quaternion);
		}

		return contour;
	}

}
