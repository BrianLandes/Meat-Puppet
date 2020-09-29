
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

			float buttonWidth = 210f;

			float leftPosition = Screen.width - 10 - buttonWidth;

			float startingVerticalPosition = 60f;
			
			if (!looping) {
				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Dancing Loop")) {
					puppet.Animation.StartLoopingAnimation(dancingLoop);
					looping = true;
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Dancing Torso Loop")) {
					puppet.Animation.StartLoopingAnimation(dancingLoop, overrideLegs:false );
					looping = true;
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Sitting Loop")) {
					sittingConfiguration.overrideLegs = true;
					puppet.Animation.StartLoopingAnimation(sittingConfiguration);
					looping = true;
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start Sitting Loop Torso")) {
					sittingConfiguration.overrideLegs = false;
					puppet.Animation.StartLoopingAnimation(sittingConfiguration);
					looping = true;
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot A")) {
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationA);
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot A Torso")) {
					var configuration = new MeatPuppetOneShotAnimationConfiguration() {
						overrideLegs = false,
					};
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationA, configuration);
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot B")) {
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationB);
				}
				startingVerticalPosition += rowHeight + rowSpacing;

				if (GUI.Button(new Rect(leftPosition, startingVerticalPosition, buttonWidth, rowHeight), "Start One Shot B Torso")) {
					var configuration = new MeatPuppetOneShotAnimationConfiguration() {
						overrideLegs = false,
					};
					puppet.Animation.PlayOneShotAnimation(oneShotAnimationB, configuration);
				}
				startingVerticalPosition += rowHeight + rowSpacing;
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
