
using UnityEngine;

public class ParameterStudioDriver : MonoBehaviour {

	public Animator animator;

	public float forwardSpeed = 0.0F;
	public float turnSpeed = 0.0F;
	public float horizontalSpeed = 0.0F;
	public float groundSteep = 0.0F;

	private bool jumping = false;

	void OnGUI() {

		float rowHeight = 20;
		//float rowWidth = 100;
		float rowSpacing = 3f;

		float labelWidth = 100f;
		float sliderWidth = 100f;
		float buttonWidth = 60f;

		float labelLeftPosition = 10;
		float sliderLeftPosition = labelLeftPosition + labelWidth + 20f;
		float buttonLeftPosition = sliderLeftPosition + sliderWidth + 10f;

		float startingVerticalPosition = 60f;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Forward Speed");
		forwardSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), forwardSpeed, -1.0F, 2.0F);
		animator.SetFloat("Forward Speed", forwardSpeed);
		if (GUI.Button(new Rect(buttonLeftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Reset")) {
			forwardSpeed = 0f;
		}

		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Turn Speed");
		turnSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), turnSpeed, -1.0F, 1.0F);
		animator.SetFloat("Turn Speed", turnSpeed);
		if (GUI.Button(new Rect(buttonLeftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Reset")) {
			turnSpeed = 0f;
		}

		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Horizontal Speed");
		horizontalSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), horizontalSpeed, -1.0F, 1.0F);
		animator.SetFloat("Horizontal Speed", horizontalSpeed);
		if (GUI.Button(new Rect(buttonLeftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Reset")) {
			horizontalSpeed = 0f;
		}
		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Ground Steep");
		groundSteep = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), groundSteep, -1.0F, 1.0F);
		animator.SetFloat("Ground Steep", groundSteep);
		if (GUI.Button(new Rect(buttonLeftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Reset")) {
			groundSteep = 0f;
		}

		
		startingVerticalPosition += rowHeight + rowSpacing;

		if (!jumping) {
			if (GUI.Button(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Jump")) {
				animator.SetTrigger("Jump");
				animator.SetBool("Grounded", false);
				jumping = true;
			}
			if (GUI.Button(new Rect(sliderLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Unground")) {
				//animator.SetTrigger("Jump");
				animator.SetBool("Grounded", false);
				jumping = true;
			}
		}
		else {
			if (GUI.Button(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Land")) {
				animator.SetBool("Grounded", true);
				jumping = false;
			}
		}
		
	}
}
