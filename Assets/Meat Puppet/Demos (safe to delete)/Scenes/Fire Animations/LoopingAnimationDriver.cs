
using System;
using UnityEngine;

namespace  PBG.MeatPuppet {
	public class LoopingAnimationDriver : MonoBehaviour {
		public MeatPuppet puppet;

		public AnimationClip dancingLoop;
		
		public MeatPuppetLoopingAnimationConfiguration sittingConfiguration;

		public AnimationClip oneShotAnimationA;
		public AnimationClip oneShotAnimationB;
		
		private bool looping = false;

		private void OnGUI() {
			
			float rowHeight = 20;
			float rowSpacing = 3f;

			float buttonWidth = 110f;

			float leftPosition = 10;

			float startingVerticalPosition = 60f;
			
			if (!looping) {
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Loop")) {
					puppet.Animation.StartLoopingAnimation(dancingLoop);
					looping = true;
				}

				startingVerticalPosition += rowHeight + rowSpacing;
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Loop")) {
					puppet.Animation.StartLoopingAnimation(sittingConfiguration);
					looping = true;
				}
				
				startingVerticalPosition += rowHeight + rowSpacing;
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot A")) {
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationA);
				}
				
				startingVerticalPosition += rowHeight + rowSpacing;
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot B")) {
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationB);
				}
			}
			else {
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Stop Loop")) {
					puppet.Animation.StopLoopingAnimation();
					looping = false;
				}
			}
		}
	}

}
