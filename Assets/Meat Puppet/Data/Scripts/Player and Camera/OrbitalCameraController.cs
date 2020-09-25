
using System;
using UnityEngine;

namespace PBG.MeatPuppet {
	public class OrbitalCameraController : MonoBehaviour {

		[Tooltip("Affects the point in space relative to the character that the camera focuses on. " +
			"Higher values cause the camera to focus on the top of or above the character, " +
			"negative values focus the camera below.")]
		[NonSerialized]public float cameraOffsetY = 1.0f;
		[Tooltip("Affects the point in space relative to the character that the camera focuses on. " +
			"Positive values cause the camera to focus to the right the character (over the shoulder), " +
			"negative values focus the camera on the left.")]
		[NonSerialized]public float cameraOffsetX = 0.0f;
		[Tooltip("Limits how far the camera can rotate vertically down. Values below -90 will cause bad spinning.")]
		[NonSerialized]public float cameraMinAngleY = -50.0f;
		[Tooltip("Limits how far the camera can rotate vertically up. Values above 90 will cause bad spinning.")]
		[NonSerialized]public float cameraMaxAngleY = 50.0f;
		[Tooltip("Limits how far the camera can zoom in. Must be greater than 0.")]
		[NonSerialized]public float cameraMinZ = 3.0f;
		[Tooltip("Limits how far the camera can zoom out.")]
		[NonSerialized]public float cameraMaxZ = 60.0f;
		[Tooltip("The initial distance of the camera.")]
		[NonSerialized]public float cameraDistance = 3;
		[Tooltip("The sensitivity of the camera as it moves around the X-axis (left and right). Negative values with invert the controls.")]
		[NonSerialized]public float camSmoothX = 4.0f;
		[Tooltip("The sensitivity of the camera as it moves around the Y-axis (up and down). Negative values with invert the controls.")]
		[NonSerialized]public float camSmoothY = -1.2f;
		[Tooltip("The sensitivity of the camera as it zooms in and out. Negative values with invert the controls.")]
		[NonSerialized]public float camSmoothZ = -10.0f;

		public float cameraX = -155.6f;
		public float cameraY = 15.7f;
		
		[Tooltip("While true, pressing 'Tab' while playing in the editor will hide/unhide the mouse cursor.")]
		public bool useQuickCursorToggle = true;

		public bool startWithMouseCaptured = false;

		public bool showGUIMessage = true;

		#region Private Variables

		Camera cam;
		Transform camTransform;
		
		Vector3 cameraOffset;


		#endregion

		void Start() {
			// Grab a handle to the camera
			cam = Camera.main;
			camTransform = cam.transform;
			// Lock and hide the cursor
			if (startWithMouseCaptured) {
				Cursor.lockState = CursorLockMode.Locked;
			}

		}

		void Update() {
			if (Cursor.lockState==CursorLockMode.Locked) {
				CalculatePosition();
			}

			Hotkeys();
		}

		void Hotkeys() {

			if (useQuickCursorToggle && Application.isPlaying && Input.GetKeyDown(KeyCode.Tab)) {
				// Lock and hide the cursor
				if (Cursor.lockState == CursorLockMode.Locked) {
					Cursor.lockState = CursorLockMode.None;
				}
				else {
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
		}

		void FixedUpdate() {
			//if (Cursor.lockState == CursorLockMode.Locked) {
				Reposition();
			//}
		}

		public void CalculatePosition() {
			//controllerState.Update();
			cameraX += Input.GetAxis("Mouse X") * camSmoothX;
			cameraY += Input.GetAxis("Mouse Y") * camSmoothY;

			//Debug.Log(string.Format("{0},{1}", cameraX, cameraY));

			cameraY = Mathf.Clamp(cameraY, cameraMinAngleY, cameraMaxAngleY);

			cameraDistance += Input.GetAxis("Mouse ScrollWheel") * camSmoothZ;
			cameraDistance = Mathf.Clamp(cameraDistance, cameraMinZ, cameraMaxZ);
		}

		public void Reposition() {
			if (cam == null) {
				// Grab a handle to the camera
				cam = Camera.main;
				camTransform = cam.transform;
			}

			// go out from this character a certain distance by a certain angle
			Vector3 dir = new Vector3(0, 0, -cameraDistance);
			Quaternion rotation = Quaternion.Euler(cameraY, cameraX, 0);
			cameraOffset.Set(cameraOffsetX, cameraOffsetY, 0.0f);

			// perform a ray cast out from the character
			// if we hit something we can put our camera there in order to stay in front of things that'll obstruct our view
			//Ray camRay = new Ray(transform.position + cameraOffset, rotation * -Vector3.forward);
			//RaycastHit camRayhit;
			//if (cameraCollidesWithTerrain && Physics.Raycast(camRay, out camRayhit, cameraDistance, collideLayer)) {
			//	camTransform.position = Vector3.Lerp(camTransform.position, camRayhit.point, 0.8f);
			//}
			//else {
			camTransform.position = Vector3.Lerp(camTransform.position, transform.position + rotation * (dir + cameraOffset), 0.8f);
			//}

			Vector3 focusPoint = transform.position + rotation * (cameraOffset);
			camTransform.LookAt(focusPoint);
			//Vector3 forward = camTransform.forward.DropY();

		}

		public void OnGUI() {
			//if (useQuickExit) {
			//	GUI.Label(new Rect(10, 10, 300, 200), "Press Esc to end program");
			//}

			if (showGUIMessage) {
				if (Cursor.lockState == CursorLockMode.Locked) {
					GUI.Label(new Rect(10, 30, 300, 200), "Press Tab to unlock the cursor");
				}
				else {
					GUI.Label(new Rect(10, 30, 300, 200), "Press Tab to lock the cursor");
				}
			}
		}

#if UNITY_EDITOR

		private void OnValidate() {
			Reposition();

		}
#endif
	}

}
