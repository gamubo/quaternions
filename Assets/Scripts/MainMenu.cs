using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	/*
	 * 1 - Крен
	 * 2 - Рыскание
	 * 3 - Тангаж
    */
	public float[] sliderValues;

	private float minValue = -45.0f;
	private float maxValue = 45.0f;

	void Start ()
	{
		sliderValues = new float[3];
	}

	void OnGUI ()
	{
		float left = 25.0f;
		float top = 75.0f;
		float width = 200.0f;
		float height = 30.0f;

		GUI.Box(new Rect(10, 10, left + width, 320), "Углы самолета");

		float textOffset = 25.0f;
		float spaceBetweenSliders = 100.0f;

		for (int i = 0; i < 3; i++) {
			float position = top + i * spaceBetweenSliders;
			Rect sliderRectangle = new Rect(left, position, width, height);

			string sliderText = "";
			float sliderValue = 0.0f;

			if (i == 0) {
				sliderText = "Крен:";
			} else if (i == 1) {
				sliderText = "Рыскание:";
			} else if (i == 2) {
				sliderText = "Тангаж:";
			}

			sliderValues[i] = GUI.HorizontalSlider (sliderRectangle, sliderValues[i], minValue, maxValue);

			GUI.Label (new Rect (left + textOffset, position - textOffset, width, height), sliderText);
			GUI.Label (new Rect (left + width - (textOffset * 2), position - textOffset, width, height), sliderValues[i].ToString ());
		}
	}
}
