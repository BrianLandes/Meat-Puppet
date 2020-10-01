
using UnityEngine;
namespace PBG.MeatPuppet {
	public class MoveInCircle : MonoBehaviour {
		public Transform center;
		public float radius = 10f;

		public float speed = 2f;

		private float time = 0f;

		private void Update() {
			time += Time.deltaTime;

			float x = center.position.x + radius * Mathf.Cos(time * speed);
			float y = center.position.y;
			float z = center.position.z + radius * Mathf.Sin(time * speed);

			transform.position = new Vector3(x, y, z);
		}
	}

}
