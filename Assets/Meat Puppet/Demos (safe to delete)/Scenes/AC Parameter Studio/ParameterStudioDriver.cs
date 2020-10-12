
using UnityEngine;

public class ParameterStudioDriver : MonoBehaviour {

	public Animator animator;

	public float forwardSpeed = 0.0F;
	public float turnSpeed = 0.0F;
	public float horizontalSpeed = 0.0F;
	public float groundSteep = 0.0F;
	public float landForce = 0.0F;
	public float acceleration = 0.0F;

	private bool jumping = false;

	void OnGUI() {

		float rowHeight = 20;
		//float rowWidth = 100;
		float rowSpacing = 3f;

		float labelWidth = 100f;
		float sliderWidth = 100f;
		float buttonWidth = 20f;

		float labelLeftPosition = 10;
		float sliderLeftPosition = labelLeftPosition + labelWidth + 20f;
		float buttonLeftPosition = sliderLeftPosition + sliderWidth + 10f;

		float startingVerticalPosition = 60f;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Forward Speed");
		forwardSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), forwardSpeed, -2.0F, 2.0F);
		animator.SetFloat("Forward Speed", forwardSpeed);

		float leftPostiion = buttonLeftPosition;
		foreach ( var value in new float[] {-2f, -1f, 0f, 1f, 2f }) {
			if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
				forwardSpeed = value;
			}
			leftPostiion += buttonWidth;
		}
		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Turn Speed");
		turnSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), turnSpeed, -1.0F, 1.0F);
		animator.SetFloat("Turn Speed", turnSpeed);
		leftPostiion = buttonLeftPosition;
		foreach (var value in new float[] { -1f, 0f, 1f }) {
			if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
				turnSpeed = value;
			}
			leftPostiion += buttonWidth;
		}
		startingVerticalPosition += rowHeight + rowSpacing;

		//GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Acceleration");
		//acceleration = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), acceleration, -1.0F, 1.0F);
		//animator.SetFloat("Acceleration", acceleration);
		//leftPostiion = buttonLeftPosition;
		//foreach (var value in new float[] { -1f, 0f, 1f }) {
		//	if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
		//		acceleration = value;
		//	}
		//	leftPostiion += buttonWidth;
		//}
		//startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Horizontal Speed");
		horizontalSpeed = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), horizontalSpeed, -2.0F, 2.0F);
		animator.SetFloat("Horizontal Speed", horizontalSpeed);
		leftPostiion = buttonLeftPosition;
		foreach (var value in new float[] { -2f, -1f, 0f, 1f, 2f }) {
			if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
				horizontalSpeed = value;
			}
			leftPostiion += buttonWidth;
		}
		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Ground Steep");
		groundSteep = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), groundSteep, -1.0F, 1.0F);
		animator.SetFloat("Ground Steep", groundSteep);
		leftPostiion = buttonLeftPosition;
		foreach (var value in new float[] { -1f, 0f, 1f }) {
			if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
				groundSteep = value;
			}
			leftPostiion += buttonWidth;
		}
		startingVerticalPosition += rowHeight + rowSpacing;

		GUI.Label(new Rect(labelLeftPosition, startingVerticalPosition, labelWidth, rowHeight), "Land Force");
		landForce = GUI.HorizontalSlider(new Rect(sliderLeftPosition, startingVerticalPosition + rowSpacing, sliderWidth, rowHeight), landForce, 0.0F, 1.0F);
		animator.SetFloat("Land Force", landForce);
		leftPostiion = buttonLeftPosition;
		foreach (var value in new float[] { 0f, 1f }) {
			if (GUI.Button(new Rect(leftPostiion, startingVerticalPosition, buttonWidth, rowHeight), "" + value)) {
				landForce = value;
			}
			leftPostiion += buttonWidth;
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


		animator.SetFloat("Any Speed", Mathf.Abs(horizontalSpeed) + Mathf.Abs(forwardSpeed) + Mathf.Abs(turnSpeed) );
	}
}
