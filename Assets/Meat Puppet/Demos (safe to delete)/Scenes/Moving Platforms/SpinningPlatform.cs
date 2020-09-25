
using UnityEngine;
namespace PBG.MeatPuppet {
	public class SpinningPlatform : MonoBehaviour {

		public float speed = 2f;


		public void Update() {

			transform.Rotate(Vector3.up, speed);

		}
	}

}
