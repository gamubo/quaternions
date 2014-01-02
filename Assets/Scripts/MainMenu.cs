using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	private float sliderValue1 = 0.0f;
	private float sliderValue2 = 0.0f;

	private float minValue = 0.0f;
	private float maxValue = 10.0f;

	void OnGUI ()
	{
		float left = 25.0f;
		float top = 75.0f;
		float width = 125.0f;
		float height = 30.0f;

		GUI.Box(new Rect(10, 10, left + width, 500), "Оси самолета");

		float textOffset = 25.0f;
		float spaceBetweenSliders = 100.0f;

		for (int i = 0; i < 2; i++) {
			float position = top + i * spaceBetweenSliders;
			Rect sliderRectangle = new Rect(left, position, width, height);

			string sliderText = "";
			float sliderValue = 0.0f;

			if (i == 0) {
				sliderValue1 = GUI.HorizontalSlider (sliderRectangle, sliderValue1, minValue, maxValue);
				sliderText = "Первая ось:";
				sliderValue = sliderValue1;
			} else if (i == 1) {
				sliderValue2 = GUI.HorizontalSlider (sliderRectangle, sliderValue2, minValue, maxValue);
				sliderText = "Вторая ось:";
				sliderValue = sliderValue2;
			}
			GUI.Label (new Rect (left + textOffset, position - textOffset, width, height), sliderText);
			GUI.Label (new Rect (left + width - textOffset, position - textOffset, width, height), sliderValue.ToString ());
		}
	}
}
