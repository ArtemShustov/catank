using System;
using UnityEngine;

namespace Game.Characters.States {
	[Serializable]
	public class IdleState: AbstractState {
		public override void Update() {
			var input = GetMoveInput();
			if (input.sqrMagnitude > Mathf.Epsilon) {
				Character.ChangeState<WalkState>();
				return;
			}
			if (Character.Input.Player.Jump.WasPerformedThisFrame()) {
				Character.ChangeState<JumpState>();
				return;
			}
		}

		private Vector2 GetMoveInput() => Character.Input.Player.Move.ReadValue<Vector2>();
	}
}