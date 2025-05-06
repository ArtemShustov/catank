using System;
using UnityEngine;

namespace Game.Characters.States {
	[Serializable]
	public class FallState: AbstractState {
		[SerializeField] private float _gravityModifier = 1;
		
		public override void Update() {
			Character.SharedData.Velocity += Physics.gravity * (_gravityModifier * Time.deltaTime);
			var movement = Character.SharedData.Velocity * Time.deltaTime;
			
			Character.Controller.Move(movement);
			
			if (Character.Controller.isGrounded) {
				if (Character.Input.Player.Move.ReadValue<Vector2>().sqrMagnitude > Mathf.Epsilon) {
					Character.ChangeState<WalkState>();
				} else {
					Character.ChangeState<IdleState>();
				}
				Character.SharedData.Velocity = Vector3.zero;
				return;
			}
		}

		public override void OnEnter() {
			Character.Animator.SetIsFall(true);
		}
		public override void OnExit() {
			Character.Animator.SetIsFall(false);
		}
	}
}