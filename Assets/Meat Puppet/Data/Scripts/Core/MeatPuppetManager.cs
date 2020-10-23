
//using UnityEngine;

//namespace PBG.MeatPuppet {
//	public class MeatPuppetManager : MonoBehaviour {

//		[Tooltip("The animation controller assigned across all puppets.")]
//		public RuntimeAnimatorController meatPuppetAnimationController;

//		[Tooltip("The layers that will be considered 'ground' to the puppets.")]
//		public LayerMask groundLayer;

//		[Tooltip("The layers that will be considered static obstacles to the puppets.")]
//		public LayerMask staticLayer;

//		public PhysicMaterial characterPhysicMaterial;
		

//		//public AnimationCurve forwardSpeedCurve;
		
//		public static MeatPuppetManager Instance {
//			get {
//				if (_instance == null) {
//					_instance = GameObject.FindObjectOfType<MeatPuppetManager>();
//				}
//				// TODO: throw an error if still null
//				return _instance;
//			}
//			private set {
//				_instance = value;
//			}
//		}
//		public static MeatPuppetManager _instance;

//		private void Awake() {
//			Instance = this;
//		}
//	}
//}